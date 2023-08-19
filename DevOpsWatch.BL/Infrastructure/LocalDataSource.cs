using DevOpsWatch.BL.Core.Interfaces;

using System;

using Windows.Security.Credentials;

namespace DevOpsWatch.BL.Infrastructure
{
    internal class LocalDataSource : ILocalDataSource
    {
        /// <summary>
        /// The credential resource.
        /// </summary>
        private const string CredentialResource = "DevOpsRemainder.Credentials";

        private readonly PasswordVault _vault;

        public LocalDataSource()
        {
            _vault = new PasswordVault();
        }

        public string GetCredential(string key)
        {
            try
            {
                var credential = _vault.Retrieve(CredentialResource, key);
                return credential.Password;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void SaveCredential(string key, string value)
        {
            var credential = new PasswordCredential(CredentialResource, key, value);
            _vault.Add(credential);
        }
    }
}
