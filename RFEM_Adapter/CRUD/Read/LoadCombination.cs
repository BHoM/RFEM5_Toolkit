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

        private List<LoadCombination> ReadLoadCombinations(List<string> ids = null)
        {
            List<LoadCombination> loadCombinationList = new List<LoadCombination>();
            rf.ILoads l = model.GetLoads();
            int lcCount = l.GetLoadCombinationCount();
            if (m_loadcaseDict.Count == 0)
            {
                ReadLoadcases();// - should/can this be handeled by setting dependency types?
            }

            if (ids == null)
            {
                for (int i = 0; i < lcCount; i++)
                {
                    rf.LoadCombination rfLoadCombination = l.GetLoadCombination(i, rf.ItemAt.AtIndex).GetData();
                    rf.CombinationLoading[] rfCombiLoadings = l.GetLoadCombination(i, rf.ItemAt.AtIndex).GetLoadings();

                    string combiName = rfLoadCombination.Description == "" ? rfLoadCombination.Definition : rfLoadCombination.Description;
                    int combiNo = rfLoadCombination.Loading.No;

                    List<Loadcase> loadcases = new List<Loadcase>();
                    List<double> combiFactors = new List<double>();
                    foreach (rf.CombinationLoading cl in rfCombiLoadings)
                    {
                        combiFactors.Add(cl.Factor);
                        loadcases.Add(m_loadcaseDict[cl.Loading.No]);
                    }
                    LoadCombination bhLoadCombination = Engine.Structure.Create.LoadCombination(combiName, combiNo, loadcases, combiFactors);

                    loadCombinationList.Add(bhLoadCombination);
                }
            }
            else
            {
                foreach (string id in ids)
                {
                    if(int.TryParse(id, out int i))
                    {
                        rf.LoadCombination rfLoadCombination = l.GetLoadCombination(i, rf.ItemAt.AtNo).GetData();
                        rf.CombinationLoading[] rfCombiLoadings = l.GetLoadCombination(i, rf.ItemAt.AtNo).GetLoadings();

                        string combiName = rfLoadCombination.Description == "" ? rfLoadCombination.Definition : rfLoadCombination.Description;
                        int combiNo = rfLoadCombination.Loading.No;

                        List<Loadcase> loadcases = new List<Loadcase>();
                        List<double> combiFactors = new List<double>();
                        foreach (rf.CombinationLoading cl in rfCombiLoadings)
                        {
                            combiFactors.Add(cl.Factor);
                            loadcases.Add(m_loadcaseDict[cl.Loading.No]);
                        }
                        LoadCombination bhLoadCombination = Engine.Structure.Create.LoadCombination(combiName, combiNo, loadcases, combiFactors);

                        loadCombinationList.Add(bhLoadCombination);
                    }
                    else
                    {
                        Engine.Base.Compute.RecordWarning("Cannot convert Load Combination ID : " + id + " into an integer!");

                    }
                }
            }

            return loadCombinationList;
        }

        /***************************************************/

    }
}
