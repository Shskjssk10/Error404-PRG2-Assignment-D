using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error404_PRG2_V2
{
    class Topping
    {
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public Topping()
        {

        }

        public Topping(string t)
        {
            Type = t;
        }

        public override string ToString()
        {
            return $"type: {Type}";
        }
    }
}
