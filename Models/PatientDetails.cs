using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHIR_Bundle_Visualizer.Models
{
    public class PatientDetails
    {
        public string Name { get; set; }
        public string BornDetail { get; set; }
        public string NHS { get; set; }

		public string GetPrefix(IEnumerable<string> prefixes)
		{
			string prefix = string.Empty;
			if (prefixes != null && prefixes.Count() > 0)
			{
				foreach (var item in prefixes)
				{
					prefix += $" {item}";
				}
			}
			return prefix.Trim();
		}
		public string GetGivenName(IEnumerable<string> givenNames)
		{
			string givenName = string.Empty;
			if (givenNames != null && givenNames.Count() > 0)
			{
				foreach (var item in givenNames)
				{
					givenName += $" {item}";
				}
			}
			return givenName.Trim();
		}
		public string GetBornDetails(string birthDate)
		{
			string bornDetail = string.Empty;
			DateTime temp = new DateTime();
			DateTime.TryParse(birthDate, out temp);
			TimeSpan age = DateTime.UtcNow - temp;
			int totalDays = (int)age.TotalDays;
			if (totalDays < 365)
			{
				int weeks = totalDays / 7;
				int days = totalDays % 7;
				bornDetail = $"{weeks} W {days} d";
			}
			else
			{
				int years = DateTime.UtcNow.Year - temp.Year;
				int months = DateTime.UtcNow.Month - temp.Month;
				if (months < 0)
				{
					years--;
					months += 12;
				}
				bornDetail = $"{years} Y {months} M";
			}
			bornDetail = $"{temp.ToString("dd-MMM-yyyy")} ({bornDetail})";
			return bornDetail;
		}
	}
}
