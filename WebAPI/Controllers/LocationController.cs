using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
	[EnableCors("_myAllowSpecificOrigins")]
	[ApiController]
	[Route("api/[controller]")]
	public class LocationController : ControllerBase
	{
		private readonly ILogger<LocationController> m_logger;
		private readonly DataContext m_context;

		public LocationController(ILogger<LocationController> logger, DataContext context)
		{
			m_logger = logger;
			m_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<LocationInfo>>> GetAll()
		{
			return Ok(await m_context.LocationInfoSet.ToListAsync());
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<LocationInfo>> Get(int id)
		{
			return Ok(await m_context.LocationInfoSet.FirstOrDefaultAsync(l => l.Id == id));
		}

		[HttpPost]
		public async Task<ActionResult> AddLocation(LocationInfo info)
		{
			await m_context.LocationInfoSet.AddAsync(info);
			await m_context.SaveChangesAsync();

			return Created("", info);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteLocation(int id)
		{
			var location = await m_context.LocationInfoSet.FirstOrDefaultAsync(l => l.Id == id);
			if (location == null)
				return Problem($"Location {id} doesn't exist.", statusCode: (int)HttpStatusCode.BadRequest);

			if (m_context.ChunkInfoSet.Any(c => c.LocationInfo.Id == id))
				return Problem($"Location {id} is currently being used for files.", 
					statusCode: (int)HttpStatusCode.BadRequest);

			m_context.Remove(location);
			await m_context.SaveChangesAsync();

			return Ok();
		}
	}
}
