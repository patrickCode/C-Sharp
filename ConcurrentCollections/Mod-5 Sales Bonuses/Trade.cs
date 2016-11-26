using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_5_Sales_Bonuses
{
    class Trade
    {
        public SalesPerson Person { get; set; }

        //Negative if items were bought
        public int QuantitySold { get; private set; }

        public Trade(SalesPerson person, int quantitySold)
        {
            this.Person = person;
            this.QuantitySold = quantitySold;
        }
    }
}
