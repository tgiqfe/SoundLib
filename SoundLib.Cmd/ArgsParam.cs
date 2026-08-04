using SoundLib.Cmd.Lib;

namespace SoundLib.Cmd
{
    internal class ArgsParam
    {
        public SubCommand SubCommand { get; set; }

        public bool SetDefault { get; set; }
        public string Name { get; set; }
        public bool? IsMute { get; set; }
        public string SetLevel { get; set; }

        public ArgsParam(string[] args)
        {
            if (args.Length > 0)
            {
                this.SubCommand = args[0].ToLower() switch
                {
                    "device" => SubCommand.Device,
                    "volume" => SubCommand.Volume,
                    _ => SubCommand.None,
                };
            }

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "/d":
                    case "-d":
                    case "/default":
                    case "--default":
                        this.SetDefault = true;
                        break;
                    case "/n":
                    case "-n":
                    case "/name":
                    case "--name":
                        if (i + 1 < args.Length)
                        {
                            this.Name = args[++i];
                        }
                        break;
                    case "/m":
                    case "-m":
                    case "/mute":
                    case "--mute":
                        if (i + 1 < args.Length)
                        {
                            string text = args[++i];
                            this.IsMute = TextFunctions.IsTrue(text) ? true :
                                TextFunctions.IsFalse(text) ? false :
                                null;
                        }
                        break;
                    case "/v":
                    case "-v":
                    case "/level":
                    case "--level":
                        if (i + 1 < args.Length)
                        {
                            this.SetLevel = args[++i];
                        }
                        break;
                }
            }
        }
    }
}
