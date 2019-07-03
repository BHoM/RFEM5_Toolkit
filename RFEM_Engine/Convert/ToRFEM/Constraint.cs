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
        public static rf.NodalSupport ToRFEM(this Constraint6DOF constraint, int constraintId)
        {
            rf.NodalSupport rfConstraint = new rf.NodalSupport();
            rfConstraint.No = constraintId;
            rfConstraint.NodeList = "1";//<-- temp just for testi´ng

            //Translation - RFEM unit is N/m
            if (constraint.TranslationX == DOFType.Free)
                rfConstraint.SupportConstantX = 0;
            else if (constraint.TranslationX == DOFType.Fixed)
                rfConstraint.SupportConstantX = -1;
            else
                rfConstraint.SupportConstantX = constraint.TranslationalStiffnessX;

            if (constraint.TranslationY == DOFType.Free)
                rfConstraint.SupportConstantY = 0;
            else if (constraint.TranslationY == DOFType.Fixed)
                rfConstraint.SupportConstantY = -1;
            else
                rfConstraint.SupportConstantY = constraint.TranslationalStiffnessY;

            if (constraint.TranslationZ == DOFType.Free)
                rfConstraint.SupportConstantZ = 0;
            else if (constraint.TranslationZ == DOFType.Fixed)
                rfConstraint.SupportConstantZ = -1;
            else
                rfConstraint.SupportConstantZ = constraint.TranslationalStiffnessZ;

            //Rotation - RFEM unit is Nm/Rad
            if (constraint.RotationX == DOFType.Free)
                rfConstraint.RestraintConstantX = 0;
            else if (constraint.RotationX == DOFType.Fixed)
                rfConstraint.RestraintConstantX = -1;
            else
                rfConstraint.RestraintConstantX = constraint.RotationalStiffnessX;

            if (constraint.RotationY == DOFType.Free)
                rfConstraint.RestraintConstantY = 0;
            else if (constraint.RotationY == DOFType.Fixed)
                rfConstraint.RestraintConstantY = -1;
            else
                rfConstraint.RestraintConstantY = constraint.RotationalStiffnessY;

            if (constraint.RotationZ == DOFType.Free)
                rfConstraint.RestraintConstantZ = 0;
            else if (constraint.RotationZ == DOFType.Fixed)
                rfConstraint.RestraintConstantZ = -1;
            else
                rfConstraint.RestraintConstantZ = constraint.RotationalStiffnessZ;


            return rfConstraint;
        }

        /***************************************************/
    }
}
