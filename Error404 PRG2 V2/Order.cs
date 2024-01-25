using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error404_PRG2_V2
{
    class Order
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private DateTime timeReceived;

        public DateTime TimeReceived
        {
            get { return timeReceived; }
            set { timeReceived = value; }
        }

        private DateTime? timeFulfilled;

        public DateTime? TimeFulfilled
        {
            get { return timeFulfilled; }
            set { timeFulfilled = value; }
        }
        public List<IceCream> IceCreamList { get; set; } = new List<IceCream>();

        public Order()
        {

        }

        public Order(int i, DateTime t)
        {
            Id = i;
            TimeReceived = t;

        }

        public void ModifyIceCream(int n)
        {
            //add to list need to n-1 cos indexing 
        }

        public void AddIceCream(IceCream iceCream)
        {
            IceCreamList.Add(iceCream);
        }

        public void DeleteIceCream(int n)
        {
            //delete from list
        }

        public double CalculateTotal()
        {
            double total = 0;
            foreach (var iceCream in IceCreamList)
            {
                total += iceCream.CalculatePrice();
            }
            return total;
        }

        public override string ToString()
        {
            return $"Order ID: {Id}\nTime Received: {TimeReceived}\nTime Fulfilled:" +
                $"{TimeFulfilled}";
        }
    }
}
