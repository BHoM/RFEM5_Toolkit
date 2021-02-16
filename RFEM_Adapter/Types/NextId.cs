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
using BH.oM.Structure.MaterialFragments;
using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/

        protected override object NextFreeId(Type objectType, bool refresh = false)
        {
            int index;

            if (!refresh && m_indexDict.TryGetValue(objectType, out index))
            {
                index++;
                m_indexDict[objectType] = index;
            }
            else
            {
                index = GetLastIdOfType(objectType) + 1;
                m_indexDict[objectType] = index;
            }

            //m_indexDict[objectType] = index;
            return index;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private int GetLastIdOfType(Type objectType)
        {
            int lastId;
            
            string typeString = objectType.Name;

            if (objectType.GetInterfaces().Contains(typeof(IMaterialFragment)))// - - - verify that this works as intended. 'SteelSection' seems to increment the count unintended
            {
                typeString = "IMaterialFragment";
            }

            switch (typeString)
            {
                case "Node":
                    lastId = (modelData.GetNodeCount() == 0) ? 0 : modelData.GetNode(modelData.GetNodeCount() - 1, rf.ItemAt.AtIndex).GetData().No;
                    break;

                case "Constraint6DOF":
                    lastId = (modelData.GetNodalSupportCount() == 0) ? 0 : modelData.GetNodalSupport(modelData.GetNodalSupportCount() - 1, rf.ItemAt.AtIndex).GetData().No;
                    break;

                case "Bar":
                    lastId = (modelData.GetMemberCount() == 0) ? 0 : modelData.GetMember(modelData.GetMemberCount() - 1, rf.ItemAt.AtIndex).GetData().No;
                    break;

                case "IMaterialFragment":
                    lastId = (modelData.GetMaterialCount() == 0) ? 0 : modelData.GetMaterial(modelData.GetMaterialCount() - 1, rf.ItemAt.AtIndex).GetData().No;
                    break;

                case "ISectionProperty":
                    lastId = (modelData.GetCrossSectionCount() == 0) ? 0 : modelData.GetCrossSection(modelData.GetCrossSectionCount() - 1, rf.ItemAt.AtIndex).GetData().No;
                    break;

                case "Edge":
                    lastId = (modelData.GetLineCount() == 0) ? 0 : modelData.GetLine(modelData.GetLineCount() - 1, rf.ItemAt.AtIndex).GetData().No;
                    break;

                case "Panel":
                    lastId = (modelData.GetSurfaceCount() == 0) ? 0 : modelData.GetSurface(modelData.GetSurfaceCount() - 1, rf.ItemAt.AtIndex).GetData().No;
                    break;

                case "RigidLink":
                    lastId = (modelData.GetMemberCount() == 0) ? 0 : modelData.GetMember(modelData.GetMemberCount() - 1, rf.ItemAt.AtIndex).GetData().No;
                    break;


                default:
                    lastId = 0;//<---- log error
                    break;
            }
            return lastId;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/


        /***************************************************/
    }
}

