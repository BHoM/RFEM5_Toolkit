/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM5
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static rf.NodalSupport ToRFEM(this Constraint6DOF constraint, int constraintId, int nodeId)
        {
            rf.NodalSupport rfConstraint = new rf.NodalSupport();
            rfConstraint.No = constraintId;
            rfConstraint.NodeList = nodeId.ToString();//<-- id reference to node(s) required for writing constraint to RFEM 


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






