using SoundLib.Core;
using SoundLib.Pwsh.Lib;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace SoundLib.Pwsh.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "SoundVolume")]
    public class GetSoundVolume : PSCmdlet
    {
        #region Command parameters

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
            Marshal.ThrowExceptionForHR(_aev.GetMasterVolumeLevelScalar(out float level));
            Marshal.ThrowExceptionForHR(_aev.GetMute(out bool mute));
            WriteObject(new SoundVolume((int)(level * 100), mute));
        }
    }
}
