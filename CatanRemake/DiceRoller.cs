using System;
using System.Collections.Generic;
using System.Text;

namespace CatanRemake
{
    class DiceRoller
    {
        public int rolledNumber;
        public int numberOfDice = 2;
        public int diceSides = 6;

        public DiceRoller()
        {

        }

        public int RandomNumber()
        {
            int coll = 0;

            for (int i = 0; i < numberOfDice; i++)
            {
                int val = CR.rng.Next(1, diceSides + 1);

                coll += val;
            }

            rolledNumber = coll;

            return coll;
        }
    }
}
