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

        public static Node ToBHoM(this rf.Node node)
        {
            Node bhNode = BH.Engine.Structure.Create.Node(new oM.Geometry.Point() { X = node.X, Y = node.Y, Z = node.Z });
            bhNode.CustomData.Add(AdapterIdName, node.No);

            return bhNode;
        }

        /***************************************************/
    }
}
