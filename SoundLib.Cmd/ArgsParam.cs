using SoundLib.Cmd.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundLib.Cmd
{
    internal class ArgsParam
    {
        public SubCommand SubCommand { get; set; }

        public bool IsList { get; set; }
        public bool SetDefault { get; set; }
        public string Name { get; set; }
        public bool? IsMute { get; set; }
        public int SetLevel { get; set; }
        public int IncreseLevel { get; set; }

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
                    case "/l":
                    case "-l":
                    case "/list":
                    case "--list":
                        this.IsList = true;
                        break;
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
                            this.IsMute = TextFunctions.IsTrue(args[++i]);
                        }
                        break;
                    case "/v":
                    case "-v":
                    case "/volume":
                    case "--volume":
                        if (i + 1 < args.Length)
                        {
                            string levelText = args[++i];
                            if (int.TryParse(levelText, out int volume))
                            {
                                this.SetLevel = volume;
                            }
                            else
                            {
                                if (levelText.StartsWith("+") || levelText.StartsWith("-"))
                                {
                                    if (int.TryParse(levelText, out int increaseVolume))
                                    {
                                        this.IncreseLevel = increaseVolume;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
