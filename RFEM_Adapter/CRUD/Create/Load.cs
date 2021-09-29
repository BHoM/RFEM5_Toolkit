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
using rf = Dlubal.RFEM5;
using BH.oM.Structure.Loads;
using BH.Adapter.RFEM;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private bool CreateCollection(IEnumerable<ILoad> loads)
        {
            if (loads.Count() > 0)
            {
                int loadId = 0;

                List<ILoad> loadList = loads.ToList();

                //*****move this to adapter *****
                modelData.FinishModification();
                rf.ILoads rfloads = model.GetLoads();
                //*******************************


                var loadGroupByCase = loadList.GroupBy(load => load.Loadcase.Number);

                foreach (var caseGroup in loadGroupByCase)
                {
                    int loadcaseId = caseGroup.Key;
                    rf.ILoadCase rfLoadcase = rfloads.GetLoadCase(loadcaseId, rf.ItemAt.AtNo);
                    rfLoadcase.PrepareModification();

                    foreach (ILoad load in caseGroup)
                    {
                        loadId = GetAdapterId<int>(load);

                        if(load.GetType().IsAssignableFrom(typeof(BarUniformlyDistributedLoad)))
                        {
                            BarUniformlyDistributedLoad barLoad = load as BarUniformlyDistributedLoad;
                            rf.MemberLoad[] rfBarLoads = barLoad.ToRFEM(loadId, loadcaseId).ToArray();
                            //rfLoadcase.SetMemberLoads(rfBarLoads);
                            foreach (rf.MemberLoad item in rfBarLoads)
                            {
                                rfLoadcase.SetMemberLoad(item);
                            }
                        }
                        //Convert.ToRFEM(load as dynamic, loadId, loadcaseId);

                        //rfLoadcase.SetMemberLoad(rfMemberLoad);

                    }
                    rfLoadcase.FinishModification();// <---- move to adapter
                    modelData.PrepareModification(); //<--- hack to avoid app lock issue

                }



                //rfLoadcase.SetMemberLoads(rfMemberLoads);
            }

            return true;
        }

    }
}