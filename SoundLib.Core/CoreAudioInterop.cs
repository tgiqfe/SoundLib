using System.Runtime.InteropServices;

namespace SoundLib.Core
{
    public class CoreAudioInterop
    {
        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioEndpointVolume
        {
            int f();
            int g();
            int h();
            int i();
            int SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext);
            int j();
            int GetMasterVolumeLevelScalar(out float pfLevel);
            int k();
            int l();
            int m();
            int n();
            int SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, Guid pguidEventContext);
            int GetMute(out bool pbMute);
        }

        //  デバイスの状態
        public enum DeviceState : uint
        {
            Active = 0x00000001,
            Disabled = 0x00000002,
            NotPresent = 0x00000004,
            Unplugged = 0x00000008,
            All = 0x0000000F
        }

        // データフロー方向
        public enum DataFlow
        {
            Render = 0,     // 出力（再生）
            Capture = 1,    // 入力（録音）
            All = 2
        }

        // デバイスロール
        public enum Role
        {
            Console = 0,
            Multimedia = 1,
            Communications = 2
        }

        // PROPERTYKEY構造体
        [StructLayout(LayoutKind.Sequential)]
        public struct PropertyKey
        {
            public Guid fmtid;
            public uint pid;
            public PropertyKey(Guid fmtid, uint pid)
            {
                this.fmtid = fmtid;
                this.pid = pid;
            }
        }

        // デバイスフレンドリ名用のプロパティキー
        public static readonly PropertyKey PKEY_Device_FriendlyName = new PropertyKey(
            new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 14);

        public static readonly PropertyKey PKEY_DeviceInterface_FriendlyName = new PropertyKey(
            new Guid(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22), 2);

        public static readonly PropertyKey PKEY_Device_DeviceDesc = new PropertyKey(
            new Guid(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0), 2);

        // WAVEFORMATEX構造体
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct WaveFormatEx
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }

        // PROPVARIANT構造体（簡易版）
        [StructLayout(LayoutKind.Explicit)]
        public struct PropVariant
        {
            [FieldOffset(0)] public ushort vt;
            [FieldOffset(8)] public nint pwszVal;
        }

        // IMMDeviceEnumerator
        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDeviceEnumerator
        {
            [PreserveSig] int EnumAudioEndpoints(DataFlow dataFlow, DeviceState stateMask, out IMMDeviceCollection? ppDevices);
            [PreserveSig] int GetDefaultAudioEndpoint(DataFlow dataFlow, Role role, out IMMDevice? ppEndpoint);
            [PreserveSig] int GetDevice(string pwstrId, out IMMDevice? ppDevice);
            [PreserveSig] int RegisterEndpointNotificationCallback(nint pClient);
            [PreserveSig] int UnregisterEndpointNotificationCallback(nint pClient);
        }

        // IMMDeviceCollection
        [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDeviceCollection
        {
            [PreserveSig] int GetCount(out uint pcDevices);
            [PreserveSig] int Item(uint nDevice, out IMMDevice? ppDevice);
        }

        // IMMDevice
        [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMMDevice
        {
            [PreserveSig] int Activate(ref Guid iid, uint dwClsCtx, nint pActivationParams, out nint ppInterface);
            [PreserveSig] int OpenPropertyStore(uint stgmAccess, out IPropertyStore? ppProperties);
            [PreserveSig] int GetId([MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);
            [PreserveSig] int GetState(out DeviceState pdwState);
        }

        // IPropertyStore
        [Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyStore
        {
            [PreserveSig] int GetCount(out uint cProps);
            [PreserveSig] int GetAt(uint iProp, out PropertyKey pkey);
            [PreserveSig] int GetValue(ref PropertyKey key, out PropVariant pv);
            [PreserveSig] int SetValue(ref PropertyKey key, ref PropVariant propvar);
            [PreserveSig] int Commit();
        }

        // IAudioClient
        [Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioClient
        {
            [PreserveSig] int Initialize(uint shareMode, uint streamFlags, long hnsBufferDuration, long hnsPeriodicity, nint pFormat, nint audioSessionGuid);
            [PreserveSig] int GetBufferSize(out uint pNumBufferFrames);
            [PreserveSig] int GetStreamLatency(out long phnsLatency);
            [PreserveSig] int GetCurrentPadding(out uint pNumPaddingFrames);
            [PreserveSig] int IsFormatSupported(uint shareMode, nint pFormat, out nint ppClosestMatch);
            [PreserveSig] int GetMixFormat(out nint ppDeviceFormat);
            [PreserveSig] int GetDevicePeriod(out long phnsDefaultDevicePeriod, out long phnsMinimumDevicePeriod);
            [PreserveSig] int Start();
            [PreserveSig] int Stop();
            [PreserveSig] int Reset();
            [PreserveSig] int SetEventHandle(nint eventHandle);
            [PreserveSig] int GetService(ref Guid riid, out nint ppv);
        }

        // MMDeviceEnumerator COM クラス
        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        public class MMDeviceEnumeratorComObject { }

        // PolicyConfig COM インターフェースと実装クラス
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("F8679F50-850A-41CF-9C72-430F290290C8")]
        public interface IPolicyConfig
        {
            [PreserveSig] int GetMixFormat(string pstrDeviceName, nint ppFormat);
            [PreserveSig] int GetDeviceFormat(string pstrDeviceName, bool bDefault, nint ppFormat);
            [PreserveSig] int ResetDeviceFormat(string pstrDeviceName);
            [PreserveSig] int SetDeviceFormat(string pstrDeviceName, nint pEndpointFormat, nint MixFormat);
            [PreserveSig] int GetProcessingPeriod(string pstrDeviceName, bool bDefault, nint pmftDefaultPeriod, nint pmftMinimumPeriod);
            [PreserveSig] int SetProcessingPeriod(string pstrDeviceName, nint pmftPeriod);
            [PreserveSig] int GetShareMode(string pstrDeviceName, nint pMode);
            [PreserveSig] int SetShareMode(string pstrDeviceName, nint mode);
            [PreserveSig] int GetPropertyValue(string pstrDeviceName, bool bFxStore, nint key, nint pv);
            [PreserveSig] int SetPropertyValue(string pstrDeviceName, bool bFxStore, nint key, nint pv);
            [PreserveSig] int SetDefaultEndpoint(string pstrDeviceName, Role role);
            [PreserveSig] int SetEndpointVisibility(string pstrDeviceName, bool bVisible);
        }

        // Windows 8/8.1/10/11用
        [ComImport]
        [Guid("F8679F50-850A-41CF-9C72-430F290290C8")]
        public class PolicyConfig { }

        // Windows Vista/7用
        [ComImport]
        [Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
        public class PolicyConfigVista { }

        // Windows 10/11用
        [ComImport]
        [Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
        public class PolicyConfig10 { }

        // IID定義
        public static readonly Guid IID_IAudioClient = new Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2");
    }
}
