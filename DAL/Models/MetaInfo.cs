using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;

namespace DAL.Models
{
	public class MetaInfo
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; } //Book1 (no ext)
		public string Type { get; set; } //.pdf
		public DateTime Timestamp { get; set; }

		public ICollection<ChunkInfo> ChunkInfoSet { get; set; }
		
		public MetaInfo()
		{
			Timestamp = new DateTime();
		}

		public MetaInfo(string fileName)
		{
			var parts = fileName.Split(".");

			Name = parts[0];
			Type = parts[1];
			Timestamp = DateTime.UtcNow;
		}

	}
}
