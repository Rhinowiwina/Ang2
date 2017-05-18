using LS.Core.Interfaces;
using System;

namespace LS.Domain
{
    public class ExternalDataTrueUp
    {
        public string ID { get; set; }
        public string ENROLLMENTNUMBER { get; set; }
        public string ENROLLMENTCHANNEL { get; set; }
        public string STATUS { get; set; }
        public int STATUSID { get; set; }
        public string PROMOCODE { get; set; }
        public DateTime CREATIONDATE_EST { get; set; }
        public string STATE { get; set; }
        public string ZIPCODE { get; set; }
        public string CREATIONTIME { get; set; }
        public string USAC_FORM { get; set; }
        public string REP_NAME { get; set; }
        public string BATCH_DATE { get; set; }
        public string GROUP { get; set; }
        public string CHANNEL { get; set; }
        public string DMA { get; set; }
        public string GEOLOCATION { get; set; }
        public DateTime DateImported { get; set; }
    }
}
