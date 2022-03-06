using CatanRemake.HexData;
using CatanRemake.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatanRemake
{
    class Player
    {
        public List<Card> hand;
        public List<DevelopmentCard> devCards;
        public List<NumberResourcePair> pairs;

        public int victoryPoints;
        public int armyCount;
        public int roadLength;

        public int roads;
        public int settlements;
        public int cities;

        public int playerIndex;

        public Player(int plIndex)
        {
            playerIndex = plIndex;

            hand = new List<Card>();
            devCards = new List<DevelopmentCard>();
            pairs = new List<NumberResourcePair>();

            victoryPoints = 0;
            armyCount = 0;
            roadLength = 0;

            roads = 15;
            settlements = 5;
            cities = 4;
        }

        public void RolledNumber(int num)
        {
            // Find out which pairs in 'pairs' match 'num' and add those resources to 'hand'

            for (int i = 0; i < pairs.Count; i++)
            {
                NumberResourcePair pair = pairs[i];

                if (pair.number == num && !Board.board.GetAt(pair.pos[0], pair.pos[1]).hasRobber)
                {
                    Card give = new Card(pair.resourceType, "cards/" + Board.board.GetAt(pair.pos[0], pair.pos[1]).tex);

                    hand.Add(give);
                }
            }
        }

        public bool CanBuildCity()
        {
            if (cities == 0)
                return false;

            int oreCount = 0;
            int wheatCount = 0;

            for (int i = 0; i < hand.Count; i++)
            {
                Card card = hand[i];

                if (card.resource == Card.ResourceType.Ore)
                {
                    oreCount++;
                }

                /*  If we have wheat, add it to wheat count*/

                if (card.resource == Card.ResourceType.Wheat)
                {
                    wheatCount++;
                }

            }

            /* if we have enough wheat && ore, then return true*/

            if (oreCount >= 3 && wheatCount >= 2)
            {
                return true;
            }
            /*otherwise return false*/
            else return false;
        }

        public bool CanBuildSettlement()
        {
            if (settlements == 0)
                return false;

            int woodCount = 0;
            int wheatCount = 0;
            int sheepCount = 0;
            int brickCount = 0;


            for (int i = 0; i < hand.Count; i++)
            {
                Card card = hand[i];

                if (card.resource == Card.ResourceType.Wood)
                {
                    woodCount++;
                }

                if (card.resource == Card.ResourceType.Sheep)
                {
                    sheepCount++;
                }

                if (card.resource == Card.ResourceType.Brick)
                {
                    brickCount++;
                }

                /*  If we have wheat, add it to wheat count*/

                if (card.resource == Card.ResourceType.Wheat)
                {
                    wheatCount++;
                }

            }

            /* if we have enough wheat && ore, then return true*/

            if (woodCount >= 1 && wheatCount >= 1 && sheepCount >= 1 && brickCount >= 1)
            {
                return true;
            }
            /*otherwise return false*/
            else return false;
        }

        public bool CanBuildRoad()
        {
            if (settlements == 0)
                return false;

            int woodCount = 0;
            int brickCount = 0;


            for (int i = 0; i < hand.Count; i++)
            {
                Card card = hand[i];

                if (card.resource == Card.ResourceType.Wood)
                {
                    woodCount++;
                }

                if (card.resource == Card.ResourceType.Brick)
                {
                    brickCount++;
                }

            }

            /* if we have enough wheat && ore, then return true*/

            if (woodCount >= 1 && brickCount >= 1)
            {
                return true;
            }
            /*otherwise return false*/
            else return false;
        }

        public bool CanBuildDevelopmentCard()
        {
            if (settlements == 0)
                return false;

            int oreCount = 0;
            int wheatCount = 0;
            int sheepCount = 0;


            for (int i = 0; i < hand.Count; i++)
            {
                Card card = hand[i];

                if (card.resource == Card.ResourceType.Ore)
                {
                    oreCount++;
                }

                if (card.resource == Card.ResourceType.Sheep)
                {
                    sheepCount++;
                }

                /*  If we have wheat, add it to wheat count*/

                if (card.resource == Card.ResourceType.Wheat)
                {
                    wheatCount++;
                }

            }

            /* if we have enough wheat && ore, then return true*/

            if (oreCount >= 1 && wheatCount >= 1 && sheepCount >= 1)
            {
                return true;
            }
            /*otherwise return false*/
            else return false;
        }

        internal class NumberResourcePair
        {
            public int number;

            public Card.ResourceType resourceType;

            public int[] pos;
        }
    }
}
