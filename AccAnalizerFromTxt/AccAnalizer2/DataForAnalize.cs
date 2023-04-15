using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccAnalizer2
{
    public class DataForAnalize
    {
        public int ID { get; set; }
        public string country { get; set; }
        public int countReg { get; set; }
        public int countGoodReg { get; set; }
        public int countFailReg { get; set; }
        public int percent { get; set; }
        public int ratio { get; set; }
        public string countryName { get; set; }
    }
}
