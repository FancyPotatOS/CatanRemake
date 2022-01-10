using System;
using System.Collections.Generic;
using System.Text;

namespace CatanRemake.HexData
{
    public struct CornerData
    {
        // Whether it is a settlement
        public bool hasSettlement;

        // Id of owner
        public int playerID;

        // Whether a city or not
        public bool isCity;
    }
}
