
using DevOpsWatch.BL.BackgroundTasks;
using DevOpsWatch.BL.Core;

using System;
using System.ServiceModel.Channels;

using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DevOpsWatch.WindowsApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadPreferences();
            //new DevOpsReminderTask().Run(null);
            //new NotificationSchedulerTask().Run(null);
            PromptSchedulingHelper.RegisterTimeBasedSchedulingOnUserActiveTask();
        }

        private void LoadPreferences()
        {
            // Load the preference values
            var setting = Settings.GetSettings();
            // Set the initial values in the form controls
            TimePickerReminderTime.Time = setting.ReminderTime;
            PasswordBoxAccessToken.Password = setting.PAT;
            ShouldShowNewTasks.IsChecked = setting.ShouldShowNewTasks;
            ShouldCloseCompletedTasks.IsChecked = setting.CloseTasksWhenEffortsAreComplete;
        }

        private void SaveAndExit(object sender, RoutedEventArgs e)
        {
            // Get the updated values from the form controls
            TimeSpan timePreference = TimePickerReminderTime.Time;
            string tokenPreference = PasswordBoxAccessToken.Password;
            bool shouldCloseCompletedTasks = ShouldCloseCompletedTasks.IsChecked ?? false;
            bool shouldShowNewTasks = ShouldShowNewTasks.IsChecked ?? false;
            string message = "";

            if (string.IsNullOrEmpty(tokenPreference))
            {
                message = "Saving without your super-secret Personal Access Token? try again, it might work..";
            }

            if (!string.IsNullOrEmpty(message))
            {
                MessageDialog dialog = new MessageDialog(message);
                _ = dialog.ShowAsync();
                return;
            }
            // Save the preference values
            Settings.SaveSettings(timePreference, tokenPreference, shouldCloseCompletedTasks, shouldShowNewTasks);
            // Run scheduler once as the settings have changed
            new NotificationSchedulerTask().Run(null, unregister: true);
            CoreApplication.Exit();
        }

        private void UpdateEffortsNow(object sender, RoutedEventArgs e)
        {
            string tokenPreference = PasswordBoxAccessToken.Password;
            TimeSpan timePreference = TimePickerReminderTime.Time;
            bool shouldCloseCompletedTasks = ShouldCloseCompletedTasks.IsChecked ?? false;
            bool shouldShowNewTasks = ShouldShowNewTasks.IsChecked ?? false;

            string message = "";
            if (string.IsNullOrEmpty(tokenPreference))
            {
                message = "How about sharing that token before we proceed?";
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageDialog dialog = new MessageDialog(message);
                _ = dialog.ShowAsync();
                return;
            }
            Settings.SaveSettings(timePreference, tokenPreference, shouldCloseCompletedTasks, shouldShowNewTasks);
            Frame.Navigate(typeof(UpdateEffort));
        }
    }
}
