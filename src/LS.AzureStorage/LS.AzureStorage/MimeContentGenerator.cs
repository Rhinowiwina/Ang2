using System;

namespace LS.AzureStorage
{
    public static class MimeContentGenerator
    {
        public static string GetMimeTypeFor(string fileFormat)
        {
            FileFormat parseResult;

            var success = Enum.TryParse(fileFormat, true, out parseResult);

            if (!success)
            {
                return "application/ocelot";
            }

            switch (parseResult)
            {
                case FileFormat.AVI:
                    return "video/x-msvideo";
                case FileFormat.FLV:
                    return "video/x-flv";
                case FileFormat.MOV:
                    return "video/quicktime";
                case FileFormat.MPEG4:
                    return "video/mpeg";
                case FileFormat.MPEGPS:
                    return "video/mpeg";
                case FileFormat.MPG:
                    return "video/mpeg";
                case FileFormat.MP4:
                    return "video/mp4";
                case FileFormat.ThreeGPP:
                    return "video/3gpp";
                case FileFormat.WMV:
                    return "video/x-ms-wmv";
                case FileFormat.WebM:
                    return "video/webm";
                default:
                    return "application/ocelot";
            }
        }

        internal static string GetMimeTypeFor(FileFormat fileFormat)
        {
            switch (fileFormat)
            {
                case FileFormat.AVI:
                    return "video/x-msvideo";
                case FileFormat.FLV:
                    return "video/x-flv";
                case FileFormat.MOV:
                    return "video/quicktime";
                case FileFormat.MPEG4:
                    return "video/mpeg";
                case FileFormat.MPEGPS:
                    return "video/mpeg";
                case FileFormat.MPG:
                    return "video/mpeg";
                case FileFormat.MP4:
                    return "video/mp4";
                case FileFormat.ThreeGPP:
                    return "video/3gpp";
                case FileFormat.WMV:
                    return "video/x-ms-wmv";
                case FileFormat.WebM:
                    return "video/webm";
                default:
                    //Todo:figure out if this is right
                    return "application/ocelot";
            }
        }

        internal enum FileFormat
        {
            UNKNOWN = 0,
            MPEG4 = 1,
            AVI = 2,
            WMV = 3,
            MPEGPS = 4,
            FLV = 5,
            ThreeGPP = 6,
            WebM = 7,
            MP4 = 8,
            MPG = 9,
            MOV = 10
        }
    }
}
