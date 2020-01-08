using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Adapter;

namespace BH.oM.Adapter.RFEM
{
    public class RFEMConfig : ActionConfig
    {
        public bool Replace {get;set;}
        public bool CloneBeforePush { get; set; } = false; //TODO: check if this was made obsolete in previous adapterRefactoring
        public bool Is2DModel { get; set; } = false;
        public bool ZIsUp { get; set; } = true;
    }
}
