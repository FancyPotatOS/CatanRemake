using System;
using System.Collections.Generic;
using System.Text;

namespace CatanRemake
{
    public class Card
    {
        public ResourceType resource;
        public string cardString;

        public Card(ResourceType r, string cS)
        {
            resource = r;

            cardString = cS;
        }

        public enum ResourceType
        {
            Ore, Wheat, Sheep, Brick, Wood
        }
    }
}
