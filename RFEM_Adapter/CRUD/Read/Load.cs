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
using BH.oM.Adapters.RFEM;

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
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

                Dictionary<int, Loadcase> bhLoadcaseDict = new Dictionary<int, Loadcase>();//create dictionary of all loadcases ! ! ! !... TODO
                bhLoadcaseDict = ReadLoadcases().Distinct().ToDictionary(x => x.Number, x => x);


                for(int i =1;i<lcCount;i++)
                {
                    //rf.LoadCase lc = l.GetLoadCase(i, rf.ItemAt.AtIndex).GetData();
                    rf.LoadCase rfLoadcase = l.GetLoadCase(i, rf.ItemAt.AtIndex).GetData();
                    Loadcase bhLoadcase;
                    int lcId;
                    string rfLoadcaseId = rfLoadcase.ID.Trim(new char[] { '#', ' ' });

                    bhLoadcase = bhLoadcaseDict[rfLoadcase.Loading.No];

                    //if (int.TryParse(rfLoadcaseId, out lcId))//#23495
                    //    bhLoadcase = bhLoadcaseDict[lcId];
                    //else
                    //    continue;



                    rf.MemberLoad[] rfMemberLoads = l.GetLoadCase(i, rf.ItemAt.AtIndex).GetMemberLoads();

                    //Dictionary<string, Bar> barsById = new Dictionary<string, Bar>();
                    //oM.Base.BHoMGroup<Bar> objectGroup = new oM.Base.BHoMGroup<Bar>();
                    //if (rfMemberLoads.Length < 0)
                    //{
                    //    barsById = ReadBars().ToDictionary(x => x.AdapterId<RFEMId>().Id.ToString(), x => x);
                    //}

                    foreach (rf.MemberLoad rfLoad in rfMemberLoads)
                    {
                        //List<string> barIds = GetIdsFromRFEMString(rfLoad.ObjectList);
                        //objectGroup.Elements.AddRange(barsById.Where(x => barsById.Contains(x.Key)));


                        if(rfLoad.Distribution == rf.LoadDistributionType.UniformType)
                        {
                            BarUniformlyDistributedLoad barUniformLoad = new BarUniformlyDistributedLoad();
                            if(rfLoad.Direction== rf.LoadDirectionType.GlobalZType)
                            {
                                barUniformLoad.Axis = LoadAxis.Global;
                                barUniformLoad.Force.Z = rfLoad.Magnitude1;
                                barUniformLoad.Projected = false;
                                barUniformLoad.Loadcase = bhLoadcase;
                                barUniformLoad.Objects = new oM.Base.BHoMGroup<Bar>();// get bars based on Ids... from somewhere ...???

                                loadList.Add(barUniformLoad);
                            }

                        }
                        else if (rfLoad.Distribution == rf.LoadDistributionType.ConcentratedType)
                        {
                            BarPointLoad barPointLoad = new BarPointLoad();
                            //...
                        }
                        else
                        {
                            Engine.Reflection.Compute.RecordWarning("Load distribution of type: " + rfLoad.Distribution.ToString() + " is not supported!");
                        }
                    }

                    //if (rfLoadcase.Loading.Type == rf.LoadingType.LoadCaseType)
                    //{
                    //    //get loadcase
                    //    rf.MemberLoad[] rfMemberLoads = l.GetLoadCase(loadId, rf.ItemAt.AtIndex).GetMemberLoads();//NOTE: this uses .AtIndex ! ! not sure .AtNO is possible !! ! ! 
                    //    //rfMemberLoads[0].Direction;
                    //    //rfMemberLoads[0].Magnitude1;
                    //}
                    //else if(rfLoadcase.Loading.Type == rf.LoadingType.LoadCombinationType)
                    //{
                    //    //not implemented
                    //}
                    //else
                    //{
                    //    Engine.Reflection.Compute.RecordWarning("Loadcase of type: " + rfLoadcase.Loading.Type.ToString() + " is not supported!");
                    //}





                    //loadList.Add(barUniformLoad);
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

