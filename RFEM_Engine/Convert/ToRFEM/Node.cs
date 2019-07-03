using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //Add methods for converting From BHoM to the specific software types, if possible to do without any BHoM calls
        //Example:
        public static rf.Node ToRFEM(this Node node, int nodeId)
        {
            rf.Node rfNode = new rf.Node();
            rfNode.No = nodeId;
            rfNode.X = node.Position.X;
            rfNode.Y = node.Position.Z;
            rfNode.Z = node.Position.Y;
            return rfNode;
        }

        /***************************************************/
    }
}
