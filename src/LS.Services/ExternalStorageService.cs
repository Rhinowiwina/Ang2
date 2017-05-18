using System;
using System.Drawing;
using System.IO;
using LS.Core;
using LS.Core.Interfaces;
using LS.Domain;
using LS.Repositories;
using Amazon.S3.Model;
using System.Collections.Generic;

namespace LS.Services
{
    public class ExternalStorageService
    {
        private readonly IExternalStorageRepository _repository;
        private static readonly string _imageSuffix = ".png";
        private readonly int _maxImageSize;

        public ExternalStorageService(ExternalStorageCredentials credentials)
        {
            if (credentials.System == "Azure") {
                _repository = new ExternalStorageAzureRepository(credentials.AccessKey, credentials.SecretKey, credentials.Path);
            } else {
                _repository = new ExternalStorageAwsRepository(credentials.AccessKey, credentials.SecretKey, credentials.Path);
            }
            
            _maxImageSize = credentials.MaxImageSize;
        }

        public ServiceProcessingResult RenameFile(string sourceFileName, string destFilename) {
            var processingResult = new ServiceProcessingResult { IsSuccessful = true };
            var result = _repository.RenameFile(sourceFileName, destFilename);
            if (result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = result.Error;
            }
            return processingResult;
        }

        public ServiceProcessingResult<List<Core.Interfaces.IExternalStorageListDirectory>> GetDirectoryList(string prefix, string delimiter) {
            var processingResult = new ServiceProcessingResult<List<Core.Interfaces.IExternalStorageListDirectory>> { IsSuccessful = true };
            var result = _repository.GetDirectoryList(prefix, delimiter);
            if (result.IsSuccessful) {
                processingResult.Data = result.Data;
            } else {
                processingResult.IsSuccessful = false;
                processingResult.Error = result.Error;
            }
            return processingResult;
        }

        public ServiceProcessingResult<Stream> Get(string filename) {
            var processingResult = new ServiceProcessingResult<Stream> { IsSuccessful = true };
            var result = _repository.Get(filename);
            if (result.IsSuccessful) {
                processingResult.Data = result.Data;
            } else {
                processingResult.IsSuccessful = false;
                processingResult.Error = result.Error;
            }
            return processingResult;
        }

        public ServiceProcessingResult SaveFile(string fileName, string filePath) {
            var processingResult = new ServiceProcessingResult();
            if (DoesFileExist(fileName)) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("File with this name already exists.","This file name is already in use. Please contact support.", false);
                return processingResult;
            }
 
            return _repository.Save(fileName, filePath);
        }

        public ServiceProcessingResult SaveFile(byte[] file,string fileName) {
            var processingResult = new ServiceProcessingResult();
            if (DoesFileExist(fileName)) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("File with this name already exists.","This file name is already in use. Please contact support.",false);
                return processingResult;
                }
            return Save(file,fileName);
           
            }

        public ServiceProcessingResult SaveImage(byte[] imageByteArray, string fileName, string imageType)
        {
            var processingResult = new ServiceProcessingResult();
            fileName = fileName + "." + imageType;
            if (DoesFileExist(fileName))
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("File with this name already exists.",
                    "This file name is already in use. Please contact support.", false);
                return processingResult;
            }

            var image = GetImageFromByteArray(imageByteArray);

            if (!ImageSizeIsValid(image))
            {
                image = ResizeImage(image);
            }

            var byteArray = ImageToByteArray(image, imageType);

            return Save(byteArray, fileName);
        }

        public ServiceProcessingResult SaveImage(Image image, string fileName, string imageType)
        
            {
            var processingResult = new ServiceProcessingResult { IsSuccessful = true};
            if (DoesFileExist(fileName))
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("File with this name already exists.",
                    "This file name is already in use. Please contact support.", false);
                return processingResult;
            }
            
            if (!ImageSizeIsValid(image))
            {
                image = ResizeImage(image);
            }

            var byteArray = ImageToByteArray(image, imageType);

            return Save(byteArray, fileName);
        }

        public ServiceProcessingResult SaveImageFromBase64EncodedString(string base64EncodedImage, string fileName, string imageType)
        {
            var processingResult = new ServiceProcessingResult { IsSuccessful = true};

            fileName = fileName + _imageSuffix;
            if (DoesFileExist(fileName))
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("File with this name already exists.",
                    "This file name is already in use. Please contact support.", false);
                return processingResult;
            }

            var image = GetAppropriatelySizedImage(base64EncodedImage);

            var byteArray = ImageToByteArray(image, imageType);

            return Save(byteArray, fileName);
        } 

        public ServiceProcessingResult SaveProofImage(string base64EncodedImage, string imageType)
        {
            var fileNameIsValid = false;
            var fileName = string.Empty;

            while (!fileNameIsValid)
            {
                fileName = GenerateFileName() + _imageSuffix;
                fileNameIsValid = !DoesFileExist(fileName);
            }

            var image = GetAppropriatelySizedImage(base64EncodedImage);

            var byteArray = ImageToByteArray(image, imageType);

            return Save(byteArray, fileName);
        }

        public bool DoesFileExist(string fileName)
        {
            return _repository.DoesFileExist(fileName);
        }

        public ServiceProcessingResult Save(byte[] byteArray, string fileName)
        {
            return _repository.Save(byteArray, fileName);
        }

        private static string GenerateFileName()
        {
            return Guid.NewGuid().ToString();
        }

        private static byte[] ImageToByteArray(Image image, string imageType)
        {
            byte[] byteArray;
            using (var stream = new MemoryStream())
            {
                if (imageType == "png") {
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                } else {
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
              
                byteArray = stream.ToArray();
            }

            return byteArray;
        }

        private Image GetAppropriatelySizedImage(string base64EncodedImage)
        {
            var byteArray = Convert.FromBase64String(base64EncodedImage);

            var image = GetImageFromByteArray(byteArray);
            if (!ImageSizeIsValid(image))
            {
                image = ResizeImage(image);
            }

            return image;
        }

        private bool ImageSizeIsValid(Image image)
        {
            return image.Height <= _maxImageSize && image.Width <= _maxImageSize;
        }

        private Image ResizeImage(Image image)
        {
            var height = image.Height;
            var width = image.Width;

            var heightWidthRatio = (double) height / width;

            if (height > width)
            {
                height = _maxImageSize;
                width = (int)(height / heightWidthRatio);
            }
            else
            {
                width = _maxImageSize;
                height = (int)(width * heightWidthRatio);
            }

            var size = new Size(width, height);
            image = new Bitmap(image, size);
            return image;
        }

        private Image GetImageFromByteArray(byte[] byteArray)
        {
            var stream = new MemoryStream(byteArray);

            return Image.FromStream(stream);
        }

        public ServiceProcessingResult<string> GetAsBase64(string filename) {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };
            var result = _repository.GetAsBase64(filename);
            if (result.IsSuccessful) {
                processingResult.Data = result.Data;
            } else {
                processingResult.IsSuccessful = false;
                processingResult.Error = result.Error;
            }
            return processingResult;
        }

        public ServiceProcessingResult<string> GeneratePreSignedUrl(string fileName)
        {
            var processingResult = new ServiceProcessingResult<string> {IsSuccessful = true};
            var result = _repository.GeneratePreSignedUrl(fileName);
            if (result.IsSuccessful)
            {
                processingResult.Data = result.Data;
            }
            else
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = result.Error;
            }
            return processingResult;
        }
    }
}