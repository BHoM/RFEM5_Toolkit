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

namespace BH.Adapter.RFEM
{
    public partial class RFEMAdapter
    {

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private List<Loadcase> ReadLoadcases(List<string> ids = null)
        {
            List<Loadcase> loadcaseList = new List<Loadcase>();
            int loadId;
            Loadcase loadcase;
            string lcName;

            if (ids == null)
            {
                List<rf.LoadCase> rfLoadcases = model.GetLoads().GetLoadCases().ToList();

                foreach (rf.LoadCase rfLoadcase in rfLoadcases)
                {
                    loadId = rfLoadcase.Loading.No;
                    lcName = rfLoadcase.Description;//.ID
                    loadcase = BH.Engine.Structure.Create.Loadcase(lcName, loadId, GetLoadNature(rfLoadcase.ActionCategory));
                    loadcaseList.Add(loadcase);

                    //string rfLoadcaseId = rfLoadcase.ID.Trim(new char[] { '#', ' ' });//some loadcases can have the id: "" !!
                    //if(int.TryParse(rfLoadcaseId, out loadId))
                    //{
                    //    lcName = rfLoadcase.Description;//.ID
                    //    loadcase = BH.Engine.Structure.Create.Loadcase(lcName, loadId, GetLoadNature(rfLoadcase.ActionCategory));
                    //    loadcaseList.Add(loadcase);
                    //}
                    //else
                    //    Engine.Reflection.Compute.RecordWarning("loadcase id: " + rfLoadcase.ID + " could not be converted to int");
                }
            }
            else
            {
                foreach (string id in ids)
                {
                    if (int.TryParse(id, out loadId))
                    {
                        rf.LoadCase rfLoadcase = model.GetLoads().GetLoadCase(loadId, rf.ItemAt.AtNo).GetData();
                        lcName = rfLoadcase.Description;//.ID
                        loadcase = BH.Engine.Structure.Create.Loadcase(lcName, loadId, GetLoadNature(rfLoadcase.ActionCategory));
                        loadcaseList.Add(loadcase);

                    }
                    else
                        Engine.Reflection.Compute.RecordWarning("loadcase id: " + id + " could not be converted to int");
                }
            }

            return loadcaseList;
        }

        /***************************************************/

