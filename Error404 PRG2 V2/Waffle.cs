using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error404_PRG2_V2
{
    class Waffle : IceCream
    {
        private string waffleFlavour;

        public string WaffleFlavour
        {
            get { return waffleFlavour; }
            set { waffleFlavour = value; }
        }

        public Waffle()
        {

        }

        public Waffle(string o, int s, List<Flavour> f, List<Topping> t, string wf) : base(o, s, f, t)
        {
            waffleFlavour = wf;
        }

        public override double CalculatePrice()
        {
            double totalCost = 0;
            List<double> scoopPrice = new List<double>() { 7.00, 8.50, 9.50 };
            foreach (var flavour in Flavours)
            {
                if (flavour.Premium == true)
                {
                    totalCost += 2;
                }
            }
            totalCost += scoopPrice[Scoops - 1];
            if (waffleFlavour != "Original")
            {
                totalCost += 3;
            }
            return totalCost;
        }

        public override string ToString()
        {
            string flavourList = "";
            string toppingList = "";
            int counter = 1;
            foreach (var flavour in Flavours)
            {
                flavourList += $"[{counter}] {flavour.Type} ({flavour.Quantity})\n";
                counter++;
            }
            if (Toppings.Count > 0)
            {
                counter = 1;
                foreach (var topping in Toppings)
                {
                    toppingList += $"[{counter}] {topping.Type}\n";
                    counter++;
                }
            }
            else
            {
                toppingList = "None";
            }
            return $"\nType: {Option} \n" +
                $"Scoops: {Scoops}\n" +
                $"Flavours: \n{flavourList}" +
                $"Toppings:\n{toppingList}" +
                $"\nWaffle Flavour: {WaffleFlavour}";
        }
    }
}
