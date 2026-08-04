using SoundLib.Core;
using SoundLib.Pwsh.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundLib.Pwsh.Cmdlet
{
    [Cmdlet(VerbsCommon.Set, "SoundVolume")]
    public class SetSoundVolume : PSCmdlet
    {
        #region Command Parameters

        [Parameter(Position = 0)]
        public string Level { get; set; }

        [Parameter(Position = 1)]
        public bool? Mute { get; set; }

        #endregion

        private CoreAudioInterop.IAudioEndpointVolume _aev = null;

        protected override void BeginProcessing()
        {
            CoreAudioInterop.IMMDeviceEnumerator enumerator =
                new CoreAudioInterop.MMDeviceEnumeratorComObject() as CoreAudioInterop.IMMDeviceEnumerator;
            CoreAudioInterop.IMMDevice dev = null;
            Marshal.ThrowExceptionForHR(enumerator.GetDefaultAudioEndpoint(
                    CoreAudioInterop.DataFlow.Render,
                    CoreAudioInterop.Role.Multimedia,
                    out dev));
            Guid epvid = typeof(CoreAudioInterop.IAudioEndpointVolume).GUID;
            nint aevPtr;
            Marshal.ThrowExceptionForHR(dev.Activate(ref epvid, 23, 0, out aevPtr));
            _aev = (CoreAudioInterop.IAudioEndpointVolume)Marshal.GetObjectForIUnknown(aevPtr);
            Marshal.Release(aevPtr);
        }

        protected override void ProcessRecord()
        {
            //  Set Volume level
            if (!string.IsNullOrEmpty(this.Level))
            {
                int levelValue = -1;

                if (this.Level.StartsWith("+") || this.Level.StartsWith("-"))
                {
                    Marshal.ThrowExceptionForHR(_aev.GetMasterVolumeLevelScalar(out float currentLevel));
                    var current = (int)(currentLevel * 100);

                    if (this.Level.Contains("+"))
                    {
                        if (int.TryParse(this.Level.Replace("+", ""), out int delta))
                        {
                            levelValue = current + delta;
                        }
                    }
                    else if (this.Level.Contains("-"))
                    {
                        if (int.TryParse(this.Level.Replace("-", ""), out int delta))
                        {
                            levelValue = current - delta;
                        }
                    }
                }
                else
                {
                    if (int.TryParse(this.Level, out int parsedLevel))
                    {
                        levelValue = parsedLevel;
                    }
                }

                if (levelValue >= 0 && levelValue <= 100)
                {
                    float volumeLevelScalar = levelValue / 100.0f;
                    Marshal.ThrowExceptionForHR(_aev.SetMasterVolumeLevelScalar(volumeLevelScalar, Guid.Empty));
                }
                else
                {
                    WriteError(new ErrorRecord(
                        new ArgumentOutOfRangeException("Level", "Level must be between 0 and 100."),
                        "InvalidLevel",
                        ErrorCategory.InvalidArgument,
                        this.Level));
                }
            }

            //  Set Mute state
            if (this.Mute != null)
            {
                Marshal.ThrowExceptionForHR(_aev.SetMute(this.Mute.Value, Guid.Empty));
            }

            //  Result
            Marshal.ThrowExceptionForHR(_aev.GetMasterVolumeLevelScalar(out float level));
            Marshal.ThrowExceptionForHR(_aev.GetMute(out bool mute));
            WriteObject(new SoundVolume((int)(level * 100), mute));
        }
    }
}
