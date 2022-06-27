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

        private List<ILoad> ReadLoadCombinations(List<string> ids = null)
        {
            List<LoadCombination> loadCombinationList = new List<LoadCombination>();


            if (ids == null)
            {
                List<rf.LoadCombination> rfLoadCombinations = model.GetLoads().GetLoadCombinations().ToList();
                rf.ILoads l = model.GetLoads();
                int lcCount = l.GetLoadCombinationCount();

                //Dictionary<int, Loadcase> bhLoadcaseDict = new Dictionary<int, Loadcase>();
                //bhLoadcaseDict = ReadLoadcases().Distinct().ToDictionary(x => x.Number, x => x);

                //Dictionary<string, Bar> bhBarDict = new Dictionary<string, Bar>();
                //bool barsRead = false;

                for (int i = 0; i < lcCount; i++)
                {
                    rf.LoadCombination rfLoadCombination = rfLoadCombinations[i];// l.GetLoadCombination(i, rf.ItemAt.AtIndex).GetData();
                    rf.CombinationLoading[] rfCombiLoadings = l.GetLoadCombination(i, rf.ItemAt.AtIndex).GetLoadings();

                    LoadCombination bhLoadCombination;

                    string combiName = rfLoadCombination.Description;
                    int combiNo;
                    if (!int.TryParse(rfLoadCombination.ID, out combiNo))
                    {
                        Engine.Base.Compute.RecordWarning("Load Combination id: " + rfLoadCombination.ID + " could not be converted to int");
                    }

                    List<Loadcase> loadcases = new List<Loadcase>();//rfCombiLoadings.Select(x => x.Loading).ToList();
                    List<double> combiFactors = new List<double>();//rfCombiLoadings.Select(x => x.Factor).ToList();
                    foreach (rf.CombinationLoading cl in rfCombiLoadings)
                    {
                        combiFactors.Add(cl.Factor);
                        loadcases.Add(m_loadcaseDict[cl.No]);
                    }
                    bhLoadCombination = Engine.Structure.Create.LoadCombination(combiName, combiNo, loadcases, combiFactors);

                    loadCombinationList.Add(bhLoadCombination);
                }
            }
            else
            {
                foreach (string id in ids)
                {


                    //loadList.Add();
                }
            }

            return loadCombinationList;
        }

        /***************************************************/

    }
}
