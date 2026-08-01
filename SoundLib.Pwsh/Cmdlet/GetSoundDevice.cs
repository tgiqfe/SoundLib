using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;


namespace SoundLib.Pwsh.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "SoundDevice")]
    internal class GetSoundDevice : PSCmdlet
    {
        #region Command parameters

        [Parameter(Position = 0)]
        public string Name { get; set; }

        [Parameter, Alias("DefaultDeviceOnly")]
        public SwitchParameter OnlyDefaultDevice { get; set; }

        #endregion

        protected override void ProcessRecord()
        {

        }
    }
}
