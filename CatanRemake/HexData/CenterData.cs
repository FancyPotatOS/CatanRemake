

namespace CatanRemake.HexData
{
    // Center of hexs only has three functions: Has a robber, has a number to be rolled, and has a type
    public class CenterData
    {
        public static readonly int[] robberIndex = { -1, -1 };

        public enum HexType
        {
            None,
            Mountain,
            Field,
            Farm,
            Forest,
            Mesa
        }

        public string tex = "Blank";
        public HexType type;

        // Number to be rolled. -1 for no number (desert)
        public int number = -1;

        public Card.ResourceType resourceType;

        // Whether has robber
        public bool hasRobber = false;
    }
}
