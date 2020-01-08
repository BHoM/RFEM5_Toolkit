using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;
using rf = Dlubal.RFEM5;

namespace BH.Engine.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Bar ToBHoM(this rf.Member member, rf.Line line, ISectionProperty sectionProperty)
        {
            rf.Point3D sPt = line.ControlPoints.First();
            rf.Point3D ePt = line.ControlPoints.Last();

            BH.oM.Geometry.Line ln = new oM.Geometry.Line() { Start = new oM.Geometry.Point() { X = sPt.X, Y = sPt.Y, Z = sPt.Z }, End = new oM.Geometry.Point() { X = ePt.X, Y = ePt.Y, Z = ePt.Z } };

            Bar bhBar = BH.Engine.Structure.Create.Bar(ln, sectionProperty, member.Rotation.Angle);

            bhBar.CustomData.Add(AdapterIdName, member.No);

            return bhBar;
        }

        /***************************************************/
    }
}
