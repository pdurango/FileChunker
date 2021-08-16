using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
	public class ChunkInfo
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; } //i.e. 0.dat (incude ext)

		public virtual MetaInfo MetaInfo { get; set; }
		public virtual LocationInfo LocationInfo { get; set; }
	}
}
