using LS.Core.Interfaces;
using System;

namespace LS.Domain
{
    public class ExternalDuplicateData
    {
        public string ID { get; set; }
        public string NEW_ENROLLMENT { get; set; }
        public DateTime CREATIONDATE { get; set; }
        public DateTime QUALIFYDATE { get; set; }
        public string ORGANIZATION { get; set; }
        public string AGENTNAME { get; set; }
        public string STATUS { get; set; }
        public string OLD_ENROLLMENTNUMBER { get; set; }
        public string CHECK_DEENROLL { get; set; }
        public DateTime OLD_DEENROLL_DATE { get; set; }
        public string OLD_ENROLLMENT_STATUS { get; set; }
        public DateTime OLD_ENROLLMENT_QUALIFYDATE { get; set; }
        public string EXPIRED { get; set; }
        public DateTime DateImported { get; set; }
    }
}
