using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LS.Domain;
using System;

namespace ApiBindingModels {

    public class SchedTaskReturnModel {
        public string Content { get; set; }
        public bool SendEmail { get; set; }
    }

    public class EnrollmentRow {
        public string Id { get; set; }
        public string EnrollmentNumber { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string CreationDateTime { get; set; }
        public string Status { get; set; }
        public string PromoCode { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }

        public EnrollmentRow(string currLine) {
            string[] cells = currLine.Split(new char[] { '|' }, StringSplitOptions.None);

            string lat = null;
            string lon = null;
            string LatLon = cells[7].Trim();
            if (LatLon.Length > 0) {
                string[] LatLonVals = LatLon.Split(new char[] { ',' }, StringSplitOptions.None);
                lat = LatLonVals[0].Trim();
                lon = LatLonVals[1].Trim();
            }

            Id = cells[0].Trim();
            EnrollmentNumber = cells[1].Trim();
            State = cells[2].Trim();
            Zipcode = cells[3].Trim();
            CreationDateTime = cells[4].Trim();
            Status = cells[5].Trim();
            PromoCode = cells[6].Trim();
            Lat = lat;
            Lon = lon;
        }
    }

    public class ActivationsRow {
        public string PromoCode { get; set; }
        public string ESN { get; set; }
        public string EnrollmentNumber { get; set; }
        public string ActivationDateTime { get; set; }
        public string DeviceType { get; set; }

        public ActivationsRow(string currLine) {
            string[] cells = currLine.Split(new char[] { '|' }, StringSplitOptions.None);

            PromoCode = cells[0].Trim();
            ESN = cells[1].Trim();
            EnrollmentNumber = cells[2].Trim();
            ActivationDateTime = cells[3].Trim();
            try { DeviceType = cells[4].Trim(); } catch {}
        }
    }

    public class RosterRow {
        public string PromoCode { get; set; }
        public string Status { get; set; }

        public RosterRow(string currLine) {
            string[] cells = currLine.Split(new char[] { '|' }, StringSplitOptions.None);

            PromoCode = cells[0].Trim();
            Status = cells[1].Trim();
        }
    }

    public class ShipmentsRow {
        public string IMEI { get; set; }
        public string PartNumber { get; set; }
        public string RetailPoNumber { get; set; }
        public string TFOrderNumber { get; set; }
        public string ShipDate { get; set; }

        public ShipmentsRow(string currLine) {
            string[] cells = currLine.Split(new char[] { '|' }, StringSplitOptions.None);

            IMEI = cells[0].Trim();
            PartNumber = cells[1].Trim();
            RetailPoNumber = cells[2].Trim();
            TFOrderNumber = cells[3].Trim();
            ShipDate = cells[4].Trim();
        }
    }

    public class StatusHistoryRow {
        public string Id { get; set; }
        public string ChangeDate { get; set; }
        public string NewStatus { get; set; }

        public StatusHistoryRow(string currLine) {
            string[] cells = currLine.Split(new char[] { '|' }, StringSplitOptions.None);

            Id = cells[0].Trim();
            ChangeDate = cells[1].Trim();
            NewStatus = cells[2].Trim();
        }
    }
}