using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error404_PRG2_V2
{
    abstract class IceCream
    {
        private string option;

        public string Option
        {
            get { return option; }
            set { option = value; }
        }

        private int scoops;

        public int Scoops
        {
            get { return scoops; }
            set { scoops = value; }
        }

        public List<Flavour> Flavours { get; set; } = new List<Flavour>();

        public List<Topping> Toppings { get; set; } = new List<Topping>();

        public IceCream()
        {

        }

        public IceCream(string o, int s, List<Flavour> f, List<Topping> t)
        {
            Option = o;
            Scoops = s;
            Flavours = f;
            Toppings = t;
        }

        public abstract double CalculatePrice();

        public override string ToString()
        {
            return $"ice cream class working";
        }
    }
}
