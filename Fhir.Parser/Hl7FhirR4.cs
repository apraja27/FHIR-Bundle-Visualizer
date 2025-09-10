extern alias R4;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                patientDetails.Name = patient.Name[0].ToString();
                patientDetails.BirthDate = patient.BirthDate;
                DateTime birthDate = new DateTime();
                DateTime.TryParse(patient.BirthDate.ToString(), out birthDate);
                TimeSpan age = DateTime.UtcNow - birthDate;
                patientDetails.Age = ((int)(age.TotalDays / 365)).ToString() + " Years";
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
