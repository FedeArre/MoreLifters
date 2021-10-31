using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifterMod
{
    public class ModSettings
    {
        [JsonProperty("disable default lifter")]
        public bool DisableDefaultLifter { get; set; }

        [JsonProperty("enable ramp model")]
        public bool EnableRampModel { get; set; }

        public ModSettings(bool b1, bool b2)
        {
            DisableDefaultLifter = b1;
            EnableRampModel = b2;
        }
    }

}
