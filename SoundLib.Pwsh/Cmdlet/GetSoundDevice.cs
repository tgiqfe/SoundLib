using SoundLib.Pwsh.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;


namespace SoundLib.Pwsh.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "SoundDevice")]
    public class GetSoundDevice : PSCmdlet
    {
        #region Command parameters

        [Parameter(Position = 0)]
        public string Name { get; set; }

        [Parameter, Alias("DefaultDeviceOnly")]
        public SwitchParameter OnlyDefaultDevice { get; set; }

        #endregion

        protected override void ProcessRecord()
        {
            SoundDevice[] ret = null;

            using (var enumerator = new AudioDeviceEnumerator())
            {
                var devices = enumerator.EnumerateAudioEndpoints(
                    CoreAudioInterop.DataFlow.Render,
                    CoreAudioInterop.DeviceState.Active);
                if (devices.Count == 0)
                {
                    WriteWarning("No active audio devices found.");
                    return;
                }

                var defaultDevice = enumerator.GetDefaultAudioEndpoint(
                    CoreAudioInterop.DataFlow.Render,
                    CoreAudioInterop.Role.Multimedia);

                if (!string.IsNullOrEmpty(this.Name))
                {
                    //  サウンドデバイスの名前を指定
                    if (this.Name.Contains("*"))
                    {
                        var regex = TextFunctions.WildcardMatch(this.Name);
                        var matchDevices = devices.Where(x => regex.IsMatch(x.FriendlyName));
                        ret = matchDevices.Select(x => new SoundDevice(
                            x.FriendlyName,
                            defaultDevice != null && x.Id == defaultDevice.Id,
                            x.State.ToString(),
                            x.Id,
                            x.DeviceDescription)).ToArray();
                    }
                    else
                    {
                        var dev = devices.FirstOrDefault(x =>
                            x.FriendlyName.Equals(this.Name, StringComparison.OrdinalIgnoreCase));
                        ret = new SoundDevice[]
                        {
                            new SoundDevice(
                                dev.FriendlyName,
                                defaultDevice != null && dev.Id == defaultDevice.Id,
                                dev.State.ToString(),
                                dev.Id,
                                dev.DeviceDescription)
                        };
                    }
                }
                else if (this.OnlyDefaultDevice)
                {
                    //  デフォルトサウンドデバイスを返す
                    if (defaultDevice != null)
                    {
                        ret = new SoundDevice[]
                        {
                            new SoundDevice(
                                defaultDevice.FriendlyName,
                                true,
                                defaultDevice.State.ToString(),
                                defaultDevice.Id,
                                defaultDevice.DeviceDescription)
                        };
                    }
                }
                else
                {
                    //  サウンドデバイス一覧を帰す
                    ret = devices.Select(x => new SoundDevice(
                        x.FriendlyName,
                        defaultDevice != null && x.Id == defaultDevice.Id,
                        x.State.ToString(),
                        x.Id,
                        x.DeviceDescription)).ToArray();
                }

                devices.ForEach(x => x.Dispose());
                defaultDevice.Dispose();
            }

            WriteObject(ret);
        }
    }
}
