using SoundLib.Cmd;

var ap = new ArgsParam(args);

switch (ap.SubCommand)
{
    case SubCommand.Device:
        var soundDevice = new SoundDeviceProcess();
        if (ap.SetDefault)
        {
            soundDevice.SetDefaultDevice(ap);
        }
        else
        {
            soundDevice.ListDevices(ap);
        }
        break;
    case SubCommand.Volume:
        var soundVolume = new SoundVolumeProcess();
        if (ap.IsMute != null || !string.IsNullOrEmpty(ap.SetLevel))
        {
            soundVolume.SetVolume(ap);

        }
        else
        {
            soundVolume.GetVolume(ap);
        }
        break;
}


