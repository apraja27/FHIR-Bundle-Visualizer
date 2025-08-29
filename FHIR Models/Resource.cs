using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHIR_Bundle_Visualizer.FHIR_Models
{
    public class Resource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public Text text { get; set; }
        public string status { get; set; }
        public MClass @class { get; set; }
        public List<MType> type { get; set; }
        public Subject subject { get; set; }
        public List<Participant> participant { get; set; }
        public Period period { get; set; }
        public Individual serviceProvider { get; set; }
        public List<Extension> extension { get; set; }
        public List<Identifier> identifier { get; set; }
        public List<Name> name { get; set; }
        public List<Telecom> telecom { get; set; }
        public string gender { get; set; }
        public string birthDate { get; set; }
        public List<Address> address { get; set; }
        public MType maritalStatus { get; set; }
        public bool multipleBirthBoolean { get; set; }
        public List<Communication> communication { get; set; }

        public string intent { get; set; }
        public Requester requester { get; set; }
        public List<Recipient> recipient { get; set; }
        public List<ContainedResource> contained { get; set; }
        public Reference patient { get; set; }
        public Period billablePeriod { get; set; }
        public DateTime created { get; set; }
        public Recipient provider { get; set; }
        public Recipient organization { get; set; }
        public Reference referral { get; set; }
        public Reference claim { get; set; }
        public List<CareTeam> careTeam { get; set; }
        public List<Diagnosis> diagnosis { get; set; }
        public Insurance insurance { get; set; }
        public List<Item> item { get; set; }
        public Money totalCost { get; set; }
        public Payment payment { get; set; }
    }

    public class ContainedResource
    {
        public string resourceType { get; set; }
        public string id { get; set; }

        // Coverage-specific
        public MType type { get; set; }

        // ReferralRequest-specific
        public string status { get; set; }
        public string intent { get; set; }
        public Subject subject { get; set; }
        public Requester requester { get; set; }
        public List<Recipient> recipient { get; set; }
    }

    public class MClass
    {
        public string system { get; set; }
        public string code { get; set; }
    }
    public class MType
    {
        public List<Coding> coding { get; set; }
        public string text { get; set; }
    }

    public class Coding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }

    public class Subject
    {
        public string reference { get; set; }
    }

    public class Participant
    {
        public Individual individual { get; set; }
    }

    public class Individual
    {
        public string reference { get; set; }
        public string display { get; set; }
    }

    public class Period
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }

    public class Text
    {
        public string status { get; set; }
        public string div { get; set; }
    }

    public class Extension
    {
        string url { get; set; }
        public Coding valueCoding { get; set; }
        public string valueString { get; set; }
        public string valueCode { get; set; }
        public ValueAddress valueAddress { get; set; }
        public decimal? valueDecimal { get; set; }
    }

    public class ValueAddress
    {
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
    }

    public class Identifier
    {
        public MType type { get; set; }
        public string system { get; set; }
        public string value { get; set; }
    }
    public class Name
    {
        public string use { get; set; }
        public string family { get; set; }
        public List<string> given { get; set; }
        public List<string> prefix { get; set; }
    }
    public class Telecom
    {
        public string system { get; set; }
        public string value { get; set; }
        public string use { get; set; }
    }
    public class Address
    {
        public List<AddressExtension> extension { get; set; }
        public List<string> line { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
    }
    public class AddressExtension
    {
        public string url { get; set; }
        public List<GeoExtension> extension { get; set; }
    }
    public class GeoExtension
    {
        public string url { get; set; }
        public decimal valueDecimal { get; set; }
    }
    
    public class Communication
    {
        public MType language { get; set; }
    }
    public class Requester
    {
        public Identifier agent { get; set; }
    }

    public class Recipient
    {
        public Identifier identifier { get; set; }
    }
    public class Reference
    {
        public string reference { get; set; }
        public string display { get; set; }
    }   
    public class CareTeam
    {
        public int sequence { get; set; }
        public Recipient provider { get; set; }
        public MType role { get; set; }
    }
    public class Diagnosis
    {
        public List<Extension> extension { get; set; }
        public int sequence { get; set; }
        public Reference diagnosisReference { get; set; }
        public List<MType> type { get; set; }
    }
    public class Insurance
    {
        public Reference coverage { get; set; }
    }
    public class Item
    {
        public int sequence { get; set; }
        public List<int> diagnosisLinkId { get; set; }
        public MType category { get; set; }
        public Period servicedPeriod { get; set; }
        public MType locationCodeableConcept { get; set; }
        public List<Reference> encounter { get; set; }
    }
    public class Money
    {
        public decimal value { get; set; }
        public string system { get; set; }
        public string code { get; set; }
    }
    public class Payment
    {
        public Money amount { get; set; }
    }

}