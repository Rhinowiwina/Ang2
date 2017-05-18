using System.IO;
using Amazon.S3.Model;
using System.Collections.Generic;

namespace LS.Core.Interfaces
{
    public interface IExternalStorageRepository
    {
        ServiceProcessingResult Save(byte[] byteArray, string fileName, string contentType = "application/octet-stream");
        ServiceProcessingResult Save(string fileName, string filePath);
        bool DoesFileExist(string fileName);
        DataAccessResult<string> GeneratePreSignedUrl(string fileName);
        DataAccessResult RenameFile(string sourceFileName, string destFilename);
        DataAccessResult<List<IExternalStorageListDirectory>> GetDirectoryList(string prefix, string delimiter);
        DataAccessResult<Stream> Get(string fileName);
        DataAccessResult<string> GetAsBase64(string fileName);
    }
}