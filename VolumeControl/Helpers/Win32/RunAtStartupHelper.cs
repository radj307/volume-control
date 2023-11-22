using System;
using System.Diagnostics.CodeAnalysis;
using VolumeControl.Log;

namespace VolumeControl.Helpers.Win32
{
    internal static class RunAtStartupHelper
    {
        #region Constructor
        static RunAtStartupHelper() => ExecutablePath = PathFinder.ExecutablePath;
        #endregion Constructor

        #region Fields
        static readonly string ExecutablePath;
        /// <summary>
        /// The name of the value for the app.
        /// </summary>
        const string ValueName = "VolumeControl";
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets whether run at startup is enabled for Volume Control or not.
        /// </summary>
        [DisallowNull] //< setting this to null isn't allowed
        public static bool? IsEnabled
        {
            get => RegistryHelper.IsRunAtStartupEnabled(ValueName, ExecutablePath);
            set
            {
                ArgumentNullException.ThrowIfNull(value, nameof(value));

                if (value.Value)
                { // enable
                    try
                    {
                        if (RegistryHelper.EnableRunAtStartup(ValueName, ExecutablePath))
                            FLog.Info($"[{nameof(RunAtStartupHelper)}] Enabled run at startup.");
                        else
                            FLog.Error($"[{nameof(RunAtStartupHelper)}] Failed to enable run at startup!");
                    }
                    catch (Exception ex)
                    {
                        FLog.Error($"[{nameof(RunAtStartupHelper)}] Failed to enable run at startup due to exception:", ex);
#if DEBUG
                        throw;
#endif
                    }
                }
                else
                { // disable
                    try
                    {
                        if (RegistryHelper.DisableRunAtStartup(ValueName))
                            FLog.Info($"[{nameof(RunAtStartupHelper)}] Disabled run at startup.");
                        else
                            FLog.Error($"[{nameof(RunAtStartupHelper)}] Failed to disable run at startup!");
                    }
                    catch (Exception ex)
                    {
                        FLog.Error($"[{nameof(RunAtStartupHelper)}] Failed to disable run at startup due to exception:", ex);
#if DEBUG
                        throw;
#endif
                    }
                }
            }
        }
        #endregion Properties
    }
}
