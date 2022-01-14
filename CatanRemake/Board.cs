using CatanRemake.HexData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using ValueNoise;

namespace CatanRemake.States
{
    class Board : IState
    {
        public static HexagonGrid<CenterData, EdgeData, CornerData> board;

        public Board()
        {
            board = new HexagonGrid<CenterData, EdgeData, CornerData>(CR.hexEdgeSize);

            /*  Initialize hex data */
            for (int j = board.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < board.arrSize; i++)
                {
                    if (board.InRange(i, j))
                    {
                        // Draw from code
                        board.SetAt(new CenterData(), i, j);
                    }
                }
            }
            /**/

        }

        public void Update()
        {

            /**/
            if (CR.newKeys.Contains(Keys.Y))
            {
                ResetBoard();
            }
            /**/
            if (CR.newKeys.Contains(Keys.U))
            {
                HexagonGrid<CenterData, EdgeData, CornerData>.Corners[] dir =
                    new HexagonGrid<CenterData, EdgeData, CornerData>.Corners[]
                {
                    HexagonGrid<CenterData, EdgeData, CornerData>.Corners.DOWNLEFT,
                    HexagonGrid<CenterData, EdgeData, CornerData>.Corners.DOWNRIGHT,
                    HexagonGrid<CenterData, EdgeData, CornerData>.Corners.LEFT,
                    HexagonGrid<CenterData, EdgeData, CornerData>.Corners.RIGHT,
                    HexagonGrid<CenterData, EdgeData, CornerData>.Corners.UPLEFT,
                    HexagonGrid<CenterData, EdgeData, CornerData>.Corners.UPRIGHT
                };

                for (int i = 0; i < board.arrSize; i++)
                {
                    for (int j = board.range[i][0]; j <= board.range[i][1]; j++)
                    {

                        // Choose a random direction
                        HexagonGrid<CenterData, EdgeData, CornerData>.Corners randDir = dir[(int)(CR.rng.NextDouble() * dir.Length)];

                        // Random color in colors
                        CornerData cd = new CornerData
                        {
                            hasSettlement = true,
                            playerID = (int)(CR.rng.NextDouble() * (CR.colors.Length - 1) + 1)
                        };
                        board.SetAtCorner(cd, i, j, randDir);
                    }
                }
            }
            /**/
            if (CR.newKeys.Contains(Keys.I))
            {
                HexagonGrid<CenterData, EdgeData, CornerData>.Edges[] dir =
                {
                    HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UPLEFT,
                    HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UP,
                    HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UPRIGHT,
                    HexagonGrid<CenterData, EdgeData, CornerData>.Edges.DOWNLEFT,
                    HexagonGrid<CenterData, EdgeData, CornerData>.Edges.DOWN,
                    HexagonGrid<CenterData, EdgeData, CornerData>.Edges.DOWNRIGHT
                };

                for (int i = 0; i < board.arrSize; i++)
                {
                    for (int j = board.range[i][0]; j <= board.range[i][1]; j++)
                    {

                        // Random color in colors
                        EdgeData ed; ed.roadID = (int)(CR.rng.NextDouble() * (CR.colors.Length - 1) + 1);
                        HexagonGrid<CenterData, EdgeData, CornerData>.Edges randDir = dir[(int)(CR.rng.NextDouble() * dir.Length)];
                        board.SetAtEdge(ed, i, j, randDir);
                    }
                }
            }
            /**/
            if (CR.newKeys.Contains(Keys.O))
            {
                for (int i = 0; i < board.arrSize; i++)
                {
                    for (int j = board.range[i][0]; j <= board.range[i][1]; j++)
                    {
                        if (CR.rng.NextDouble() > 0.80)
                        {
                            int rangeStart = 2;
                            int rangeSize= 10;

                            board.GetAt(i, j).number = (int)(CR.rng.NextDouble() * rangeSize + rangeStart);
                        }
                    }
                }
            }
            /**/
        }

        public void Draw()
        {
            /*  Draw every hexagon   */
            for (int j = board.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < board.arrSize; i++)
                {
                    if (board.InRange(i, j))
                    {
                        // Draw from code
                        DrawHex(CR.texs[board.GetAt(i, j).tex], i, j, CR.cameraPos, Color.White);
                    }
                }
            }

