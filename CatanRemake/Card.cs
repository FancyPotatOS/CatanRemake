using System;
using System.Collections.Generic;
using System.Text;

namespace CatanRemake
{
    public class Card
    {
        public ResourceType resource;

        public Card(ResourceType r)
        {
            resource = r;
        }

        public enum ResourceType
        {
            Ore, Wheat, Sheep, Brick, Wood
        }
    }
}
