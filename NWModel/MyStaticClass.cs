using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWModel
{
    public static class MyStaticClass
    {
        public static string GroupByUniPrice(this decimal? price)
        {
            if (price < 30 || price == null)
                return "Low Price";
            else
                return "High Price";
        } 
    }
}
