/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Base;
using BH.oM.Data.Requests;
using System.Runtime.InteropServices;
using rf = Dlubal.RFEM5;
using Dlubal.RFEM5;

using System.Reflection;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Adapter;

namespace BH.Adapter.RFEM5
{
    public partial class RFEM5Adapter
    {
        public override IEnumerable<object> Pull(IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            if (IsApplicationRunning() & TryToShowApp())
                return base.Pull(request, pullType, actionConfig);
            else
            {
                BH.Engine.Base.Compute.RecordError("Make sure that either the RFEM Adapter component has opened an instance of the application or you have opened one yourself."
               + "\nCheck if you have a frozen instance of RFEM in the background. Look in Task Manager.");
                return null;
            }
        }

    }
}