        private LoadNature GetLoadNature(rf.ActionCategoryType rfCategory)
        {

            switch (rfCategory)
            {
                case rf.ActionCategoryType.UnknownActionCategory:
                    return LoadNature.Other;
                case rf.ActionCategoryType.Permanent:
                    return LoadNature.Other;
                case rf.ActionCategoryType.Prestress:
                    return LoadNature.Prestress;
                case rf.ActionCategoryType.SnowIce:
                    return LoadNature.Snow;
                case rf.ActionCategoryType.Wind:
                    return LoadNature.Wind;
                case rf.ActionCategoryType.TemperatureNonFire:
                    return LoadNature.Temperature;
                case rf.ActionCategoryType.SettlementsOfFoundationSoil:
                    return LoadNature.Other;
                case rf.ActionCategoryType.OtherCategory:
                    return LoadNature.Other;
                case rf.ActionCategoryType.Accidental:
                    return LoadNature.Accidental;
                case rf.ActionCategoryType.Earthquake:
                    return LoadNature.Seismic;
                case rf.ActionCategoryType.UserDefined:
                case rf.ActionCategoryType.ImperfectionCategory:
                case rf.ActionCategoryType.PermanentSmallFluctuations:
                case rf.ActionCategoryType.WeightOfIce:
                    return LoadNature.Other;
                case rf.ActionCategoryType.DeadLoad:
                case rf.ActionCategoryType.DeadLoadDL:
                case rf.ActionCategoryType.DeadLoadGK:
                    return LoadNature.Dead;

                case rf.ActionCategoryType.PermanentFromCranes:
                case rf.ActionCategoryType.PermanentImposed:
                case rf.ActionCategoryType.SelfStrainingForce:
                case rf.ActionCategoryType.Variable:
                case rf.ActionCategoryType.TrafficLoads:
                    return LoadNature.Other;

                case rf.ActionCategoryType.Live:
                case rf.ActionCategoryType.RoofLive:
                    return LoadNature.Live;

                case rf.ActionCategoryType.Imposed:
                case rf.ActionCategoryType.ImposedCategoryA:
                case rf.ActionCategoryType.ImposedCategoryB:
                case rf.ActionCategoryType.ImposedCategoryC:
                case rf.ActionCategoryType.ImposedCategoryD:
                case rf.ActionCategoryType.ImposedCategoryE:
                case rf.ActionCategoryType.ImposedLive:
                case rf.ActionCategoryType.ImposedLoad:
                case rf.ActionCategoryType.ImposedCategoryF:
                case rf.ActionCategoryType.ImposedCategoryG:
                case rf.ActionCategoryType.ImposedCategoryH:
                case rf.ActionCategoryType.ImposedCategoryI:
                case rf.ActionCategoryType.ImposedCategoryJ:
                case rf.ActionCategoryType.ImposedLoadsCategoryH:
                case rf.ActionCategoryType.ImposedLoadsCategoryK:
                case rf.ActionCategoryType.ImposedLoadsCategoryKOther:
                    return LoadNature.Other;

                case rf.ActionCategoryType.SnowIce2:
                case rf.ActionCategoryType.SnowFinland:
                case rf.ActionCategoryType.SnowHGreaterThan1000:
                case rf.ActionCategoryType.ShowHLowerThan1000:
                case rf.ActionCategoryType.SnowSKLowerThan275:
                case rf.ActionCategoryType.SnowSKGreaterThan275:
                case rf.ActionCategoryType.SnowOutdoorSKLowerThan275:
                case rf.ActionCategoryType.SnowOutdoorSKGreaterThan275:
                case rf.ActionCategoryType.Snow:
                case rf.ActionCategoryType.SnowSweden1:
                case rf.ActionCategoryType.SnowSweden2:
                case rf.ActionCategoryType.SnowSweden3:
                case rf.ActionCategoryType.SnowSaintPierreEtMiquelon:
                case rf.ActionCategoryType.SnowIceRain:
                    return LoadNature.Snow;

                case rf.ActionCategoryType.WindLoad:
                case rf.ActionCategoryType.WindOnIce:
                case rf.ActionCategoryType.WindLiveLoad:
                case rf.ActionCategoryType.WindWL:
                case rf.ActionCategoryType.WindWK:
                case rf.ActionCategoryType.WindLoadsFwkPersistentDesignSituations:
                case rf.ActionCategoryType.WindLoadsFwkExecution:
                case rf.ActionCategoryType.WindLoadsFw:
                    return LoadNature.Wind;

                case rf.ActionCategoryType.UniformTemperatures:
                case rf.ActionCategoryType.TemperatureShrinkageCreep:
                    return LoadNature.Temperature;

                case rf.ActionCategoryType.PermanentFromSoilEarthLoad:
                case rf.ActionCategoryType.PermanentFromSoilEarthPressure:
                case rf.ActionCategoryType.PermanentFromSoilWaterPressure:
                case rf.ActionCategoryType.VariableFromSoilEarthPressure:
                case rf.ActionCategoryType.VariableFromSoilWaterPressure:
                case rf.ActionCategoryType.LateralEarthWaterPressure:
                case rf.ActionCategoryType.LateralEarthPressure:
                case rf.ActionCategoryType.Fluids:
                case rf.ActionCategoryType.Flood:
                case rf.ActionCategoryType.Rain:
                case rf.ActionCategoryType.CraneLoad:
                case rf.ActionCategoryType.ErectionLoad:
                case rf.ActionCategoryType.NotionalHorizontalForces:
                case rf.ActionCategoryType.ActionsDuringExecution:
                    return LoadNature.Other;

                case rf.ActionCategoryType.AccidentalLoad:
                    return LoadNature.Accidental;

                case rf.ActionCategoryType.EarthquakeLoadE:
                case rf.ActionCategoryType.EarthquakeLiveLoad:
                case rf.ActionCategoryType.EarthquakeLoadEL:
                    return LoadNature.Seismic;

                case rf.ActionCategoryType.GbPermanent:
                case rf.ActionCategoryType.GbCivilBuildings211:
                case rf.ActionCategoryType.GbCivilBuildings212:
                case rf.ActionCategoryType.GbCivilBuildings22:
                case rf.ActionCategoryType.GbCivilBuildings231:
                case rf.ActionCategoryType.GbCivilBuildings232:
                case rf.ActionCategoryType.GbCivilBuildings241:
                case rf.ActionCategoryType.GbCivilBuildings242:
                case rf.ActionCategoryType.GbCivilBuildings251:
                case rf.ActionCategoryType.GbCivilBuildings252:
                case rf.ActionCategoryType.GbCivilBuildings261:
                case rf.ActionCategoryType.GbCivilBuildings262:
                case rf.ActionCategoryType.GbCivilBuildings27:
                case rf.ActionCategoryType.GbCivilBuildings281:
                case rf.ActionCategoryType.GbCivilBuildings282:
                case rf.ActionCategoryType.GbCivilBuildings291:
                case rf.ActionCategoryType.GbCivilBuildings292:
                case rf.ActionCategoryType.GbCivilBuildings2101:
                case rf.ActionCategoryType.GbCivilBuildings2102:
                case rf.ActionCategoryType.GbCivilBuildings2111:
                case rf.ActionCategoryType.GbCivilBuildings2112:
                case rf.ActionCategoryType.GbCivilBuildings2113:
                case rf.ActionCategoryType.GbCivilBuildings2121:
                case rf.ActionCategoryType.GbCivilBuildings2122:
                case rf.ActionCategoryType.GbIndustrialBuilding31:
                case rf.ActionCategoryType.GbIndustrialBuilding321:
                case rf.ActionCategoryType.GbIndustrialBuilding322:
                case rf.ActionCategoryType.GbIndustrialBuilding323:
                case rf.ActionCategoryType.GbIndustrialBuilding33:
                case rf.ActionCategoryType.GbIndustrialBuilding34:
                case rf.ActionCategoryType.GbIndustrialBuilding35:
                case rf.ActionCategoryType.GbIndustrialBuilding36:
                case rf.ActionCategoryType.GbLiveLoadsOnRoofs41:
                case rf.ActionCategoryType.GbLiveLoadsOnRoofs42:
                case rf.ActionCategoryType.GbLiveLoadsOnRoofs43:
                case rf.ActionCategoryType.GbLiveLoadsOnRoofs44:
                case rf.ActionCategoryType.GbAshLoadOnRoofings51:
                case rf.ActionCategoryType.GbAshLoadOnRoofings52:
                case rf.ActionCategoryType.GbCraneLoads61:
                case rf.ActionCategoryType.GbCraneLoads62:
                case rf.ActionCategoryType.GbCraneLoads63:
                case rf.ActionCategoryType.GbCraneLoads64:
                case rf.ActionCategoryType.GbSnowLoadZone1:
                case rf.ActionCategoryType.GbSnowLoadZone2:
                case rf.ActionCategoryType.GbSnowLoadZone3:
                case rf.ActionCategoryType.GbWindLoadSccordingTo714:
                case rf.ActionCategoryType.GbAccidentalActions:
                case rf.ActionCategoryType.GbSeismic:
                case rf.ActionCategoryType.UnevenSettlements:
                case rf.ActionCategoryType.Gr1A:
                case rf.ActionCategoryType.Gr1B:
                case rf.ActionCategoryType.Gr2:
                case rf.ActionCategoryType.Gr3:
                case rf.ActionCategoryType.Gr4:
                case rf.ActionCategoryType.Gr5:
                case rf.ActionCategoryType.ConstructionLoadsDueToWorkingPersonnel:
                case rf.ActionCategoryType.OtherConstructionLoads:
                case rf.ActionCategoryType.ImposedFromCranesCategoryA:
                case rf.ActionCategoryType.ImposedFromCranesCategoryB:
                case rf.ActionCategoryType.ImposedFromCranesCategoryC:
                case rf.ActionCategoryType.ImposedFromCranesCategoryD:
                case rf.ActionCategoryType.ImposedFromCranesGeneral:
                case rf.ActionCategoryType.PipingSelfWeight:
                case rf.ActionCategoryType.PipingFluid:
                case rf.ActionCategoryType.PipingFluidPressureTest:
                case rf.ActionCategoryType.PipingTemperature:
                case rf.ActionCategoryType.PipingPressure:
                case rf.ActionCategoryType.PipingTestPressure:
                case rf.ActionCategoryType.PipingDisplacement:
                case rf.ActionCategoryType.PipingHangerCategory:
                case rf.ActionCategoryType.PipingColdSpring:
                case rf.ActionCategoryType.FormFindingResults:
                case rf.ActionCategoryType.Nbr1A:
                case rf.ActionCategoryType.Nbr1B:
                case rf.ActionCategoryType.Nbr1C:
                case rf.ActionCategoryType.Nbr1D:
                case rf.ActionCategoryType.Nbr1E:
                case rf.ActionCategoryType.Nbr1F:
                case rf.ActionCategoryType.NbrSettlements:
                case rf.ActionCategoryType.NbrActionsMaxValues:
                case rf.ActionCategoryType.NbrGeneralVariableActions:
                case rf.ActionCategoryType.NbrExceptional:
                case rf.ActionCategoryType.LoadArisingFromExtraordinaryEvent:
                    return LoadNature.Other;
                default:
                    return LoadNature.Other;

            }
        }
    }
}


