using System.Runtime.InteropServices;

namespace SoundLib
{
    internal class AudioDevice : IDisposable
    {
        private CoreAudioInterop.IMMDevice? _device;

        public string Id { get; private set; } = string.Empty;
        public string FriendlyName { get; private set; } = string.Empty;
        public string DeviceDescription { get; private set; } = string.Empty;
        public CoreAudioInterop.DeviceState State { get; private set; }

        public AudioDevice(CoreAudioInterop.IMMDevice device)
        {
            _device = device;
            LoadDeviceInfo();
        }

        private void LoadDeviceInfo()
        {
            if (_device == null) return;

            //  Get the device state
            _device.GetId(out string id);
            this.Id = id;

            //  Get the device state
            _device.GetState(out var state);
            this.State = state;

            //  Get the device property store
            int hr = _device.OpenPropertyStore(0, out var propertyStore);
            if (hr == 0 && propertyStore != null)
            {
                try
                {
                    //  Get the friendly name
                    var key = CoreAudioInterop.PKEY_Device_FriendlyName;
                    hr = propertyStore.GetValue(ref key, out var propVariant);
                    if (hr == 0 && propVariant.pwszVal != nint.Zero)
                    {
                        this.FriendlyName = Marshal.PtrToStringUni(propVariant.pwszVal) ?? string.Empty;
                        Marshal.FreeCoTaskMem(propVariant.pwszVal);
                    }

                    //  Get the device description
                    key = CoreAudioInterop.PKEY_Device_DeviceDesc;
                    hr = propertyStore.GetValue(ref key, out propVariant);
                    if (hr == 0 && propVariant.pwszVal != nint.Zero)
                    {
                        this.DeviceDescription = Marshal.PtrToStringUni(propVariant.pwszVal) ?? string.Empty;
                        Marshal.FreeCoTaskMem(propVariant.pwszVal);
                    }
                }
                catch { }
                finally
                {
                    Marshal.ReleaseComObject(propertyStore);
                }
            }
        }

        public AudioFormatInfo? GetAudioFormat()
        {
            if (_device == null) return null;

            try
            {
                var iid = CoreAudioInterop.IID_IAudioClient;
                int hr = _device.Activate(ref iid, 0, nint.Zero, out nint pAudioClient);

                if (hr != 0 || pAudioClient == nint.Zero) return null;

                var audioClient = Marshal.GetObjectForIUnknown(pAudioClient) as CoreAudioInterop.IAudioClient;
                if (audioClient == null)
                {
                    Marshal.Release(pAudioClient);
                    return null;
                }

                try
                {
                    hr = audioClient.GetMixFormat(out nint pFormat);
                    if (hr == 0 && pFormat != nint.Zero)
                    {
                        if (hr == 0 && pFormat != nint.Zero)
                        {
                            try
                            {
                                var waveFormat = Marshal.PtrToStructure<CoreAudioInterop.WaveFormatEx>(pFormat);
                                return new AudioFormatInfo
                                {
                                    SampleRate = waveFormat.nSamplesPerSec,
                                    BitsPerSample = waveFormat.wBitsPerSample,
                                    Channels = waveFormat.nChannels
                                };
                            }
                            catch { }
                            finally
                            {
                                Marshal.FreeCoTaskMem(pFormat);
                            }
                        }
                    }
                }
                catch { }
                finally
                {
                    Marshal.ReleaseComObject(audioClient);
                    Marshal.Release(pAudioClient);
                }
            }
            catch { }
            return null;
        }

        public CoreAudioInterop.IMMDevice? GetNativeDevice() => _device;

        ~AudioDevice()
        {
            Dispose();
        }

        #region Disposable support

        private bool _disposed = false;

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_device != null)
                {
                    Marshal.ReleaseComObject(_device);
                    _device = null;
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
