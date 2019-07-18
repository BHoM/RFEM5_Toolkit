using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.Engine.RFEM;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        public static rf.Line RFEMLine(Bar bar, int lineId)
        {
            rf.Line centreLine = new rf.Line();
            centreLine.No = lineId;
            int startNodeId = System.Convert.ToInt32(bar.StartNode.CustomData[Convert.AdapterId]);
            int endNodeId = System.Convert.ToInt32(bar.EndNode.CustomData[Convert.AdapterId]);
            centreLine.NodeList = String.Join(",", new int[] { startNodeId, endNodeId });
            centreLine.Type = rf.LineType.PolylineType;

            return centreLine;
        }

        /***************************************************/
    }
}