            /*  Draw every token    */
            for (int j = board.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < board.arrSize; i++)
                {
                    if (board.InRange(i, j))
                    {
                        DrawToken(i, j, CR.cameraPos, Color.White);
                    }
                }
            }

            /**/

            /*  Draw every Edge  */
            for (int j = board.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < board.arrSize; i++)
                {
                    if (board.InRange(i, j))
                    {
                        // Draw top 3 hexes
                        HexagonGrid<CenterData, EdgeData, CornerData>.Edges[] dir =
                        {
                            HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UPLEFT,
                            HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UP,
                            HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UPRIGHT,
                            HexagonGrid<CenterData, EdgeData, CornerData>.Edges.DOWNLEFT,
                            HexagonGrid<CenterData, EdgeData, CornerData>.Edges.DOWN,
                            HexagonGrid<CenterData, EdgeData, CornerData>.Edges.DOWNRIGHT
                        };

                        for (int y = 0; y < dir.Length; y++)
                        {
                            int val = board.GetAtEdge(i, j, dir[y]).roadID;
                            if (val == 0)
                                continue;
                            else
                                DrawEdge(i, j, CR.cameraPos, dir[y], CR.colors[val]);
                        }
                    }
                }
            }

            /*  Draw every corner    */
            for (int j = board.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < board.arrSize; i++)
                {
                    if (board.InRange(i, j))
                    {
                        // Draw top 3 hexes
                        HexagonGrid<CenterData, EdgeData, CornerData>.Corners[] dir = {
                            HexagonGrid<CenterData, EdgeData, CornerData>.Corners.UPRIGHT,
                            HexagonGrid<CenterData, EdgeData, CornerData>.Corners.UPLEFT,
                            HexagonGrid<CenterData, EdgeData, CornerData>.Corners.LEFT,
                            HexagonGrid<CenterData, EdgeData, CornerData>.Corners.RIGHT,
                            HexagonGrid<CenterData, EdgeData, CornerData>.Corners.DOWNRIGHT,
                            HexagonGrid<CenterData, EdgeData, CornerData>.Corners.DOWNLEFT };

                        for (int y = 0; y < dir.Length; y++)
                        {
                            CornerData corner = board.GetAtCorner(i, j, dir[y]);
                            if (!corner.hasSettlement)
                                continue;
                            else
                                DrawCorner(i, j, CR.cameraPos, dir[y], CR.colors[corner.playerID]);
                        }
                    }
                }
            }

            /*  Draw every player    * /
            for (int j = board.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < board.arrSize; i++)
                {
                    if (board.InRange(i, j))
                    {
                        CenterData cd = board.GetAt(i, j);

                        foreach (int id in cd.players)
                            DrawPlayers(i, j, start);
                    }
                }
            }
            /**/
        }

        // Prints mask corner at i, j hex on topright/topleft corner
        public void DrawCorner(int i, int j, Point start, HexagonGrid<CenterData, EdgeData, CornerData>.Corners corner, Color c)
        {
            Point pos;
            Texture2D tex;
            // Determine type of corner
            if (corner == HexagonGrid<CenterData, EdgeData, CornerData>.Corners.UPLEFT)
            {
                // Get UPLEFT point for corner
                pos = GetDrawPos(i, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPLEFTPOINT, CR.scale);
                tex = CR.texs["U_D_L_Point"];
            }
            else if (corner == HexagonGrid<CenterData, EdgeData, CornerData>.Corners.UPRIGHT)
            {
                // Get UPRIGHT point for corner
                pos = GetDrawPos(i, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHTPOINT, CR.scale);
                tex = CR.texs["U_D_R_Point"];
            }
            else if (corner == HexagonGrid<CenterData, EdgeData, CornerData>.Corners.DOWNLEFT)
            {
                // Get UPLEFT point for corner (i, j - 1)
                pos = GetDrawPos(i, j - 1, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPLEFTPOINT, CR.scale);
                tex = CR.texs["U_D_L_Point"];
            }
            else if (corner == HexagonGrid<CenterData, EdgeData, CornerData>.Corners.DOWNRIGHT)
            {
                // Get UPRIGHT point for corner (i, j - 1)
                pos = GetDrawPos(i, j - 1, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHTPOINT, CR.scale);
                tex = CR.texs["U_D_R_Point"];
            }
            else if (corner == HexagonGrid<CenterData, EdgeData, CornerData>.Corners.RIGHT)
            {

                // Get UPLEFT point for corner (i + 1, j)
                pos = GetDrawPos(i + 1, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPLEFTPOINT, CR.scale);
                tex = CR.texs["U_D_L_Point"];
            }
            // Default to left
            else
            {
                // Get UPRIGHT point for corner (i - 1, j - 1)
                pos = GetDrawPos(i - 1, j - 1, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHTPOINT, CR.scale);
                tex = CR.texs["U_D_R_Point"];
            }

            // Size of tile
            Point size = new Point((int)(CR.tileSize * CR.scale), (int)(CR.tileSize * CR.scale));
            Rectangle bound = new Rectangle(pos, size);

            CR._spriteBatch.Draw(tex, bound, c);
        }

        public void DrawEdge(int i, int j, Point start, HexagonGrid<CenterData, EdgeData, CornerData>.Edges edge, Color c)
        {
            Point pos;
            Texture2D tex;
            // Determine type of corner
            if (edge == HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UPLEFT)
            {
                pos = GetDrawPos(i, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPLEFTEDGE, CR.scale);
                tex = CR.texs["U_L"];
            }
            else if (edge == HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UPRIGHT)
            {
                pos = GetDrawPos(i, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHTEDGE, CR.scale);
                tex = CR.texs["U_R"];
            }
            else if (edge == HexagonGrid<CenterData, EdgeData, CornerData>.Edges.DOWNLEFT)
            {
                pos = GetDrawPos(i - 1, j - 1, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHTEDGE, CR.scale);
                tex = CR.texs["U_R"];
            }
            else if (edge == HexagonGrid<CenterData, EdgeData, CornerData>.Edges.DOWNRIGHT)
            {
                pos = GetDrawPos(i + 1, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPLEFTEDGE, CR.scale);
                tex = CR.texs["U_L"];
            }
            else if (edge == HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UP)
            {
                pos = GetDrawPos(i, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPEDGE, CR.scale);
                tex = CR.texs["U_D"];
            }
            // Default to down
            else
            {
                pos = GetDrawPos(i, j - 1, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPEDGE, CR.scale);
                tex = CR.texs["U_D"];
            }

            // Size of tile
            Point size = new Point((int)(CR.tileSize * CR.scale), (int)(CR.tileSize * CR.scale));
            Rectangle bound = new Rectangle(pos, size);

            CR._spriteBatch.Draw(tex, bound, c);
        }

        public void DrawPlayers(int i, int j, Point start)
        {
            /**/
            return;
            /** /
            CenterData cd = board.GetAt(i, j);

            for (int ii = 0; ii < cd.next; ii++)
            {
                DrawPlayer(i, j, start, colors[cd.players[ii]], ii);
            }
            /**/
        }


            /*  Drawing Logic   */
        public void DrawPlayer(int i, int j, Point start, Color c, int playerIndex)
        {
            Point pos = GetDrawPos(i, j, start);
            // Size of tile
            Point size = new Point((int)(CR.tileSize * CR.scale), (int)(CR.tileSize * CR.scale));
            Rectangle bound = new Rectangle(pos, size);

            Texture2D tex = CR.texs["Player" + (playerIndex + 1)];

            CR._spriteBatch.Draw(tex, bound, c);
        }

        public void DrawToken(int i, int j, Point start, Color c)
        {
            Point pos = GetDrawPos(i, j, start);
            // Size of tile
            //Point size = new Point((int)(CR.tileSize * CR.scale), (int)(CR.tileSize * CR.scale / 1.75f));
            Point size = new Point((int)(CR.tileSize * CR.scale), (int)(CR.tileSize * CR.scale));
            Rectangle bound = new Rectangle(pos, size);

            CenterData cd = board.GetAt(i, j);
            if (cd.number != -1)
            {
                string texName = "numbers_warped/" + ((cd.number == 0) ? "token" : "" + cd.number);
                Texture2D tex = CR.texs[texName];

                CR._spriteBatch.Draw(tex, bound, c);
            }
        }

        public void DrawHex(Texture2D hex, int i, int j, Point start, Color c)
        {
            Point pos = GetDrawPos(i, j, start);
            Point size = new Point((int)(CR.tileSize * CR.scale), (int)(CR.tileSize * CR.scale));
            Rectangle bound = new Rectangle(pos, size);

            CR._spriteBatch.Draw(hex, bound, c);
        }

        // Get position hex will be drawn
        public static Point GetDrawPos(int i, int j, Point start)
        {
            int[] URD = board.GetURD(i, j);
            return Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHT, URD[0] * CR.scale) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.DOWN, URD[1] * CR.scale) + start;
        }

        public static Point Multiply(Point p, int c)
        {
            return new Point(p.X * c, p.Y * c);
        }

        public static Point Multiply(Point p, double c)
        {
            return new Point((int)(p.X * c), (int)(p.Y * c));
        }

        /*  Helper Functions    */

        /**/
        public void ResetBoard()
        {
            CR.seed++;
            RNG.seed = CR.seed;

            // Fill hex with 0 - 3
            for (int i = 0; i < board.arrSize; i++)
            {
                for (int j = 0; j < board.arrSize; j++)
                {
                    if (board.InRange(i, j))
                    {
                        /**/
                        int val = (int)(ValueNoise.ValueNoise.Noise2D((double)i / 2.5, (double)j / 2.5) * CR.hexArtCount);
                        board.GetAt(i, j).tex = CR.allTexNames[CR.hexArtStart + val];
                        /** /
                        board.GetAt(i, j).tex = CR.allTexNames[4 + (int)(CR.rng.NextDouble() * CR.hexArtCount)];
                        /**/
                    }
                }
            }
        }
        /**/

        /**/
    }
}
