using SoundLib.Cmd;

var ap = new ArgsParam(args);

switch (ap.SubCommand)
{
    case SubCommand.Device:
        if (ap.IsList)
        {
            SoundDeviceProcess.ListDevices();
        }
        else if (ap.SetDefault && !string.IsNullOrEmpty(ap.Name))
        {
            SoundDeviceProcess.SetDefaultDevice(ap.Name);
        }
        break;
    case SubCommand.Volume:
        break;
}


