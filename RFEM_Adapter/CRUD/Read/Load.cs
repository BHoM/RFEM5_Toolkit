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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;
using BH.Engine.Spatial;
using rf = Dlubal.RFEM5;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Loads;
using BH.Engine.Adapter;
using BH.oM.Adapters.RFEM5;

namespace BH.Adapter.RFEM5
{
    public partial class RFEM5Adapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private List<ILoad> ReadLoads(List<string> ids = null)
        {
            List<ILoad> loadList = new List<ILoad>();
            
            
            if (ids == null)
            {
                List<rf.LoadCase> rfLoadcases = model.GetLoads().GetLoadCases().ToList();
                rf.ILoads l = model.GetLoads();
                int lcCount = l.GetLoadCaseCount();

                Dictionary<int, Loadcase> bhLoadcaseDict = new Dictionary<int, Loadcase>();
                bhLoadcaseDict = ReadLoadcases().Distinct().ToDictionary(x => x.Number, x => x);

                Dictionary<string, Bar> bhBarDict = new Dictionary<string, Bar>();
                bool barsRead = false;

                for(int i = 0;i<lcCount;i++)
                {
                    rf.LoadCase rfLoadcase = l.GetLoadCase(i, rf.ItemAt.AtIndex).GetData();
                    Loadcase bhLoadcase;

                    bhLoadcase = bhLoadcaseDict[rfLoadcase.Loading.No];

                    
                    rf.MemberLoad[] rfMemberLoads = l.GetLoadCase(i, rf.ItemAt.AtIndex).GetMemberLoads();
                    if (rfMemberLoads.Length > 0)
                    {
                        if (!barsRead)
                        {
                            bhBarDict = ReadBars().ToDictionary(x => GetAdapterId(x).ToString(), x => x);
                            barsRead = true;
                        }


                        foreach (rf.MemberLoad rfLoad in rfMemberLoads)
                        {
                            List<string> barIds = BH.Engine.Adapters.RFEM5.Compute.GetIdListFromString(rfLoad.ObjectList);
                            oM.Base.BHoMGroup<Bar> barGroup = new oM.Base.BHoMGroup<Bar>();
                            barGroup.Elements.AddRange(barIds.Where(k => bhBarDict.ContainsKey(k)).Select(k=>bhBarDict[k]));


                            if (rfLoad.Distribution == rf.LoadDistributionType.UniformType)
                            {
                                BarUniformlyDistributedLoad barUniformLoad = rfLoad.FromRFEM(bhLoadcase, barGroup);
                                loadList.Add(barUniformLoad);

                            }
                            else if (rfLoad.Distribution == rf.LoadDistributionType.ConcentratedType)
                            {
                                BarPointLoad barPointLoad = new BarPointLoad();
                                //...
                            }
                            else
                            {
                                Engine.Base.Compute.RecordWarning("Load distribution of type: " + rfLoad.Distribution.ToString() + " is not supported!");
                            }
                        }


                    }

                }
            }
            else
            {
                foreach (string id in ids)
                {
                    

                    //loadList.Add();
                }
            }

            return loadList;
        }

        /***************************************************/

    }
}






