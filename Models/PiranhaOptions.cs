using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cms_mvc.Models
{
    public class PiranhaOptions
    {
        private string _basePath = "wwwroot/uploads/";
        private string _baseUrl = "~/uploads/";
        private string _databaseType = "file";
        private string _databaseFilename = "piranha.mvcweb.db";
        private string _databasePath = "./";
        private string _mediaStorageType = "file";
        private bool _enableDiscoveryClient = false;
        private bool _enableRedisCache = false;
        private bool _enableSessionCache = false;

        public bool UseDiscoveryServices { get; set; } = false;


        // Current valid types are: sqlserver | mysql | postgres | file
        public string DatabaseType
        {
            get
            {
                string _envDbType = Environment.GetEnvironmentVariable("PIRANHA_DBTYPE");
                if (!string.IsNullOrEmpty(_envDbType))
                {
                    return _envDbType;
                }

                return _databaseType;
            }
            set
            {
                _databaseType = value;
            }
        }

        // Used only for the file database type
        public string DatabaseFilename
        {
            get
            {
                string _envDbName = Environment.GetEnvironmentVariable("PIRANHA_DBNAME");
                if (!string.IsNullOrEmpty(_envDbName))
                {
                    return _envDbName;
                }

                return _databaseFilename;
            }
            set
            {
                _databaseFilename = value;
            }
        }

        // Used only for the file database type
        public string DatabasePath
        {
            get
            {
                string _envDbPath = Environment.GetEnvironmentVariable("PIRANHA_DBPATH");
                if (!string.IsNullOrEmpty(_envDbPath))
                {
                    return _envDbPath;
                }

                return _databasePath;
            }
            set
            {
                _databasePath = value;
            }
        }

        // Target location to upload the portal media
        public string BasePath { 
            get
            {
                string _envPath = Environment.GetEnvironmentVariable("PIRANHA_BASEPATH");
                if (!string.IsNullOrEmpty(_envPath))
                {
                    return _envPath;
                }

                return _basePath;
            }
            set
            {
                _basePath = value;
            }
        }

        // Base url used to access the uploaded portal media
        public string BaseUrl {
            get
            {
                string _envUrl = Environment.GetEnvironmentVariable("PIRANHA_BASEURL");
                if (!string.IsNullOrEmpty(_envUrl))
                {
                    return _envUrl;
                }

                return _baseUrl;
            }
            set {
                _baseUrl = value;
            }
        }

        public string MediaStorageType
        {
            get
            {
                string _envMediaStore = Environment.GetEnvironmentVariable("PIRANHA_MEDIASTORE");
                if (!string.IsNullOrEmpty(_envMediaStore))
                {
                    return _envMediaStore;
                }

                if (string.IsNullOrEmpty(_mediaStorageType))
                    return "file";

                return _mediaStorageType;
            }
            set
            {
                _mediaStorageType = value;
            }
        }

        public bool EnableDiscoveryClient
        {
            get
            {
                bool _envEnableDiscovery = false;
                var _success = bool.TryParse(Environment.GetEnvironmentVariable("PIRANHA_DISCOVERY"), out _envEnableDiscovery);
                if (!_success)
                {
                    return false;
                }
                else
                {
                    _enableDiscoveryClient = _envEnableDiscovery;
                }
                return _enableDiscoveryClient;
            }
            set
            {
                _enableDiscoveryClient = value;
            }
        }

        public bool EnableRedisCache
        {
            get
            {
                bool _envEnableCache = false;
                var _success = bool.TryParse(Environment.GetEnvironmentVariable("PIRANHA_REDISCACHE"), out _envEnableCache);
                if (!_success)
                {
                    return false;
                }
                else
                {
                    _enableRedisCache = _envEnableCache;
                }
                return _enableRedisCache;
            }
            set
            {
                _enableRedisCache = value;
            }
        }

        public bool EnableSessionCache
        {
            get
            {
                bool _envEnableCache = false;
                var _success = bool.TryParse(Environment.GetEnvironmentVariable("PIRANHA_REDISCACHE"), out _envEnableCache);
                if (!_success)
                {
                    return false;
                }
                else
                {
                    _enableSessionCache = _envEnableCache;
                }
                return _enableSessionCache;
            }
            set
            {
                _enableSessionCache = value;
            }
        }


        public bool UseFileStorage
        {
            get
            {
                return (MediaStorageType.ToLower() == "file");
            }
        }

        public string DatabaseFilePath
        {
            get
            {
                return DatabasePath + DatabaseFilename;
            }
        }
    }
}
