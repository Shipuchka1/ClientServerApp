using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
   public class Arguments
    {
        public string className { get; set; }
        public string methodName { get; set; }
        public List<Argument> args { get; set; }
        public Argument result { get; set; }
        public Arguments()
        {
            args = new List<Argument>();
        }
    }

    public class Argument
    {
        public string type { get; set; }
        public object value { get; set; }
    }
}
