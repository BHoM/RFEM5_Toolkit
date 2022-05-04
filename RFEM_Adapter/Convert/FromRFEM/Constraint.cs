/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
            Constraint6DOF bhConstraint = new Constraint6DOF();

            
            bhConstraint.Name = rfConstraint.Comment;

            //Translation x
            if (rfConstraint.SupportConstantX == 0)
            {
                bhConstraint.TranslationX = DOFType.Free;
                bhConstraint.TranslationalStiffnessX = 0;
            }
            else if (rfConstraint.SupportConstantX == -1)
            {
                bhConstraint.TranslationX = DOFType.Fixed;
                bhConstraint.TranslationalStiffnessX = -1;
            }
            else
            {
                bhConstraint.TranslationalStiffnessX = rfConstraint.SupportConstantX;
                bhConstraint.TranslationX = DOFType.Spring;
            }
            //Translation Y
            if (rfConstraint.SupportConstantY == 0)
            {
                bhConstraint.TranslationY = DOFType.Free;
                bhConstraint.TranslationalStiffnessY = 0;
            }
            else if (rfConstraint.SupportConstantY == -1)
            {
                bhConstraint.TranslationY = DOFType.Fixed;
                bhConstraint.TranslationalStiffnessY = -1;
            }
            else
            {
                bhConstraint.TranslationalStiffnessY = rfConstraint.SupportConstantY;
                bhConstraint.TranslationY = DOFType.Spring;
            }
            //Translation Z
            if (rfConstraint.SupportConstantZ == 0)
            {
                bhConstraint.TranslationZ = DOFType.Free;
                bhConstraint.TranslationalStiffnessZ = 0;
            }
            else if (rfConstraint.SupportConstantZ == -1)
            {
                bhConstraint.TranslationZ = DOFType.Fixed;
                bhConstraint.TranslationalStiffnessZ = -1;
            }
            else
            {
                bhConstraint.TranslationalStiffnessX = rfConstraint.SupportConstantZ;
                bhConstraint.TranslationZ = DOFType.Spring;
            }



            //Rotation X
            if (rfConstraint.RestraintConstantX == 0)
            {
                bhConstraint.RotationX = DOFType.Free;
                bhConstraint.RotationalStiffnessX = 0;
            }
            else if (rfConstraint.RestraintConstantX == -1)
            {
                bhConstraint.RotationX = DOFType.Fixed;
                bhConstraint.RotationalStiffnessX = -1;
            }
            else
            {
                bhConstraint.RotationalStiffnessX = rfConstraint.RestraintConstantX;
                bhConstraint.RotationX = DOFType.Spring;
            }

            //Rotation Y
            if (rfConstraint.RestraintConstantY == 0)
            {
                bhConstraint.RotationY = DOFType.Free;
                bhConstraint.RotationalStiffnessY = 0;
            }
            else if (rfConstraint.RestraintConstantY == -1)
            {
                bhConstraint.RotationY = DOFType.Fixed;
                bhConstraint.RotationalStiffnessY = -1;
            }
            else
            {
                bhConstraint.RotationalStiffnessY = rfConstraint.RestraintConstantY;
                bhConstraint.RotationY = DOFType.Spring;
            }

            //Rotation Z
            if (rfConstraint.RestraintConstantZ == 0)
            {
                bhConstraint.RotationZ = DOFType.Free;
                bhConstraint.RotationalStiffnessZ = 0;
            }
            else if (rfConstraint.RestraintConstantZ == -1)
            {
                bhConstraint.RotationZ = DOFType.Fixed;
                bhConstraint.RotationalStiffnessZ = -1;
            }
            else
            {
                bhConstraint.RotationalStiffnessZ = rfConstraint.RestraintConstantZ;
                bhConstraint.RotationZ = DOFType.Spring;
            }


            bhConstraint.SetAdapterId(typeof(RFEMId), rfConstraint.No);

            return bhConstraint;
        }

        /***************************************************/
    }
}


