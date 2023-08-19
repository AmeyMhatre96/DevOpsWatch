using DevOpsWatch.BL.Core.Interfaces;
using DevOpsWatch.BL.Core.Models;

namespace DevOpsWatch.BL.Core
{
    public static class Services
    {
        private static ILocalDataSource _localDataSource = null;

        public static ILocalDataSource LocalDataSource
        {
            get
            {
                if (_localDataSource == null)
                {
                    _localDataSource = new Infrastructure.LocalDataSource();
                }

                return _localDataSource;
            }
        }

        private static IDevOpsSource _devOpsSource = null;

        public static IDevOpsSource DevOpsSource
        {
            get
            {
                if (_devOpsSource == null)
                {
                    var settings = Settings.GetSettings();
                    _devOpsSource = new AzureDevOpsSource.AzureDevOpsSource(settings);
                }

                return _devOpsSource;
            }
            set
            {
                _devOpsSource = value;
            }
        }

    }
}
