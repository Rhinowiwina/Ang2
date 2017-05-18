using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Domain;
using Exceptionless;
using Exceptionless.Models;

namespace LS.Services {
    public class ErrorBuilder {

        //var error = new ErrorBuilder(new Exception("This is the message"));
        //error.AddTags("CustomTag");
        //        error.AddTags("JarrettTesting");
        //        error.AddObject("LoggedInUserObject", LoggedInUser);
        //        error.Submit();

        protected Error newError { get; set; }
        public ErrorBuilder() {
            newError = new Error {
                Message = "",
                Type = KnownTypes.Error,
                Objects = new DataDictionary(),
                Tags = new TagSet()           
            };
        }

        public void SetMessage(string message) {
            newError.Message = message;
        }

        public void AddObject(string name, object objectToAdd) {
            newError.Objects.Add(name, objectToAdd);
        }

        public void AddTags(params string[] tags) {
            foreach (var tag in tags) {
                newError.Tags.Add(tag);
            }
        }

        public void MarkAsCritical() {
            newError.Tags.Add(KnownTags.Critical);
        }

        public void SetType(string Type) {
            newError.Type = Type;
        }

        public void Submit() {
            ExceptionlessClient.Default.SubmitEvent(
                new Event {
                    Message = newError.Message,
                    Tags = newError.Tags,
                    Type = newError.Type,
                    Data = newError.Objects
                }
            );
        }
    }

    public static class KnownTags {
        public const string Critical = "Critical";
        public const string Internal = "Internal";
    }
    public static class KnownTypes {
        public const string Error = "error";
        public const string FeatureUsage = "usage";
        public const string Log = "log";
        public const string NotFound = "404";
        public const string Session = "session";
    }
}
