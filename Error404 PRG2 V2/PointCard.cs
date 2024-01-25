using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Error404_PRG2_V2
{
    class PointCard
    {
        private int points;

        public int Points
        {
            get { return points; }
            set { points = value; }
        }

        private int punchCard;

        public int PunchCard
        {
            get { return punchCard; }
            set { punchCard = value; }
        }

        private string tier = "Ordinary";

        public string Tier
        {
            get { return tier; }
            set { tier = value; } // check if can implement calculation here 
        }

        public PointCard()
        {

        }

        public PointCard(int p, int pc)
        {
            Points = p;
            PunchCard = pc;
        }

        public void AddPoints(int point)
        {
            points += point;

            if (points >= 100 && tier != "Gold")
            {
                tier = "Gold";
            }
            else if (points >= 50 && tier != "Silver")
            {
                tier = "Silver";
            }
        }

        public void RedeemPoints(int point)
        {
            points = Math.Max(points - point, 0);
        }

        public void Punch()
        {
            PunchCard += 1;
            if (PunchCard == 11)
            {
                PunchCard = 0;
            }
        }

        public override string ToString()
        {
            return $"{Points}, {PunchCard}, {Tier}";
        }
    }
}
