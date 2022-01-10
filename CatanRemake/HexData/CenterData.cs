﻿

namespace CatanRemake.HexData
{
    // Center of hexs only has three functions: Has a robber, has a number to be rolled, and has a type
    public class CenterData
    {
        public static readonly int[] robberIndex = { -1, -1 };
        public string tex = "Blank";

        // Number to be rolled. -1 for no number (desert)
        public int number = -1;

        // Whether has robber
        public bool hasRobber = false;

        public CenterData() { }
    }
}
