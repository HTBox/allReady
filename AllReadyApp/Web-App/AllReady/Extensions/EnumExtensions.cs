using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;

namespace AllReady.Extensions
{
    public static class EnumExtensions
    {
        public static string ToName(this UserType theType)
        {
            return Enum.GetName(typeof(UserType), theType);
        }
    }
}
