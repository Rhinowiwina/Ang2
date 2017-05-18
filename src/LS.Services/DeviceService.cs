using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using LS.Core;
using LS.Domain;
using LS.Core.Interfaces;
using LS.Services.Factories;

namespace LS.Services {
    public class DeviceService {

        public bool IsValidIMEI(string imei) {
            if (imei.Length != 15) {
                return false;
            }
            int[] posImei = new int[15];
            for (int innlop = 0; innlop < 15; innlop++) {
                posImei[innlop] = Convert.ToInt32(imei.Substring(innlop, 1));
                if (innlop % 2 != 0) posImei[innlop] = posImei[innlop] * 2;
                while (posImei[innlop] > 9) posImei[innlop] = (posImei[innlop] % 10) + (posImei[innlop] / 10);
            }

            var totalval = 0;
            foreach (int v in posImei) totalval += v;
            if (0 == totalval % 10) {
                return true;
            }
            return false;
        }

        public ServiceProcessingResult<ValidatedDevice> ValidateDeviceId(string input) {
            input = input.Trim();
            var result = new ServiceProcessingResult<ValidatedDevice>();

            var deviceValidationDetails = new ValidatedDevice();

            deviceValidationDetails.IsValid = false;
            deviceValidationDetails.Type = "Unknown";
            deviceValidationDetails.HEX = input;
            deviceValidationDetails.DEC = input;

            bool imeiIsValid = IsValidIMEI(input);

            if (imeiIsValid) {
                deviceValidationDetails.IsValid = true;
                deviceValidationDetails.Type = "IMEI";
            } else {

                Regex regexSpecChar = new Regex("[^a-zA-Z0-9]+");
                Regex regexLetters = new Regex("[a-zA-Z]+");

                if (!regexSpecChar.IsMatch(input)) {
                    if (input.Length == 11 && regexLetters.IsMatch(input)) {
                        input = input.Substring(0, 8);
                    }

                    if (input.Length == 8 || input.Length == 11) {
                        deviceValidationDetails.Type = "ESN";
                        if (input.Length == 8) {
                            deviceValidationDetails.HEX = input;

                            var firstPart = Convert.ToInt32(input.Substring(0, 2), 16).ToString("D3");
                            var lastPart = Convert.ToInt32(input.Substring(2), 16).ToString("D8");

                            deviceValidationDetails.DEC = firstPart + lastPart;
                            deviceValidationDetails.IsValid = true;

                        } else {
                            deviceValidationDetails.DEC = input;

                            var firstPart = Convert.ToInt32(input.Substring(0, 3)).ToString("X2");
                            var lastPart = Convert.ToInt32(input.Substring(3)).ToString("X6");

                            deviceValidationDetails.HEX = firstPart + lastPart;
                            deviceValidationDetails.IsValid = true;
                        }
                    } else if (input.Length == 14 || input.Length == 18) {
                        deviceValidationDetails.Type = "MEID";

                        if (input.Length == 14) {
                            deviceValidationDetails.HEX = input;

                            var firstPart = Convert.ToInt64(input.Substring(0, 8), 16).ToString("D10");
                            var lastPart = Convert.ToInt64(input.Substring(8), 16).ToString("D8");

                            deviceValidationDetails.DEC = firstPart + lastPart;
                            deviceValidationDetails.IsValid = true;
                        } else {
                            deviceValidationDetails.DEC = input;

                            var firstPart = Convert.ToInt64(input.Substring(0, 10)).ToString("X8");
                            var lastPart = Convert.ToInt64(input.Substring(10)).ToString("X6");

                            deviceValidationDetails.HEX = firstPart + lastPart;
                            deviceValidationDetails.IsValid = true;
                        }
                    }
                }
            }

            result.IsSuccessful = true;
            result.Data = deviceValidationDetails;
            return result;
        }
    }
}
