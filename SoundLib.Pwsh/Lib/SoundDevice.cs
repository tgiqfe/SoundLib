using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundLib.Pwsh.Lib
{
    public class SoundDevice
    {
        public string Name { get; private set; }
        public bool IsDefault { get; private set; }
        public string State { get; private set; }
        public string Id { get; private set; }
        public string Description { get; private set; }

        public SoundDevice(string name, bool isDefault, string state, string id, string description)
        {
            Name = name;
            IsDefault = isDefault;
            State = state;
            Id = id;
            Description = description;
        }
    }
}
