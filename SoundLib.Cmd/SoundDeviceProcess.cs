using SoundLib.Cmd.Lib;
using SoundLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundLib.Cmd
{
    internal class SoundDeviceProcess
    {
        public void ListDevices(ArgsParam ap)
        {
            using (var enumerator = new AudioDeviceEnumerator())
            {
                var devices = enumerator.EnumerateAudioEndpoints(
                    CoreAudioInterop.DataFlow.Render,
                    CoreAudioInterop.DeviceState.Active);
                if (devices.Count == 0)
                {
                    Console.WriteLine("No active audio devices found.");
                    return;
                }
                var defaultDevice = enumerator.GetDefaultAudioEndpoint(
                    CoreAudioInterop.DataFlow.Render,
                    CoreAudioInterop.Role.Multimedia);

                if (string.IsNullOrEmpty(ap.Name))
                {
                    //  全件表示
                    devices.ForEach(x => Print(x, x.Id == defaultDevice?.Id));
                }
                else if (ap.Name.Contains("*"))
                {
                    //  名前指定&ワイルドカード対応
                    var regex = TextFunctions.WildcardMatch(ap.Name);
                    var matchDevice = devices.FirstOrDefault(x => regex.IsMatch(x.FriendlyName));
                    if (matchDevice != null)
                    {
                        Print(matchDevice, matchDevice.Id == defaultDevice?.Id);
                    }
                    else
                    {
                        Console.WriteLine($"Device '{ap.Name}' not found.");
                    }
                }
                else
                {
                    //  名前指定のみ
                    var matchDevice = devices.FirstOrDefault(x => x.FriendlyName.Equals(ap.Name, StringComparison.OrdinalIgnoreCase));
                    if (matchDevice != null)
                    {
                        Print(matchDevice, matchDevice.Id == defaultDevice?.Id);
                    }
                    else
                    {
                        Console.WriteLine($"Device '{ap.Name}' not found.");
                    }
                }

                devices.ForEach(x => x.Dispose());
                defaultDevice.Dispose();
            }
        }

        public void SetDefaultDevice(ArgsParam ap)
        {
            using (var enumerator = new AudioDeviceEnumerator())
            {
                var devices = enumerator.EnumerateAudioEndpoints(
                    CoreAudioInterop.DataFlow.Render,
                    CoreAudioInterop.DeviceState.Active);
                if (devices.Count == 0)
                {
                    Console.WriteLine("No active audio devices found.");
                    return;
                }
                var defaultDevice = enumerator.GetDefaultAudioEndpoint(
                    CoreAudioInterop.DataFlow.Render,
                    CoreAudioInterop.Role.Multimedia);

                if (string.IsNullOrEmpty(ap.Name))
                {
                    Console.WriteLine("Please specify a device name to set as default.");
                    return;
                }
                else if (ap.Name.Contains("*"))
                {
                    //  名前指定&ワイルドカード対応
                    var regex = TextFunctions.WildcardMatch(ap.Name);
                    var matchDevice = devices.FirstOrDefault(x => regex.IsMatch(x.FriendlyName));
                    if (matchDevice != null && matchDevice.Id != defaultDevice?.Id)
                    {
                        enumerator.SetDefaultDevice(matchDevice.Id);
                        Console.WriteLine($"Device '{matchDevice.FriendlyName}' set as default.");
                        Print(matchDevice, true);
                    }
                    else
                    {
                        Console.WriteLine($"Device '{ap.Name}' not found or already set as default.");
                    }
                }
                else
                {
                    //  名前指定のみ
                    var matchDevice = devices.FirstOrDefault(x => x.FriendlyName.Equals(ap.Name, StringComparison.OrdinalIgnoreCase));
                    if (matchDevice != null && matchDevice.Id != defaultDevice?.Id)
                    {
                        enumerator.SetDefaultDevice(matchDevice.Id);
                        Console.WriteLine($"Device '{matchDevice.FriendlyName}' set as default.");
                        Print(matchDevice, true);
                    }
                    else
                    {
                        Console.WriteLine($"Device '{ap.Name}' not found or already set as default.");
                    }
                }

                devices.ForEach(x => x.Dispose());
                defaultDevice.Dispose();
            }
        }

        private void Print(AudioDevice device, bool isDefault)
        {
            Console.WriteLine(device.FriendlyName);
            Console.WriteLine("  Default     : " + (isDefault ? "Yes" : "No"));
            Console.WriteLine("  State       : " + device.State);
            Console.WriteLine("  ID          : " + device.Id);
            Console.WriteLine("  Description : " + device.DeviceDescription);
        }
    }
}
