﻿using System;
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
        public static Constraint6DOF ToBHoM(this rf.NodalSupport rfConstraint)
        {
            Constraint6DOF bhConstraint = BH.Engine.Structure.Create.Constraint6DOF();
            bhConstraint.CustomData.Add(AdapterId, rfConstraint.No);

            //Translation
            if (rfConstraint.SupportConstantX == 0)
                bhConstraint.TranslationX = DOFType.Free;
            if (rfConstraint.SupportConstantX == -1)
                bhConstraint.TranslationX = DOFType.Fixed;
            else
                bhConstraint.TranslationalStiffnessX = rfConstraint.SupportConstantX;

            if (rfConstraint.SupportConstantY == 0)
                bhConstraint.TranslationY = DOFType.Free;
            if (rfConstraint.SupportConstantY == -1)
                bhConstraint.TranslationY = DOFType.Fixed;
            else
                bhConstraint.TranslationalStiffnessY = rfConstraint.SupportConstantY;

            if (rfConstraint.SupportConstantZ == 0)
                bhConstraint.TranslationZ = DOFType.Free;
            if (rfConstraint.SupportConstantZ == -1)
                bhConstraint.TranslationZ = DOFType.Fixed;
            else
                bhConstraint.TranslationalStiffnessZ = rfConstraint.SupportConstantZ;

            //Rotation
            if (rfConstraint.RestraintConstantX == 0)
                bhConstraint.RotationX = DOFType.Free;
            if (rfConstraint.RestraintConstantX == -1)
                bhConstraint.RotationX = DOFType.Fixed;
            else
                bhConstraint.RotationalStiffnessX = rfConstraint.RestraintConstantX;

            if (rfConstraint.RestraintConstantY == 0)
                bhConstraint.RotationY = DOFType.Free;
            if (rfConstraint.RestraintConstantY == -1)
                bhConstraint.RotationY = DOFType.Fixed;
            else
                bhConstraint.RotationalStiffnessY = rfConstraint.RestraintConstantY;

            if (rfConstraint.RestraintConstantZ == 0)
                bhConstraint.RotationZ = DOFType.Free;
            if (rfConstraint.RestraintConstantZ == -1)
                bhConstraint.RotationZ = DOFType.Fixed;
            else
                bhConstraint.RotationalStiffnessZ = rfConstraint.RestraintConstantZ;


            return bhConstraint;
        }

        /***************************************************/
    }
}
