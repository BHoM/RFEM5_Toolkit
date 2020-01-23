using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Adapter;
using BH.oM.Base;

namespace BH.oM.Adapter.RFEM
{
    [Description("This Config can be specified in the `ActionConfig` input of any Adapter Action (e.g. Push).")]
    // Note: this will get passed within any CRUD method (see their signature). 
    // In order to access its properties, you will need to cast it to `RFEMActionConfig`.
    public class RFEMSettings : IObject
    {
        public bool Is2DModel { get; set; } = false;
        public bool ZIsUp { get; set; } = true;
    }
}
