using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error404_PRG2_V2
{
    class Customer
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int memberID;

        public int MemberID
        {
            get { return memberID; }
            set { memberID = value; }
        }

        private DateTime dob;

        public DateTime Dob
        {
            get { return dob; }
            set { dob = value; }
        }

        private Order currentOrder;

        public Order CurrentOrder
        {
            get { return currentOrder; }
            set { currentOrder = value; }
        }
        public List<Order> OrderHistory { get; set; } = new List<Order>();

        public PointCard Rewards { get; set; }

        public Customer()
        {
            Rewards = new PointCard();
        }

        public Customer(string n, int m, DateTime d)
        {
            Name = n;
            MemberID = m;
            Dob = d;
            Rewards = new PointCard();
        }

        public Order MakeOrder()
        {
            Order order = new Order(MemberID, DateTime.Now);
            return order;
        }

        public bool IsBirthday()
        {
            if (DateTime.Now == Dob)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"{Name,-10} {MemberID,-13} {Dob.ToShortDateString()}";
        }
    }
}
