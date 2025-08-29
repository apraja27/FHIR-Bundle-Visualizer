﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHIR_Bundle_Visualizer.FHIR_Models
{
    class FHIRBundle
    {
        public string resourceType { get; set; }
        public string type { get; set; }
        public List<Entry> entry { get; set; }
    }
}
