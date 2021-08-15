using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
	public class FileInfo
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; }
		public string Type { get; set; }
		public DateTime Timestamp { get; set; }

		public ICollection<ChunkInfo> ChunkInfoSet { get; set; }
		
		public FileInfo()
		{
			Timestamp = new DateTime();
		}
	}
}
