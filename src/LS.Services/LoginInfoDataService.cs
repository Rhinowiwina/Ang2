using System.Diagnostics;
using System.Collections.Generic;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System.Linq;
using System.Configuration;

namespace LS.Services {
    public class LoginInfoDataService : BaseDataService<LoginInfo, string> {
        public override BaseRepository<LoginInfo, string> GetDefaultRepository() {
            return new LoginInfoRepository();
        }

        public ServiceProcessingResult<bool> CurrentLoginIsValid(string userId, string sessionid) {
            var processingResult = new ServiceProcessingResult<bool> { IsSuccessful = true };

            var getCurrentLoginResult = new ServiceProcessingResult<List<LoginInfo>>();

            if (ConfigurationSettings.AppSettings["Environment"].ToString() == "DEV" || ConfigurationSettings.AppSettings["Environment"].ToString() == "Staging") {
                getCurrentLoginResult = base.GetAllWhere(li => li.UserId == userId);
            } else {
                getCurrentLoginResult = base.GetAllWhere(li => li.UserId == userId && li.SessionId == sessionid);
            }

            var getCurrentLoginResultOrdered = getCurrentLoginResult.Data.OrderByDescending(x => x.DateModified);

            if (!getCurrentLoginResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = getCurrentLoginResult.Error;
                return processingResult;
            }
            var currentLogin = getCurrentLoginResultOrdered.First();

            processingResult.Data = currentLogin != null && currentLogin.UserIsLoggedIn;

            return processingResult;
        }

        public ServiceProcessingResult<List<LoginInfo>> GetFrom(string userId, string sessionId) {
            return GetAllWhere(li => li.UserId == userId && li.SessionId == sessionId);
        }

        public ServiceProcessingResult<bool> IsUserLoggedInElsewhere(string userId, string sessionId) {
            var processingResult = new ServiceProcessingResult<bool>();

            var loginInfoResult = GetAllWhere(li => li.UserId == userId && li.SessionId != sessionId && li.UserIsLoggedIn);
            if (!loginInfoResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = loginInfoResult.Error;
                return processingResult;
            }

            processingResult.IsSuccessful = true;
            processingResult.Data = loginInfoResult.Data.Count != 0;

            return processingResult;
        }

        public ServiceProcessingResult LogOutUserElsewhere(string userId) {
            var processingResult = new ServiceProcessingResult();

            var getActiveLoginsForUser = GetAllWhere(li => li.UserId == userId && li.UserIsLoggedIn);
            if (!getActiveLoginsForUser.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = getActiveLoginsForUser.Error;
                return processingResult;
            }
            var activeLogins = getActiveLoginsForUser.Data;

            foreach (var activeLogin in activeLogins) {
                activeLogin.UserIsLoggedIn = false;
                var updateResult = Update(activeLogin);
                if (!updateResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = updateResult.Error;
                    return processingResult;
                }
            }

            processingResult.IsSuccessful = true;

            return processingResult;
        }
    }
}
