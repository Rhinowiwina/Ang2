using System;
using System.Text.RegularExpressions;
using LS.Core;
using LS.Core.Interfaces;
namespace LS.Utilities
{

    public class deviceDetails
    {
        public bool IsValid { get; set; }
        public string Type { get; set; }
        public string HEX { get; set; }
        public string DEC { get; set; }
    }
    public class ValidateDevice
    {
        public Boolean IsValidIMEI(string IMEI)
        {
            if (IMEI.Length != 15)
                return false;
            else
            {
                Int32[] PosIMEI = new Int32[15];
                for (int innlop = 0; innlop < 15; innlop++)
                {
                    PosIMEI[innlop] = Convert.ToInt32(IMEI.Substring(innlop, 1));
                    if (innlop % 2 != 0) PosIMEI[innlop] = PosIMEI[innlop] * 2;
                    while (PosIMEI[innlop] > 9) PosIMEI[innlop] = (PosIMEI[innlop] % 10) + (PosIMEI[innlop] / 10);
                }

                Int32 Totalval = 0;
                foreach (Int32 v in PosIMEI) Totalval += v;
                if (0 == Totalval % 10)
                    return true;
                else
                    return false;
            }
        }

        public ServiceProcessingResult<IValidateDevice> ValidateDeviceID(string input)
        {
            input = input.Trim();
            var deviceValidationDetails = new ServiceProcessingResult<IValidateDevice>();
            deviceValidationDetails.Data = null;
            deviceValidationDetails.Data.IsValid = false;
            deviceValidationDetails.Data.Type = "Unknown";
            deviceValidationDetails.Data.HEX = input;
            deviceValidationDetails.Data.DEC = input;

            bool IMEIIsValid = IsValidIMEI(input);

            if (IMEIIsValid)
            {
                deviceValidationDetails.Data.IsValid = true;
                deviceValidationDetails.Data.Type = "IMEI";
            }
            else
            {
                Regex regexSpecChar = new Regex("[^a-zA-Z0-9]+");
                Regex regexLetters = new Regex("[a-zA-Z]+");

                if (!regexSpecChar.IsMatch(input))
                {
                    if (input.Length == 11 && regexLetters.IsMatch(input))
                    {
                        input = input.Substring(0, 8);
                    }

                    if (input.Length == 8 || input.Length == 11)
                    {
                        deviceValidationDetails.Data.Type = "ESN";
                        if (input.Length == 8)
                        {
                            deviceValidationDetails.Data.HEX = input;

                            var FirstPart = Convert.ToInt32(input.Substring(0, 2), 16).ToString("D3");
                            var LastPart = Convert.ToInt32(input.Substring(2), 16).ToString("D8");

                            deviceValidationDetails.Data.DEC = FirstPart + LastPart;

                        }
                        else
                        {
                            deviceValidationDetails.Data.DEC = input;

                            var FirstPart = Convert.ToInt32(input.Substring(0, 3)).ToString("X2");
                            var LastPart = Convert.ToInt32(input.Substring(3)).ToString("X6");

                            deviceValidationDetails.Data.HEX = FirstPart + LastPart;
                        }
                    }
                    else if (input.Length == 14 || input.Length == 18)
                    {
                        deviceValidationDetails.Data.Type = "MEID";

                        if (input.Length == 14)
                        {
                            deviceValidationDetails.Data.HEX = input;

                            var FirstPart = Convert.ToInt64(input.Substring(0, 8), 16).ToString("D10");
                            var LastPart = Convert.ToInt64(input.Substring(8), 16).ToString("D8");

                            deviceValidationDetails.Data.DEC = FirstPart + LastPart;
                        }
                        else
                        {
                            deviceValidationDetails.Data.DEC = input;

                            var FirstPart = Convert.ToInt64(input.Substring(0, 10)).ToString("X8");
                            var LastPart = Convert.ToInt64(input.Substring(10)).ToString("X6");

                            deviceValidationDetails.Data.HEX = FirstPart + LastPart;
                        }
                    }
                }
            }
            return deviceValidationDetails;
        }
    }
}
