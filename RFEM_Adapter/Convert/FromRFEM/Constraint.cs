/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Constraints;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //Add methods for converting to BHoM from the specific software types. 
        //Only do this if possible to do without any com-calls or similar to the adapter
        public static Constraint6DOF FromRFEM(this rf.NodalSupport rfConstraint)
        {
            Constraint6DOF bhConstraint = BH.Engine.Structure.Create.Constraint6DOF();

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

            bhConstraint.SetAdapterId(typeof(RFEMId), rfConstraint.No);
            return bhConstraint;
        }

        /***************************************************/
    }
}

