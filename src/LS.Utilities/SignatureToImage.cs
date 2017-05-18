
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using Topaz;
using LS.Core;
using System.Windows.Forms;
namespace LS.Utilities {

    /// <summary>
    /// A supplemental class for Signature Pad (https://github.com/thomasjbradley/signature-pad)
    /// that generates an image of the signature's JSON output server-side using C#. Alternately,
    /// you can provide a name and generate an image that resembles a signature from a font.
    /// Similar to Signature to Image PHP (https://github.com/thomasjbradley/signature-to-image)
    /// </summary>
    public class SignatureToImage {

        public ServiceProcessingResult<Image> GetImage(string data, string SigType, bool IsInitials) {
            var processingResult = new ServiceProcessingResult<Image>() { IsSuccessful = true };

            if (SigType == "Tablet") {
                if (IsInitials) {
                    Image sigImage = SigJsonToImage(data, new Size(185, 120));
                    processingResult.Data = sigImage;
                } else {
                    Image sigImage = SigJsonToImage(data, new Size(320, 125));
                    processingResult.Data = sigImage;
                }
                
            } else if (SigType == "Topaz") {
                var result = TopazStrToImage(data);
                if (!result.IsSuccessful) {
                    processingResult.Error = result.Error;
                    processingResult.IsSuccessful = false;
                    return processingResult;
                }
                processingResult.Data = result.Data;

            } else if (SigType == "ePad") {
                processingResult.Error = new ProcessingError("ePad under Contruction", "ePad under contruction.", true, false);
                processingResult.IsSuccessful = false;
                return processingResult;

            } else {
                processingResult.Error = new ProcessingError("Signature type ("+SigType+") not found.", "Signature type (" + SigType + ") not found.", true, false);
                processingResult.IsSuccessful = false;
                return processingResult;
            }

            return processingResult;
        }

        //***TABLET***//
        public Color BackgroundColor { get; set; }
        public Color PenColor { get; set; }
        public int CanvasWidth { get; set; }
        public int CanvasHeight { get; set; }
        public float PenWidth { get; set; }
        public float FontSize { get; set; }
        public string FontName { get; set; }

        private class SignatureLine {
            public int lx { get; set; }
            public int ly { get; set; }
            public int mx { get; set; }
            public int my { get; set; }
        }

        //* Author:      Curtis Herbert (me@forgottenexpanse.com)
        //* License:     BSD License
        //* Version:     2.0 (2012-02-11)
        //* Contributor: Justin Stolle (justin@justinstolle.com)
        //*/
        /// <summary>
        /// Gets a new signature generator with the default options.
        /// </summary>
        public SignatureToImage() {
            // Default values
            BackgroundColor = Color.Transparent;
            PenColor = Color.FromArgb(0, 0, 0);
            CanvasWidth = 320;
            CanvasHeight = 125;
            PenWidth = 2;
            FontSize = 18;
            FontName = "Journal";
        }

        //public Bitmap SigJsonToImage(string json) {
        //    return SigJsonToImage(json, new Size(CanvasWidth, CanvasHeight));
        //}

        public Bitmap SigJsonToImage(string json, Size size) {
            var signatureImage = GetBlankCanvas(size.Width, size.Height);
            if (!string.IsNullOrWhiteSpace(json)) {
                using (var signatureGraphic = Graphics.FromImage(signatureImage)) {
                    signatureGraphic.SmoothingMode = SmoothingMode.AntiAlias;
                    var pen = new Pen(PenColor, PenWidth);
                    var serializer = new JavaScriptSerializer();
                    // Next line may throw System.ArgumentException if the string
                    // is an invalid json primitive for the SignatureLine structure
                    var lines = serializer.Deserialize<List<SignatureLine>>(json);
                    foreach (var line in lines) {
                        signatureGraphic.DrawLine(pen, line.lx, line.ly, line.mx, line.my);
                    }
                }
            }
            return (Bitmap)((signatureImage.Width == CanvasWidth && signatureImage.Height == CanvasHeight) ? signatureImage : ResizeImage(signatureImage, size));
        }

        private Bitmap GetBlankCanvas(int width, int height ) {
            var blankImage = new Bitmap(width, height);
            blankImage.MakeTransparent();
            using (var signatureGraphic = Graphics.FromImage(blankImage)) {
                signatureGraphic.Clear(BackgroundColor);
            }
            return blankImage;
        }

        private Image ResizeImage(Image img, Size size) {
            int srcWidth = img.Width;
            int srcHeight = img.Height;

            float percent = 0;
            float percWidth = 0;
            float percHeight = 0;

            percWidth = ((float)size.Width / (float)srcWidth);
            percHeight = ((float)size.Height / (float)srcHeight);
            percent = (percHeight < percWidth) ? percHeight : percWidth;

            int destWidth = (int)(srcWidth * percent);
            int destHeight = (int)(srcHeight * percent);

            Bitmap bmp = new Bitmap(destWidth, destHeight);

            Graphics graphic = Graphics.FromImage((Image)bmp);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(img, 0, 0, destWidth, destHeight);
            graphic.Dispose();

            return (Image)bmp;
        }

        //***TOPAZ***//
        public ServiceProcessingResult<Image> TopazStrToImage(string data) {
            var processingResult = new ServiceProcessingResult<Image>() { IsSuccessful = true };
            Image sigImage;
            try {
                SigPlusNET sigPlusNet = new SigPlusNET();
                sigPlusNet.SetSigCompressionMode(1);
                sigPlusNet.SetSigString(data);
                if (sigPlusNet.NumberOfTabletPoints() > 0) {
                    sigPlusNet.SetImageFileFormat(1);
                    sigPlusNet.SetImageXSize(500);
                    sigPlusNet.SetImageYSize(165);
                    sigPlusNet.SetImagePenWidth(8);
                    sigPlusNet.SetJustifyMode(5);
                    //string root = HttpContext.Current.Server.MapPath("/app/main/support/SigTest");
                    //string path = root + "\\signature.png";
                    //sigPlusNet.GetSigImage().Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    //string jpgpath = root + "\\signature.png";
                    //sigPlusNet.GetSigImage().Save(jpgpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    sigImage = sigPlusNet.GetSigImage();
                    processingResult.Data = sigImage;
                } else {
                    processingResult.Error = new ProcessingError("No data uploaded for signature image", "No data uploaded for signature image", true, false);
                    processingResult.IsSuccessful = false;
                    return processingResult;
                }
            } catch (Exception ex) {
                processingResult.Error = new ProcessingError("Error generating Topaz signature image", "Error generating Topaz signature image", true, false);
                processingResult.IsSuccessful = false;
                return processingResult;
            }

            return processingResult;
        }
    }
}