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

        //Add methods for converting to BHoM from the specific software types. 
        //Only do this if possible to do without any com-calls or similar to the adapter
        //Example:
        public static Node ToBHoM(this rf.Node node)
        {
            Node bhNode = new Node();
            bhNode.Position.X = node.X;
            bhNode.Position.Y = node.Y;
            bhNode.Position.Z = node.Z;
            bhNode.CustomData.Add(AdapterId, node.No);

            return bhNode;
        }

        /***************************************************/
    }
}
