using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChunkServiceHandler;
using DAL;
using DAL.Models;
using FileChunker;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
	[EnableCors("_myAllowSpecificOrigins")]
	[ApiController]
	[Route("api/[controller]")]
	public class FileController : ControllerBase
	{
		private readonly ILogger<LocationController> m_logger;
		private readonly DataContext m_context;
		private readonly ChunkService m_chunkService;

		public FileController(ILogger<LocationController> logger, DataContext context, ChunkService chunkService)
		{
			m_logger = logger;
			m_context = context;
			m_chunkService = chunkService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<LocationInfo>>> GetAll()
		{
			return Ok(await m_context.MetaInfoSet.ToListAsync());
		}

		//todo - add progress tracking
		[HttpGet("{id}")]
		public async Task<IActionResult> GetFile(int id)
		{
			var chunks = m_context.ChunkInfoSet
				.Where(f => f.MetaInfo.Id == id)
				.Include(l => l.LocationInfo)
				.ToList();

			if (chunks.Count == 0)
				return Content("File does not exist.");

			var metaInfo = await m_context.MetaInfoSet.FirstOrDefaultAsync(m => m.Id == id);
			string file = ChunkService.MergeChunks(chunks, metaInfo);

			var memory = new MemoryStream();
			using (var stream = new FileStream(file, FileMode.Open))
			{
				await stream.CopyToAsync(memory);
			}
			memory.Position = 0;

			new FileExtensionContentTypeProvider().TryGetContentType(file, out string contentType);
			return File(memory, contentType ?? "application/octet-stream", Path.GetFileName(file));
		}

		//todo - i cant figure out how to pass in a List<LocationInfo> as a second param, 
		//so for now itll just be a csv of locationIds
		[HttpPost]
		[RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
		[RequestSizeLimit(524288000)]
		public async Task<ActionResult> AddFile(IFormFile file, [FromForm] string locations = null)
		{
			if (file == null || file.Length == 0)
				return Content("file not selected");

			int similarFileCount = m_context.MetaInfoSet
				.Count(f => f.Name.EndsWith(Path.GetFileNameWithoutExtension(file.FileName )));
			var fileName = similarFileCount == 0 ? file.FileName : $"({similarFileCount}){file.FileName}";
			//where the file will be stored temporarily
			var tmpFile = Path.Combine(Path.GetTempPath(), fileName);

			using (var stream = new FileStream(tmpFile, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			//Get available locations
			List<LocationInfo> locationsList = null;
			if (!string.IsNullOrEmpty(locations))
			{
				int[] locationsArr = Array.ConvertAll(locations.Split(','), int.Parse);
				locationsList = m_context.LocationInfoSet
					.Where(l => locationsArr.Contains(l.Id))
					.ToList();
			}
			else
				locationsList = m_context.LocationInfoSet.ToList();

			var metaInfo = new MetaInfo(fileName);
			await m_context.MetaInfoSet.AddAsync(metaInfo);
			await m_context.SaveChangesAsync(); //Need to save to generate Id

			try
			{
				ChunkService.SplitFile(tmpFile, chunkDir);
				List<ChunkInfo> chunks = ChunkService.ScatterChunks(chunkDir, locationsList, metaInfo);

				await m_context.ChunkInfoSet.AddRangeAsync(chunks);
				await m_context.SaveChangesAsync();
			}
			catch(Exception e)
			{
				m_context.MetaInfoSet.Remove(metaInfo);
				await m_context.SaveChangesAsync();
				return BadRequest(e);
			}
			finally
			{
				System.IO.File.Delete(tmpFile);
			}

			return Created("", metaInfo);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteFile(int id)
		{
			var chunks = m_context.ChunkInfoSet
				.Where(f => f.MetaInfo.Id == id)
				.Include(l => l.LocationInfo)
				.ToList();

			var paths = chunks.Select(c => c.LocationInfo.Path).Distinct().ToArray();
			var metaInfo = await m_context.MetaInfoSet.FirstOrDefaultAsync(m => m.Id == id);
			
			ChunkService.DeleteChunks(paths, metaInfo.Name);
			m_context.RemoveRange(chunks);
			m_context.Remove(metaInfo);
			await m_context.SaveChangesAsync();

			return Ok();
		}
	}
}

/*public static List<T> Execute<T>(MyDbContext context, string query) where T : class
{
	var result = context.Database
						.SqlQuery<T>(query)
						.ToList();

	return result;
}*/