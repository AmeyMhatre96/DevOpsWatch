using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;

namespace DevOpsWatch.WindowsApp
{
    internal static class AppCenterHelper
    {
        internal static void Initialize()
        {
            AppCenter.Start("229b0bc0-385d-4f85-9410-ba234a6da4bd", typeof(Distribute));
            Distribute.CheckForUpdate();
        }
    }
}
