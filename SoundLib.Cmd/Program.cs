using SoundLib.Cmd;

var ap = new ArgsParam(args);

switch (ap.SubCommand)
{
    case SubCommand.Device:
        if (ap.IsList)
        {
            SoundDeviceProcess.ListDevices(ap);
        }
        else if (ap.SetDefault)
        {
            SoundDeviceProcess.SetDefaultDevice(ap);
        }
        break;
    case SubCommand.Volume:
        break;
}


