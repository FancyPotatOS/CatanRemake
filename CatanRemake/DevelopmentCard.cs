using System;
using System.Collections.Generic;
using System.Text;

namespace CatanRemake
{
    public class DevelopmentCard
    {
        public DevelopmentCardType devCardType;

        public DevelopmentCard(DevelopmentCardType dct)
        {
            devCardType = dct;
        }

        public enum DevelopmentCardType
        {
            Monopoly, FreeChicken, YearOfPlenty, RoadBuilder, Knight, VictoryPoint
        }
    }
}
