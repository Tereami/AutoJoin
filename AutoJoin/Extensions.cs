﻿using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJoin
{
    public static class Extensions
    {
        public static int GetElementIdValue(this ElementId id)
        {
            int result = 0;
#if R2017 || R2018 || R2019 || R2020 || R2021 || R2022  || R2023
            result = id.GetElementIdValue();
#else
            result = (int)id.Value;
#endif
            return result;
        }
    }
}
