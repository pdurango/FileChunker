using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.Models;
using FileChunker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class FileController : ControllerBase
	{
		private readonly ILogger<LocationController> m_logger;
		private readonly DataContext m_context;

		public FileController(ILogger<LocationController> logger, DataContext context)
		{
			m_logger = logger;
			m_context = context;
		}

		/*[HttpGet("{id}")]
		public async Task<ActionResult<LocationInfo>> Get(int id)
		{
			return Ok(await m_context.LocationInfoSet.FirstOrDefaultAsync(l => l.Id == id));
		}*/

		//[RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
		[HttpPost]
		[RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
		[RequestSizeLimit(209715200)]
		public async Task<ActionResult> AddFile(IFormFile file)
		{
			if (file == null || file.Length == 0)
				return Content("file not selected");

			int similarFileCount = m_context.MetaInfoSet
				.Count(f => f.Name.EndsWith(Path.GetFileNameWithoutExtension(file.FileName )));
			var fileName = similarFileCount == 0 ? file.FileName : $"({similarFileCount}){file.FileName}";
			//where the file will be stored temporarily
			var tmpFile = Path.Combine(Path.GetTempPath(), fileName);
			//where the file chunks will be stored temporarily
			var chunkDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(fileName));
			Directory.CreateDirectory(chunkDir);
			using (var stream = new FileStream(tmpFile, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			//Get available locations
			var locations = m_context.LocationInfoSet.ToList();
			var metaInfo = new MetaInfo(fileName);
			await m_context.MetaInfoSet.AddAsync(metaInfo);
			await m_context.SaveChangesAsync(); //Need to save to generate Id

			try
			{
				Chunker.SplitFile(tmpFile, chunkDir);
				List<ChunkInfo> chunks = Chunker.ScatterChunks(chunkDir, locations, metaInfo);

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
					Directory.Delete(chunkDir, true);
			}

			return Created("", metaInfo);
		}

		[HttpGet("{fileName}")]
		public async Task<IActionResult> GetFile(string fileName)  
		{  
			if (fileName == null)  
				return Content("filename not present");

			var chunks = m_context.ChunkInfoSet
				.Where(f => f.MetaInfo.Name == Path.GetFileNameWithoutExtension(fileName))
				.Include(l => l.LocationInfo)
				.ToList();

			if(chunks.Count == 0)
				return Content("filename does not exist");

			string file = Chunker.MergeChunks(chunks, fileName);

			var memory = new MemoryStream();
			using (var stream = new FileStream(file, FileMode.Open))
			{
				await stream.CopyToAsync(memory);
			}
			memory.Position = 0;

			new FileExtensionContentTypeProvider().TryGetContentType(file, out string contentType);
			return File(memory, contentType ?? "application/octet-stream", Path.GetFileName(file));
		} 

		/*[HttpDelete]
		public async Task<ActionResult> DeleteLocation(int id)
		{
			var location = await m_context.LocationInfoSet.FirstOrDefaultAsync(l => l.Id == id);
			m_context.Remove(location);
			await m_context.SaveChangesAsync();

			return Ok();
		}*/
	}
}
