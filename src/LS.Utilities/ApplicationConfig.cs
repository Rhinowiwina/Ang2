using System.Configuration;

namespace LS.Utilities {
    public static class ApplicationConfig {
        private static readonly object LockObject = new object();

        private static volatile string _nladApiBaseUrl;
        private static volatile string _puertoricoApiBaseUrl;
        private static volatile string _cgmRESTApiBaseUrl;
        private static volatile string _nladApiVersionNumber;
        private static volatile string _puertoricoApiVersionNumber;
        private static volatile string _mobileImageUploadUsername;
        private static volatile string _mobileImageUploadPassword;
        private static volatile string _webAPIUsername;
        private static volatile string _webAPIPassword;
        private static volatile string _solixWebAPIUsername;
        private static volatile string _solixWebAPIPassword;
        private static volatile string _topUpAppUsername;
        private static volatile string _topUpAppPassword;
        private static volatile string _usersPortalUsername;
        private static volatile string _usersPortalPassword;

        public static string NladApiBaseUrl {
            get {
                if (_nladApiBaseUrl != null) {
                    return _nladApiBaseUrl;
                }
                lock (LockObject) {
                    _nladApiBaseUrl = ConfigurationManager.AppSettings["NladApiBaseUrl"];
                }
                return _nladApiBaseUrl;
            }
        }
        public static string PuertoRicoApiBaseUrl {
            get {
                if (_puertoricoApiBaseUrl != null) {
                    return _puertoricoApiBaseUrl;
                }
                lock (LockObject) {
                    _puertoricoApiBaseUrl = ConfigurationManager.AppSettings["PuertoRicoApiBaseUrl"];
                }
                return _puertoricoApiBaseUrl;
            }
        }

        public static string CGMRESTApiBaseUrl {
            get {
                if (_cgmRESTApiBaseUrl != null) {
                    return _cgmRESTApiBaseUrl;
                }
                lock (LockObject) {
                    _cgmRESTApiBaseUrl = ConfigurationManager.AppSettings["CGMRESTApiBaseUrl"];
                }
                return _cgmRESTApiBaseUrl;
            }
        }

        public static string NladApiVersionNumber {
            get {
                if (_nladApiVersionNumber != null) {
                    return _nladApiVersionNumber;
                }
                lock (LockObject) {
                    _nladApiVersionNumber = ConfigurationManager.AppSettings["NladApiVersionNumber"];
                }
                return _nladApiVersionNumber;
            }
        }
        public static string PuertoRicoApiVersionNumber {
            get {
                if (_puertoricoApiVersionNumber != null) {
                    return _puertoricoApiVersionNumber;
                }
                lock (LockObject) {
                    _puertoricoApiVersionNumber = ConfigurationManager.AppSettings["PuertoRicoApiVersionNumber"];
                }
                return _puertoricoApiVersionNumber;
            }
        }

        public static string MobileImageUploadUsername {
            get {
                if (_mobileImageUploadUsername != null) {
                    return _mobileImageUploadUsername;
                }
                lock (LockObject) {
                    _mobileImageUploadUsername = ConfigurationManager.AppSettings["MobileImageUploadUsername"];
                }
                return _mobileImageUploadUsername;
            }
        }

        public static string MobileImageUploadPassword {
            get {
                if (_mobileImageUploadPassword != null) {
                    return _mobileImageUploadPassword;
                }
                lock (LockObject) {
                    _mobileImageUploadPassword = ConfigurationManager.AppSettings["MobileImageUploadPassword"];
                }
                return _mobileImageUploadPassword;
            }
        }

        public static string WebAPIUsername {
            get {
                if (_webAPIUsername != null) {
                    return _webAPIUsername;
                }
                lock (LockObject) {
                    _webAPIUsername = ConfigurationManager.AppSettings["WebAPIUsername"];
                }
                return _webAPIUsername;
            }
        }

        public static string WebAPIPassword {
            get {
                if (_webAPIPassword != null) {
                    return _webAPIPassword;
                }
                lock (LockObject) {
                    _webAPIPassword = ConfigurationManager.AppSettings["WebAPIPassword"];
                }
                return _webAPIPassword;
            }
        }

        public static string UsersPortalUsername {
            get {
                if (_usersPortalUsername != null) {
                    return _usersPortalUsername;
                }
                lock (LockObject) {
                    _usersPortalUsername = ConfigurationManager.AppSettings["UsersPortalAPIUsername"];
                }
                return _usersPortalUsername;
            }
        }

        public static string UsersPortalPassword {
            get {
                if (_usersPortalPassword != null) {
                    return _usersPortalPassword;
                }
                lock (LockObject) {
                    _usersPortalPassword = ConfigurationManager.AppSettings["UsersPortalAPIPassword"];
                }
                return _usersPortalPassword;
            }
        }

        public static string TopUpAppUsername {
            get {
                if (_topUpAppUsername != null) {
                    return _topUpAppUsername;
                }
                lock (LockObject) {
                    _topUpAppUsername = ConfigurationManager.AppSettings["TopUpAppUsername"];
                }
                return _topUpAppUsername;
            }
        }

        public static string TopUpAppPassword {
            get {
                if (_topUpAppPassword != null) {
                    return _topUpAppPassword;
                }
                lock (LockObject) {
                    _topUpAppPassword = ConfigurationManager.AppSettings["TopUpAppPassword"];
                }
                return _topUpAppPassword;
            }
        }       
    }
}
