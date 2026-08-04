using SoundLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SoundLib.Cmd
{
    internal class SoundVolumeProcess
    {
        private CoreAudioInterop.IAudioEndpointVolume _aev = null;

        public SoundVolumeProcess()
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

        public void GetVolume(ArgsParam ap)
        {
            Marshal.ThrowExceptionForHR(_aev.GetMasterVolumeLevelScalar(out float level));
            Marshal.ThrowExceptionForHR(_aev.GetMute(out bool mute));

            Print((int)(level * 100), mute);
        }

        public void SetVolume(ArgsParam ap)
        {
            //  Set Volume level
            if (!string.IsNullOrEmpty(ap.SetLevel))
            {
                int levelValue = -1;

                if (ap.SetLevel.StartsWith("+") || ap.SetLevel.StartsWith("-"))
                {
                    Marshal.ThrowExceptionForHR(_aev.GetMasterVolumeLevelScalar(out float currentLevel));
                    var current = (int)(currentLevel * 100);

                    if (ap.SetLevel.Contains("+"))
                    {
                        if (int.TryParse(ap.SetLevel.Replace("+", ""), out int delta))
                        {
                            levelValue = current + delta;
                        }
                    }
                    else if (ap.SetLevel.Contains("-"))
                    {
                        if (int.TryParse(ap.SetLevel.Replace("-", ""), out int delta))
                        {
                            levelValue = current - delta;
                        }
                    }
                }
                else
                {
                    if (int.TryParse(ap.SetLevel, out int parsedLevel))
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
                    Console.WriteLine("Level must be between 0 and 100.");
                }
            }

            //  Set Mute state
            if (ap.IsMute != null)
            {
                Marshal.ThrowExceptionForHR(_aev.SetMute(ap.IsMute.Value, Guid.Empty));
            }

            //  Result
            Marshal.ThrowExceptionForHR(_aev.GetMasterVolumeLevelScalar(out float level));
            Marshal.ThrowExceptionForHR(_aev.GetMute(out bool mute));
            Print((int)(level * 100), mute);
        }

        private void Print(int level, bool isMuted)
        {
            Console.WriteLine($"Level : {level}");
            Console.WriteLine($"Mute  : {(isMuted ? "Yes" : "No")}");
        }
    }
}
