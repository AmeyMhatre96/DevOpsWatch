using System;

namespace DevOpsWatch.BL.Core
{
    /// <summary>
    /// The settings.
    /// </summary>
    public sealed class Settings
    {
        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        public string Organization { get; set; } = "<devops-org>";

        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        public string Project { get; set; } = "<devops-project>";

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the p a t.
        /// </summary>
        public string PAT { get; set; }

        /// <summary>
        /// Gets or sets the reminder time.
        /// </summary>
        public TimeSpan ReminderTime { get; set; }

        public bool CloseTasksWhenEffortsAreComplete { get; set; }
        public bool ShouldShowNewTasks { get; set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <returns>A Settings.</returns>
        public static Settings GetSettings()
        {
            var settings = new Settings();
            var timePref = Services.LocalDataSource.GetCredential(CredentialKeys.TimePreference);
            TimeSpan timePreference = string.IsNullOrEmpty(timePref) ? new TimeSpan(17, 30, 0) : TimeSpan.Parse(timePref);
            settings.ReminderTime = timePreference;
            string tokenPreference = Services.LocalDataSource.GetCredential(CredentialKeys.PAT) ?? "";
            string shouldCloseTasks = Services.LocalDataSource.GetCredential(CredentialKeys.CloseTasksWhenEffortsAreComplete) ?? "";
            string shouldShowNewTasks = Services.LocalDataSource.GetCredential(CredentialKeys.ShouldShowNewTasks) ?? "";
            if (string.IsNullOrEmpty(shouldCloseTasks))
            {
                settings.CloseTasksWhenEffortsAreComplete = true;
            }
            else
            {
                settings.CloseTasksWhenEffortsAreComplete = bool.Parse(shouldCloseTasks);
            }
            if (string.IsNullOrEmpty(shouldShowNewTasks))
            {
                settings.ShouldShowNewTasks = true;
            }
            else
            {
                settings.ShouldShowNewTasks = bool.Parse(shouldShowNewTasks);
            }
            settings.PAT = tokenPreference;

            return settings;
        }

        public static void SaveSettings(TimeSpan timePreference,  string tokenPreference, bool shouldCloseCompletedTasks, bool shouldShowNewTasks)
        {
            Services.LocalDataSource.SaveCredential(CredentialKeys.TimePreference, timePreference.ToString());
            if (Services.LocalDataSource.GetCredential(CredentialKeys.PAT) != tokenPreference)
            {
                // reset instance.
                Services.DevOpsSource = null;
            }
            Services.LocalDataSource.SaveCredential(CredentialKeys.PAT, tokenPreference);
            Services.LocalDataSource.SaveCredential(CredentialKeys.CloseTasksWhenEffortsAreComplete, shouldCloseCompletedTasks.ToString());
            Services.LocalDataSource.SaveCredential(CredentialKeys.ShouldShowNewTasks, shouldShowNewTasks.ToString());
        }
    }

    public static class CredentialKeys
    {
        internal static readonly string TimePreference = "DevOpsWach.TimePreference";
        internal static readonly string PAT = "DevOpsWach.PAT";
        internal static readonly string CloseTasksWhenEffortsAreComplete = "DevOpsWach.CloseTasksWhenEffortsAreComplete";
        internal static readonly string ShouldShowNewTasks = "DevOpsWach.ShouldShowNewTasks";
    }
}