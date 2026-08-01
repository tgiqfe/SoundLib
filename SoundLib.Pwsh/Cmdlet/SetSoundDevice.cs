using SoundLib.Pwsh.Lib;
using System.Management.Automation;

namespace SoundLib.Pwsh.Cmdlet
{
    [Cmdlet(VerbsCommon.Set, "SoundDevice")]
    public class SetSoundDevice : PSCmdlet
    {
        [Parameter(Position = 0)]
        public string Name { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public SoundDevice[] Device { get; set; }

        [Parameter]
        public SwitchParameter SetDefault { get; set; }

        protected override void ProcessRecord()
        {
            if (!string.IsNullOrEmpty(this.Name))
            {
                using (var enumerator = new AudioDeviceEnumerator())
                {
                    var devices = enumerator.EnumerateAudioEndpoints(
                        CoreAudioInterop.DataFlow.Render,
                        CoreAudioInterop.DeviceState.Active);
                    if (devices.Count > 0)
                    {
                        var defaultDevice = enumerator.GetDefaultAudioEndpoint(
                            CoreAudioInterop.DataFlow.Render,
                            CoreAudioInterop.Role.Multimedia);

                        if (this.Name.Contains("*"))
                        {
                            //  名前指定 & ワイルドカード指定
                            var regex = TextFunctions.WildcardMatch(this.Name);
                            var matchDevice = devices.FirstOrDefault(x => regex.IsMatch(x.FriendlyName));
                            if (matchDevice != null && matchDevice.Id != defaultDevice.Id)
                            {
                                enumerator.SetDefaultDevice(matchDevice.Id);
                                Thread.Sleep(500);
                                WriteObject(enumerator.GetDefaultAudioEndpoint(
                                    CoreAudioInterop.DataFlow.Render,
                                    CoreAudioInterop.Role.Multimedia));
                            }
                            else
                            {
                                Console.WriteLine("No matching device found for the wildcard pattern.");
                            }
                        }
                        else
                        {
                            //  名前指定のみ
                            var matchDevice = devices.FirstOrDefault(x => x.FriendlyName.Equals(this.Name, StringComparison.OrdinalIgnoreCase));
                            if (matchDevice != null && matchDevice.Id != defaultDevice.Id)
                            {
                                enumerator.SetDefaultDevice(matchDevice.Id);
                                Thread.Sleep(500);
                                WriteObject(enumerator.GetDefaultAudioEndpoint(
                                    CoreAudioInterop.DataFlow.Render,
                                    CoreAudioInterop.Role.Multimedia));
                            }
                            else
                            {
                                Console.WriteLine("No matching device found with the specified name.");
                            }
                        }
                    }
                }
            }
            else if (this.Device?.Length > 0)
            {
                //  Get-SoundDeviceで取得したデバイスから
                var dev = this.Device[0];
                using (var enumerator = new AudioDeviceEnumerator())
                {
                    var defaultDevice = enumerator.GetDefaultAudioEndpoint(
                        CoreAudioInterop.DataFlow.Render,
                        CoreAudioInterop.Role.Multimedia);
                    if (dev.Id != defaultDevice.Id)
                    {
                        enumerator.SetDefaultDevice(dev.Id);
                        Thread.Sleep(500);
                        WriteObject(enumerator.GetDefaultAudioEndpoint(
                            CoreAudioInterop.DataFlow.Render,
                            CoreAudioInterop.Role.Multimedia));
                    }
                }
            }
        }

    }
}
