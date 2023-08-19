namespace DevOpsWatch.BL.Core.Interfaces
{
    public interface ILocalDataSource
    {
        /// <summary>
        /// Gets the credential.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string GetCredential(string key);

        /// <summary>
        /// Saves the credential.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void SaveCredential(string key, string value);
    }
}
