/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base;
using System.Runtime.InteropServices;
using rf = Dlubal.RFEM5;
using System.Collections;
using BH.oM.Adapter;
using BH.oM.Base;
using BH.oM.Adapter.Commands;


namespace BH.Adapter.RFEM5
{
    public partial class RFEM5Adapter
    {
        public override Output<List<object>, bool> Execute(IExecuteCommand command, ActionConfig actionConfig = null)
        {
            var output = new Output<List<object>, bool>() { Item1 = null, Item2 = false };

            output.Item2 = RunCommand(command as dynamic);

            return output;
        }

        private bool RunCommand(ClearResults command)
        {
            modelData.Clean();
            return true;
        }

        // Fallback case
        private bool RunCommand(IExecuteCommand command)
        {
            BH.Engine.Base.Compute.RecordError($"The command {command.GetType().Name} is not supported by {this.GetType().Name}");
            return true;
        }


        /*
        private bool RunCommand(BH.oM.Adapter.Commands - "UNLOCK")
        {
            modelData.Clean();
            return true;
        }

        private bool RunCommand(BH.oM.Adapter.Commands - "LOCK")
        {
            modelData.PrepareModification();
            return true;
        }

        private bool RunCommand(BH.oM.Adapter.Commands - "READSECTIONS")
        {
            return ReadSectionFromRFEMLibrary("IPE 100");
            return true;
        }
        */
    }
}




