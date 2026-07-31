using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SoundLib
{
    internal class AudioDeviceEnumerator : IDisposable
    {
        private CoreAudioInterop.IMMDeviceEnumerator? _enumerator;

        public AudioDeviceEnumerator()
        {
            var enumeratorObject = new CoreAudioInterop.MMDeviceEnumeratorComObject();
            _enumerator = enumeratorObject as CoreAudioInterop.IMMDeviceEnumerator;
        }

        public List<AudioDevice> EnumerateAudioEndpoints(CoreAudioInterop.DataFlow dataFlow, CoreAudioInterop.DeviceState stateMask)
        {
            var devices = new List<AudioDevice>();

            if (_enumerator == null) return devices;
            int hr = _enumerator.EnumAudioEndpoints(dataFlow, stateMask, out var collection);
            if (hr != 0 || collection == null) return devices;

            try
            {
                collection.GetCount(out uint count);
                for (uint i = 0; i < count; i++)
                {
                    hr = collection.Item(i, out var device);
                    if (hr == 0 && device != null)
                    {
                        devices.Add(new AudioDevice(device));
                    }
                }
            }
            catch { }
            finally
            {
                Marshal.ReleaseComObject(collection);
            }
            return devices;
        }

        public AudioDevice? GetDefaultAudioEndpoint(CoreAudioInterop.DataFlow dataFlow, CoreAudioInterop.Role role)
        {
            if (_enumerator == null) return null;

            int hr = _enumerator.GetDefaultAudioEndpoint(dataFlow, role, out var device);
            if (hr != 0 || device == null) return null;

            return new AudioDevice(device);
        }

        /// <summary>
        /// Sets the default audio device for the specified data flow and role.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public bool SetDefaultDevice(string deviceId)
        {
            try
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    Console.WriteLine("Need Windows Vista or later.");
                    return false;
                }
                CoreAudioInterop.IPolicyConfig? policyConfig = null;

                if (Environment.OSVersion.Version.Major >= 10)
                {
                    //  Windows 10,11, and later versions
                    policyConfig = new CoreAudioInterop.PolicyConfig10() as CoreAudioInterop.IPolicyConfig;
                }
                else if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 2)
                {
                    //  Windows 8,8.1
                    policyConfig = new CoreAudioInterop.PolicyConfig() as CoreAudioInterop.IPolicyConfig;
                }
                else
                {
                    //  Windows Vista,7
                    policyConfig = new CoreAudioInterop.PolicyConfigVista() as CoreAudioInterop.IPolicyConfig;
                }

                if (policyConfig != null)
                {
                    policyConfig.SetDefaultEndpoint(deviceId, CoreAudioInterop.Role.Console);
                    policyConfig.SetDefaultEndpoint(deviceId, CoreAudioInterop.Role.Multimedia);
                    policyConfig.SetDefaultEndpoint(deviceId, CoreAudioInterop.Role.Communications);
                    if (OperatingSystem.IsWindows())
                    {
                        Marshal.ReleaseComObject(policyConfig);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting default device: {ex.Message}");
            }
            return false;
        }

        ~AudioDeviceEnumerator()
        {
            Dispose();
        }

        #region IDiposable support

        private bool _disposed = false;

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_enumerator != null)
                {
                    Marsal.ReleaseComObject(_enumerator);
                    _enumerator = null;
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);

        #endregion
        }
    }
