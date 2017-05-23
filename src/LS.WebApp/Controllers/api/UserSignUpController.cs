using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LS.Core;
using System;
using LS.Services;
using LS.ApiBindingModels;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;
using System.IO;
using LS.Utilities;
using Ionic.Zip;
using Newtonsoft.Json;
using Exceptionless;
using Exceptionless.Models;
using System.Configuration;
using System.Drawing;
using LS.WebApp.CustomAttributes;
namespace LS.WebApp.Controllers.api {
    public class ImageForUpload {
        public Byte[] Image { get; set; }
        public string Filename { get; set; }
    }


    [SingleSessionAuthorize]
    [RoutePrefix("api/userSignUp")]
    public class UserSignUpController : BaseApiController {
      
        [Route("enroll")]
        public async Task<IHttpActionResult> EnrollUser() {
            var processingResult = new ServiceProcessingResult<string>() { IsSuccessful = true };
            var userID = Utils.RandomGuidString();

            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/App_Data"))) {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/App_Data"));
            }
    
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            var data = await Request.Content.ReadAsMultipartAsync(provider);


            var model = new UserSignUpsView();

            // Show all the key-value pairs.
            foreach (var key in provider.FormData.AllKeys) {
                foreach (var val in provider.FormData.GetValues(key)) {
                    //model = JsonConvert.DeserializeObject<UserSignUpsView>(val);
                    //model.GetType().GetProperty(key).SetValue(model,val,null);
                    Utils.SetObjectProperty(key,val,model);
                }
            }
            //directory to save in
            ZipFile zip = new ZipFile();
            var fullFilePath = root + "/" + userID;
            Directory.CreateDirectory(fullFilePath);

            var Images = new List<ImageForUpload>();
            //GovId
         if (!String.IsNullOrEmpty(model.Gov64) && model.Gov64!="null"){
                var Gov64 = new ImageForUpload();
                Gov64.Image = Convert.FromBase64String(model.Gov64);
                Gov64.Filename = "govid64_" + userID +".jpg";
                model.GovernmentDocFilename = Gov64.Filename;
                Images.Add(Gov64);
                try
                    {
                    using (var ms = new MemoryStream(Gov64.Image,0,Gov64.Image.Length)) {
                        Image image = Image.FromStream(ms,true);
                        image.Save(Path.Combine(fullFilePath,Gov64.Filename),System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }catch(Exception ex)
                    {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error reading Government Id image file for onboarding user.","Error reading Government Id image file for onboarding user",true,false);
                    ex.ToExceptionless()
                   .SetMessage("Error reading Government Id image file for onboarding user.")
                   .MarkAsCritical()
                   .Submit();
                    //Logger.Error("Error reading Government Id image file for onboarding user.",ex);
                    return Ok(processingResult);

                    }
                //Add to zip file
                zip.AddFile(Path.Combine(fullFilePath,Gov64.Filename),"/");
           }
            //Disclosure Form
            if (!String.IsNullOrEmpty(model.Disclosure64 ) && model.Disclosure64 != "null")
                {
                var DiscloseFrm = new ImageForUpload();
                DiscloseFrm.Image = Convert.FromBase64String(model.Disclosure64);
                DiscloseFrm.Filename = "disc64_" + userID + ".jpg";
                model.DisclosureFilename = DiscloseFrm.Filename;
                Images.Add(DiscloseFrm);
                try
                    {
                    using (var ms = new MemoryStream(DiscloseFrm.Image,0,DiscloseFrm.Image.Length))
                        {
                        Image image = Image.FromStream(ms,true);
                        image.Save(Path.Combine(fullFilePath,DiscloseFrm.Filename),System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                catch (Exception ex)
                    {
                   ex.ToExceptionless()
                  .SetMessage("Error reading Disclosure Form image file for onboarding user.")
                  .MarkAsCritical()
                  .Submit();
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error reading Disclosure Form image file for onboarding user.","Error reading Disclosure Form image file for onboarding user",true,false);
                    //Logger.Error("Error reading Disclosure Form image file for onboarding user.",ex);
                    return Ok(processingResult);

                    }
                //Add to zip file
                zip.AddFile(Path.Combine(fullFilePath,DiscloseFrm.Filename),"/");
                }

            // Authorization Form
            if (!String.IsNullOrEmpty(model.Authorization64) && model.Authorization64 !="null")
                {
                var AuthFrm = new ImageForUpload();
                AuthFrm.Image = Convert.FromBase64String(model.Authorization64);
                AuthFrm.Filename = "auth64_" + userID + ".jpg";
                model.AuthorizationFilename = AuthFrm.Filename;
                Images.Add(AuthFrm);
                try
                    {
                    using (var ms = new MemoryStream(AuthFrm.Image,0,AuthFrm.Image.Length))
                        {
                        Image image = Image.FromStream(ms,true);
                        image.Save(Path.Combine(fullFilePath,AuthFrm.Filename),System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                catch (Exception ex)
                    {
                    ex.ToExceptionless()
                    .SetMessage("Error reading Authorization Form  image file for onboarding user.")
                    .MarkAsCritical()
                    .Submit();
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error reading Authorization Form image file for onboarding user.","Error reading Authorization Form  image file for onboarding user",true,false);
                    //Logger.Error("Error reading Authorization Form  image file for onboarding user.",ex);
                    return Ok(processingResult);

                    }

                //Add to zip file
                zip.AddFile(Path.Combine(fullFilePath,AuthFrm.Filename),"/");
                }

            //Training Certificate
            if (!String.IsNullOrEmpty(model.Training64) && model.Training64 !="null")
                {
                var TrainingFrm = new ImageForUpload();
                TrainingFrm.Image = Convert.FromBase64String(model.Training64);
                TrainingFrm.Filename = "train64_" + userID + ".jpg";
                model.TrainingCertFilename = TrainingFrm.Filename;
                Images.Add(TrainingFrm);
                try
                    {
                    using (var ms = new MemoryStream(TrainingFrm.Image,0,TrainingFrm.Image.Length))
                        {
                        Image image = Image.FromStream(ms,true);
                        image.Save(Path.Combine(fullFilePath,TrainingFrm.Filename),System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                catch (Exception ex)
                    {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error reading Training Certificate Form image file for onboarding user.","Error reading Training Certificate Form  image file for onboarding user",true,false);
                    ex.ToExceptionless()
                    .SetMessage("Error reading Training Certificate Form  image file for onboarding user.")
                    .MarkAsCritical()
                    .Submit();
                    //Logger.Error("Error reading Training Certificate Form  image file for onboarding user.",ex);
                    return Ok(processingResult);

                    }
                //Add to zip file
                zip.AddFile(Path.Combine(fullFilePath,TrainingFrm.Filename),"/");
                }


            try {
                foreach (MultipartFileData fileData in provider.FileData) {
                    string fileField = fileData.Headers.ContentDisposition.FileName;
                    //strip slashes off
                    if (fileField.StartsWith("\"") && fileField.EndsWith("\""))
                        { fileField = fileField.Trim('"'); }
                    if (fileField.Contains(@"/") || fileField.Contains(@"\"))
                        { fileField = Path.GetFileName(fileField); }
                    string myFilename = "";
                    switch (fileField) {
                        case "govId": {
                            myFilename = "govid_" + model.GovernmentDocFilename;
                            model.GovernmentDocFilename = myFilename;
                            break;
                        }
                        case "disclosure": {
                            myFilename = "disc_" + model.DisclosureFilename;
                            model.DisclosureFilename = myFilename;
                            break;
                        }
                        case "authorization": {
                            myFilename = "auth_" + model.AuthorizationFilename;
                            model.AuthorizationFilename = myFilename;
                            break;
                        }
                        case "training": {
                            myFilename = "train_" + model.TrainingCertFilename;
                            model.TrainingCertFilename = myFilename;
                            break;
                        }
                    }
                    
                    var imageObj = new ImageForUpload { Image = File.ReadAllBytes(fileData.LocalFileName),Filename = myFilename };

                    //Add to image list for S3 upload later
                    Images.Add(imageObj);

                    //move to folder... also moves the appdata file
                    File.Move(fileData.LocalFileName, Path.Combine(fullFilePath,myFilename));
                  
                    //Add to zip file
                    zip.AddFile(Path.Combine(fullFilePath,myFilename), "/");
                }

                zip.TempFileFolder = Path.GetTempPath();

                //Save zip file
                zip.Save(root + "\\" + userID + ".zip");
                zip.Dispose();

                //Delete folder with images since we zipped them
                var dir = new DirectoryInfo(fullFilePath);
                dir.Delete(true);


                //Add to email attachment list
                var attachmentList = new List<string>();
                attachmentList.Add(root + "\\" + userID + ".zip");


                var body = @"
                <strong>Subcontractor: </strong>" + model.Subcontractor + "<br>" +
                    "<strong>First Name: </strong>" + model.FirstName + "<br>" +
                    "<strong>Last Name: </strong>" + model.LastName + "<br> " +
                    "<strong>DOB: </strong>" + model.DOB + " <br> " +
                    "<strong>SSN: </strong>" + model.Ssn + " <br> " +
                    "<strong>Address1: </strong>" + model.Address1 + " <br> " +
                    "<strong>Address2: </strong>" + model.Address2 + " <br> " +
                    "<strong>City: </strong>" + model.City + " <br> " +
                    "<strong>State: </strong>" + model.State + " <br> " +
                    "<strong>Zip: </strong>" + model.Zipcode + " <br> " +
                    "<strong>Phone: </strong>" + model.PhoneNumber + " <br> " +
                    "<strong>Email: </strong>" + model.Email + " <br> " +
                    "<strong>Background Certificate ID:</strong>" + model.BackGroundCertificateID + "<br>" +
                    "<strong>Drug Screen Certificate ID: </strong>" + model.DrugScreenCertificateID + " <br> " +
                    "<strong>Government ID Filename: </strong>" + model.GovernmentDocFilename + " <br> " +
                    "<strong>Disclosure Form Filename: </strong>" + model.DisclosureFilename + " <br> " +
                    "<strong>Authorization Form Filename: </strong>" + model.AuthorizationFilename + " <br> " +
                    "<strong>Training Certificate Filename: </strong>" + model.TrainingCertFilename + " <br> ";

                var emailHelper = new EmailHelper();
                
                var sendEmail = emailHelper.SendEmail("New User Signup", ConfigurationManager.AppSettings["OnboardingEmail"], "", body, attachmentList);

                var sqlQuery = new SQLQuery();

                SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@Id", userID),
                    new SqlParameter("@Subcontractor",model.Subcontractor),
                    new SqlParameter("@FirstName",model.FirstName),
                    new SqlParameter("@LastName",model.LastName),
                    new SqlParameter("@Dob",model.DOB),
                    new SqlParameter("@Ssn",model.Ssn),
                    new SqlParameter("@Address1",model.Address1),
                    new SqlParameter("@Address2",model.Address2==null?"":model.Address2),
                    new SqlParameter("@City",model.City),
                    new SqlParameter("@State",model.State),
                    new SqlParameter("@Zipcode",model.Zipcode),
                    new SqlParameter("@PhoneNumber",model.PhoneNumber),
                    new SqlParameter("@Email",model.Email),
                    new SqlParameter("@BackGroundCertificateID",model.BackGroundCertificateID),
                    new SqlParameter("@DrugScreenCertificateID",model.DrugScreenCertificateID==null?"":model.DrugScreenCertificateID),
                    new SqlParameter("@GovernmentDocFilename",model.GovernmentDocFilename),
                    new SqlParameter("@DisclosureFilename",model.DisclosureFilename),
                    new SqlParameter("@AuthorizationFilename",model.AuthorizationFilename),
                    new SqlParameter("@TrainingCertFilename",model.TrainingCertFilename)
                };
                var strQuery = "INSERT INTO [dbo].[UserSignUps]([Id],[Subcontractor],[FirstName],[LastName],[DOB],[Ssn],[Address1],[Address2],[City],[State],[Zipcode],[PhoneNumber],[Email],[BackGroundCertificateID],[DrugScreenCertificateID],[GovernmentDocFilename],[DisclosureFilename],[AuthorizationFileName],[TrainingCertFileName],[DateCreated],[DateModified],[IsDeleted])  VALUES (@Id,@Subcontractor,@FirstName,@LastName,@DOB,@Ssn,@Address1,@Address2,@City,@State,@Zipcode,@PhoneNumber,@Email,@BackGroundCertificateID,@DrugScreenCertificateID,@GovernmentDocFilename,@DisclosureFilename,@AuthorizationFileName,@TrainingCertFileName,GETDATE(),GETDATE(),0)";
                var userResult = await sqlQuery.ExecuteNonQueryAsync(CommandType.Text, strQuery, parameters);
                if (!userResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Failed to insert user sign up information.", "Failed to insert user sign up information", true, false);
                    return Ok(processingResult);
                }
                if (userResult.Data != 1) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Failed to insert user sign up information.", "Failed to insert user sign up information", true, false);
                    return Ok(processingResult);
                }

                ////Add to S3

                var fileDataService = new FileDataService();
                var fileUploadResult = await fileDataService.UploadFileAsync(userID + ".zip", root, "Storage", "65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c");
                if (!fileUploadResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = fileUploadResult.Error;
                    
                    return Ok(processingResult);
                }

                //Delete Zip File
                File.Delete(root + "\\" + userID + ".zip");


            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error generating zip file for onboarding user.", "Error generating zip file for onboarding user", true, false);
                ex.ToExceptionless()
                  .SetMessage("Error generating zip file for onboarding users.")
                  .MarkAsCritical()
                  .Submit();
                
                return Ok(processingResult);
            }



            processingResult.IsSuccessful = true;
            processingResult.Data = "Successfull";
            return Ok(processingResult);

        }
        public Image ConvertToImage(string base64String) {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes,0,imageBytes.Length))
                {
                Image image = Image.FromStream(ms,true);
                return image;
                }

            }
       
        }
}

