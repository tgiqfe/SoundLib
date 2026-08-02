using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundLib.Cmd
{
    internal class SoundDeviceProcess
    {
        public static void ListDevices()
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

                foreach (var device in devices)
                {
                    Console.WriteLine(device.FriendlyName);
                    Console.WriteLine("  Default     : " + (device.Id == defaultDevice?.Id ? "Yes" : "No"));
                    Console.WriteLine("  State       : " + device.State);
                    Console.WriteLine("  ID          : " + device.Id);
                    Console.WriteLine("  Description : " + device.DeviceDescription);
                }

                devices.ForEach(x => x.Dispose());
                defaultDevice.Dispose();
            }
        }

        public static void SetDefaultDevice(string deviceName)
        {
            /*
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
                var deviceToSet = devices.FirstOrDefault(x => x.FriendlyName.Equals(deviceName, StringComparison.OrdinalIgnoreCase));
                if (deviceToSet == null)
                {
                    Console.WriteLine($"Device '{deviceName}' not found.");
                    return;
                }
                enumerator.SetDefaultAudioEndpoint(deviceToSet.Id, CoreAudioInterop.Role.Multimedia);
                Console.WriteLine($"Device '{deviceName}' set as default.");
            }
            */
        }
    }
}
