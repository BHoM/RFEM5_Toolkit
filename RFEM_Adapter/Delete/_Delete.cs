/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using rf = Dlubal.RFEM5;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/

        protected override int Delete(Type type, IEnumerable<object> ids)
        {
            //Insert code here to enable deletion of specific types of objects with specific ids
            if (type.ToRfemType() == rf.ModelObjectType.UnknownObject)
            {
                return 0;//Fail
            }

            if (ids == null)//delete all of the type
            {
                int deleteCount = TypeCount(type);
                for (int i = 0; i < deleteCount - 1; i++)
                {
                    if (DeleteObject(type, i, true))
                        return 0;//could this not fail after some objects have been deleted ???
                }
                return deleteCount;
            }
            else//delete only objects with the id
            {
                int deleteCount = 0;
                foreach (int id in ids as dynamic)
                {
                    if (DeleteObject(type, id, false))
                        deleteCount++;
                }
                return deleteCount;
            }

        }

        /***************************************************/

        private bool DeleteObject(Type type, int id, bool byIndex)
        {
            bool success = true;
            string typeString = type.ToString();

            switch (typeString)
            {
                case "Node":
                    if (byIndex)
                        modelData.GetNode(id, rf.ItemAt.AtIndex).Delete();//<--- .Delete() returns void - there might need to be a check to see if the delete was successful
                    else
                        modelData.GetNode(id, rf.ItemAt.AtNo).Delete();
                    break;
                case "Bar":
                    if (byIndex)
                        modelData.GetMember(id, rf.ItemAt.AtIndex).Delete();
                    else
                        modelData.GetMember(id, rf.ItemAt.AtNo).Delete();
                    break;
                case "Material":
                    if (byIndex)
                        modelData.GetMaterial(id, rf.ItemAt.AtIndex).Delete();
                    else
                        modelData.GetMaterial(id, rf.ItemAt.AtNo).Delete();
                    break;
                case "SectionProperty":
                    if (byIndex)
                        modelData.GetCrossSection(id, rf.ItemAt.AtIndex).Delete();
                    else
                        modelData.GetCrossSection(id, rf.ItemAt.AtNo).Delete();
                    break;
                default:
                    success = false;//<---- log error
                    break;
            }

            return success;
        }

        private int TypeCount(Type type)
        {
            string typeString = type.ToString();

            switch (typeString)
            {
                case "Node":
                    return modelData.GetNodeCount();
                case "Bar":
                    return modelData.GetMemberCount();//<--- unsure of this!!! can there be other member types than bars ???
                case "Material":
                    return modelData.GetMaterialCount();
                case "SectionProperty":
                    return modelData.GetCrossSectionCount();
                default:
                    return 0;
            }
        }

    }
}
