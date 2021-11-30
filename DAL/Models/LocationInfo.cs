using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
	public class LocationInfo
	{
		public enum LocationType
		{
			local,
			gdrive,
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string Path { get; set; } //general folder - i.e. C:\Program Files
		public LocationType Type { get; set; } = LocationType.local;
	}
}
