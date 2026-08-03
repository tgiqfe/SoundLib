using SoundLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SoundLib.Pwsh.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "SoundVolume")]
    public class GetSoundVolume : PSCmdlet
    {
        #region Command parameters

        #endregion

        protected override void ProcessRecord()
        {
            CoreAudioInterop.IMMDeviceEnumerator enumerator = new CoreAudioInterop.MMDeviceEnumeratorComObject() as CoreAudioInterop.IMMDeviceEnumerator;
            CoreAudioInterop.IMMDevice dev = null;
            Marshal.ThrowExceptionForHR(enumerator.GetDefaultAudioEndpoint(
                    CoreAudioInterop.DataFlow.Render,
                    CoreAudioInterop.Role.Multimedia,
                    out dev));
            CoreAudioInterop.IAudioEndpointVolume vol = null;
            Guid epvid = typeof(CoreAudioInterop.IAudioEndpointVolume).GUID;
            //Marshal.ThrowExceptionForHR(dev.Activate(ref epvid, 23, 0, out vol));






            CoreAudioInterop.IAudioEndpointVolume Vol = null;



        }

    }
}
