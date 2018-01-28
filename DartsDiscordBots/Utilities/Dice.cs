using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartsDiscordBots.Utilities
{
    public class Dice
    {
        private static Random _random = new Random();

        int Sides { get; set; }

        public Dice(int sides)
        {
            this.Sides = sides;
        }

        public int Roll()
        {
            return _random.Next(1, this.Sides + 1);
        }

        public DiceResult Roll(int times)
        {
            List<int> temp = new List<int>();
            while (times > 0)
            {
                temp.Add(this.Roll());
                times--;
            }
            return new DiceResult(temp);
        }
    };

    public struct DiceResult
    {
        public int Total { get; set; }
        public List<int> Rolls { get; set; }

        public DiceResult(List<int> rolls)
        {
            Rolls = rolls;
            Total = 0;

            foreach (int val in rolls)
            {
                Total += val;
            }
        }

    }
}
