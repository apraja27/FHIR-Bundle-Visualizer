using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHIR_Bundle_Visualizer.FHIR_Models
{
	public class Entry
	{
		public string fullUrl { get; set; }
		public Resource resource { get; set; }
		public Request request { get; set; }
	}
}
