extern alias R4;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FHIR_Bundle_Visualizer.Models;

namespace FHIR_Bundle_Visualizer.Fhir.Parser
{
    public class Hl7FhirR4
    {
        public static PatientDetails GetPatientDetails(Bundle bundle)
        {
            PatientDetails patientDetails = new PatientDetails();
            try
            {
                var patient = bundle.Entry
                                    .Select(e => e.Resource)
                                    .OfType<R4::Hl7.Fhir.Model.Patient>()
                                    .FirstOrDefault();
                if (patient != null)
                {
                    string prefix = string.Empty;
                    string familyName = string.Empty;
                    string givenName = string.Empty;
                    if (patient.Name != null && patient.Name.Count() > 0)
                    {
                        prefix = patientDetails.GetPrefix(patient.Name[0].Prefix);
                        familyName = patient.Name[0].Family.Trim();
                        givenName = patientDetails.GetGivenName(patient.Name[0].Given);
                    }
                    patientDetails.Name = $"{prefix} {familyName} {givenName}";
                    patientDetails.NHS = "Unknown";
                    if (patient.BirthDate != null)
                    {
                        patientDetails.BornDetail = patientDetails.GetBornDetails(patient.BirthDate);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return patientDetails;
        }

        public static string SerializeToString(Base resource)
        {
            var serializer = new R4::Hl7.Fhir.Serialization.FhirJsonSerializer(new SerializerSettings { Pretty = true });
            string jsonString = serializer.SerializeToString(resource);
            return jsonString;
        }

        public static Bundle DeserializeFromString(string jsonString)
        {
            var parser = new R4::Hl7.Fhir.Serialization.FhirJsonParser();
            Bundle bundle = parser.Parse<Bundle>(jsonString);
            return bundle;
        }
    }
}
