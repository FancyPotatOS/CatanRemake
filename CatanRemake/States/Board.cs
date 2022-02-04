using CatanRemake.HexData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using ValueNoise;


namespace CatanRemake.States
{
    class Board : IState
    {
        public static HexagonGrid<CenterData, EdgeData, CornerData> board;

        // Whose turn it is based on index
        public static int turn;
        // String of what is selected right now
        public static string selected;

        // Index of hex that is being hovered over
        public static int[] hovered;
        // Index of card that is selected
        public static int cardSelect;
        readonly List<string> cards = new List<string> { 
            "cards/Mountain1",
            "cards/Mesa2",
            "cards/Farm2",
            "cards/Field3"
        };

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
                        CenterData cd = new CenterData();
                        cd.tex = CR.allTexNames[(int)(CR.rng.NextDouble() * CR.hexArtCount) + CR.hexArtStart];
                        if (cd.tex != "Desert")
                            cd.number = (int)(CR.rng.NextDouble() * 11 + 2);
                        // Draw from code
                        board.SetAt(cd, i, j);
                    }
                }
            }
            /**/

            hovered = new int[] { 0, 0 };
            cardSelect = 0;

            CenterData.robberIndex[0] = 4;
            CenterData.robberIndex[1] = 4;

            selected = "hex";
        }

        public static bool newClick = true;
        public IState Update()
        {
            IState shouldReturn = this;

            /** /
            if (CR.newKeys.Contains(Keys.Y))
            {
                ResetBoard();
            }
            /** /
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
            /** /
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
            /** /
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

            /*  Get currently selected hex  */
            if (CR.preKeys.Contains(Keys.LeftShift))
            {
                if (CR.preKeys.Contains(Keys.W))
                {
                    CR.cameraPos.Y += (int)CR.scale + 1;
                }
                else if (CR.preKeys.Contains(Keys.S))
                {
                    CR.cameraPos.Y -= (int)CR.scale + 1;
                }
                if (CR.preKeys.Contains(Keys.A))
                {
                    CR.cameraPos.X += (int)CR.scale + 1;
                }
                else if (CR.preKeys.Contains(Keys.D))
                {
                    CR.cameraPos.X -= (int)CR.scale + 1;
                }
                if (CR.newKeys.Contains(Keys.Q))
                {
                    CR.scale += 0.5f;
                    CR.scale = Math.Max(5.5f, CR.scale);
                    CR.scale = Math.Min(10f, CR.scale);

                    CR.UpdateCameraMinMax();
                }
                else if (CR.newKeys.Contains(Keys.E))
                {
                    CR.scale += -0.5f;
                    CR.scale = Math.Max(5.5f, CR.scale);
                    CR.scale = Math.Min(10f, CR.scale);

                    CR.UpdateCameraMinMax();
                }
            }
            else
            {
                // Controller keys
                if (CR.newKeys.Contains(Keys.C))
                    selected = "cards";
                if (CR.newKeys.Contains(Keys.B))
                {
                    selected = "build";
                    shouldReturn = new BuildingState(this);
                }
                if (CR.newKeys.Contains(Keys.X))
                    selected = "hex";

                else if (selected == "cards")
                {
                    if (CR.newKeys.Contains(Keys.A))
                    {
                        cardSelect -= cardSelect == 0 ? 0 : 1;

                    }
                    else if (CR.newKeys.Contains(Keys.D))
                    {
                        cardSelect += cardSelect == cards.Count - 1 ? 0 : 1;
                    }
                }
                else if (selected == "hex")
                {
                    if (CR.newKeys.Contains(Keys.Q))
                        if (board.InRange(hovered[0] - 1, hovered[1]))
                            hovered[0]--;
                    if (CR.newKeys.Contains(Keys.D))
                        if (board.InRange(hovered[0] + 1, hovered[1]))
                            hovered[0]++;
                    if (CR.newKeys.Contains(Keys.S))
                        if (board.InRange(hovered[0], hovered[1] - 1))
                            hovered[1]--;
                    if (CR.newKeys.Contains(Keys.W))
                        if (board.InRange(hovered[0], hovered[1] + 1))
                            hovered[1]++;
                    if (CR.newKeys.Contains(Keys.A))
                        if (board.InRange(hovered[0] - 1, hovered[1] - 1))
                        { hovered[0]--; hovered[1]--; }
                    if (CR.newKeys.Contains(Keys.E))
                        if (board.InRange(hovered[0] + 1, hovered[1] + 1))
                        { hovered[0]++; hovered[1]++; }
                }
            }

            // If clicked
            if (CR.mouseState.LeftButton == ButtonState.Pressed)
            {
                UpdateGUISelection();

                newClick = false;
            }
            else
                newClick = true;

            /**/

            return shouldReturn;
        }

        /*  Update Logic    */
        public void UpdateGUISelection()
        {
            /*  Check if state change is selected*/
            Point mousePos = CR.mouseState.Position;

            Point topMiddle = new Point(-totalWidth / 2 + CR._graphics.PreferredBackBufferWidth / 2, 0);

            for (int i = 0; i < sm.Length && CR.mouseState.Y < cardSize.Y; i++)
            {
                Point offset = new Point((int)(i * offsetI), 0);
                Point curr = topMiddle + offset;

                if (curr.X <= mousePos.X && mousePos.X < curr.X + cardSize.X)
                {
                    selected = accState[i];
                }
            }

            /*  Check if hex selection is selected  */
            if (newClick && HCGUIUSelect.Contains(mousePos) && selected == "hex")
            {
                HCGUIUPressCount = 0;
                if (board.InRange(hovered[0], hovered[1] + 1))
                    hovered[1]++;
            }
            else if (newClick && HCGUIURSelect.Contains(mousePos) && selected == "hex")
            {
                HCGUIURPressCount = 0;
                if (board.InRange(hovered[0] + 1, hovered[1] + 1))
                { hovered[0]++; hovered[1]++; }
            }
            else if (newClick && HCGUIULSelect.Contains(mousePos) && selected == "hex")
            {
                HCGUIULPressCount = 0;
                if (board.InRange(hovered[0] - 1, hovered[1]))
                    hovered[0]--;
            }
            else if (newClick && HCGUIDRSelect.Contains(mousePos) && (selected == "hex" || selected == "cards"))
            {
                HCGUIDRPressCount = 0;
                if (selected == "hex" && board.InRange(hovered[0] + 1, hovered[1]))
                    hovered[0]++;
                else
                    cardSelect += cardSelect == cards.Count - 1 ? 0 : 1;

            }
            else if (newClick && HCGUIDLSelect.Contains(mousePos) && (selected == "hex" || selected == "cards"))
            {
                HCGUIDLPressCount = 0;
                if (selected == "hex" && board.InRange(hovered[0] - 1, hovered[1] - 1))
                { hovered[0]--; hovered[1]--; }
                else
                    cardSelect -= cardSelect == 0 ? 0 : 1;
            }
            else if (newClick && HCGUIDSelect.Contains(mousePos) && selected == "hex")
            {
                HCGUIDPressCount = 0;
                if (board.InRange(hovered[0], hovered[1] - 1))
                    hovered[1]--;
            }
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

            /*  Draw the robber  */
            DrawRobber(CR.cameraPos);

            /*  Draw selection border over the hex  */

            DrawHex(CR.texs["Selection"], hovered[0], hovered[1], CR.cameraPos, (selected != "hex" ? Color.DarkGray : Color.White));

            /*  Draw the cards  */
            int index = 0;
            foreach (string card in cards)
            {
                DrawCard(card, index);
                index++;
            }

            /*  Draw markers for the state  */
            DrawStateMarkers();

            /*  Draw gui for selecting hex and cards    */
            DrawHexControlGUI();

            /**/
        }


            /*  Drawing Logic   */

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
            i = i + j + start.X;
            j = i + start.Y;
            start.X = i + j;
            return;
            /** /
            CenterData cd = board.GetAt(i, j);

            for (int ii = 0; ii < cd.next; ii++)
            {
                DrawPlayer(i, j, start, colors[cd.players[ii]], ii);
            }
            /**/
        }

        public void DrawRobber(Point start)
        {
            int i = CenterData.robberIndex[0];
            int j = CenterData.robberIndex[1];

            CenterData cd = board.GetAt(i, j);

            DrawPlayer(i, j, start, Color.White, "Main");
        }
        public void DrawPlayer(int i, int j, Point start, Color c, string suffix)
        {
            Point pos = GetDrawPos(i, j, start);
            // Size of tile
            Point size = new Point((int)(CR.tileSize * CR.scale), (int)(CR.tileSize * CR.scale));
            Rectangle bound = new Rectangle(pos, size);

            Texture2D tex = CR.texs["Player" + suffix];

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

        public void DrawCard(string tex, int index)
        {

            Point size = new Point(64, 96);
            size = Multiply(size, 2);
            Point middle = new Point(CR._graphics.PreferredBackBufferWidth / 2, CR._graphics.PreferredBackBufferHeight);

            float printIndex;
            if (selected != "cards")
                printIndex = index;
            else if (index < cardSelect)
                printIndex = index - cardSelect - 1;
            else if (cardSelect < index)
                printIndex = index - cardSelect + 1;
            else
                printIndex = 0;
            Point offset = new Point((int)(printIndex * (size.X) / 2), selected == "cards" ? 0 : size.Y * 3 / 4);

            middle += offset;

            Rectangle bound = new Rectangle(middle - new Point(size.X / 2, size.Y * 3 / 2), size);

            CR._spriteBatch.Draw(CR.texs[tex], bound, Color.White);
        }

        public static readonly string[] sm = { "gui/XHex", "gui/CCards", "gui/BBuild" };
        public static readonly string[] accState = { "hex", "cards", "build" };
        public static readonly Point cardSize = new Point(128, 128);
        public static readonly int totalWidth = cardSize.X * (int)(sm.Length * 1.5f);
        public static readonly float offsetI = cardSize.X * 1.5f;
        public void DrawStateMarkers()
        {
            Point topMiddle = new Point(-totalWidth / 2 + CR._graphics.PreferredBackBufferWidth / 2, 0);     

            for (int i = 0; i < sm.Length; i++)
            {
                Point offset = new Point((int)(i * offsetI), 0);
                Point curr = topMiddle + offset;

                if (selected == accState[i])
                    curr.Y += cardSize.Y / 2;

                Rectangle bound = new Rectangle(curr, cardSize);
                CR._spriteBatch.Draw(CR.texs[sm[i]], bound, Color.White);
            }
        }

        /**/
        public static Point topRight = new Point(CR._graphics.PreferredBackBufferWidth, 0);
        public static readonly int HCGUIScale = 5;
        public static readonly Point HCGUISize = new Point(CR.tileSize * HCGUIScale, CR.tileSize * HCGUIScale);
        public static Point HCGUIMiddle = topRight - new Point(HCGUISize.X * 3, -HCGUISize.Y * 2);
        public static Point HCGUIU = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.UP, HCGUIScale);
        public static Point HCGUIUR = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.UPRIGHT, HCGUIScale);
        public static Point HCGUIUL = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.UPLEFT, HCGUIScale);
        public static Point HCGUIDR = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.DOWNRIGHT, HCGUIScale);
        public static Point HCGUIDL = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.DOWNLEFT, HCGUIScale);
        public static Point HCGUID = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.DOWN, HCGUIScale);
        public static Rectangle HCGUIUSelect = new Rectangle(HCGUIU, new Point(HCGUISize.X, HCGUISize.Y / 2));
        public static Rectangle HCGUIURSelect = new Rectangle(HCGUIUR, new Point(HCGUISize.X, HCGUISize.Y / 2));
        public static Rectangle HCGUIULSelect = new Rectangle(HCGUIUL, new Point(HCGUISize.X, HCGUISize.Y / 2));
        public static Rectangle HCGUIDRSelect = new Rectangle(HCGUIDR, new Point(HCGUISize.X, HCGUISize.Y / 2));
        public static Rectangle HCGUIDLSelect = new Rectangle(HCGUIDL, new Point(HCGUISize.X, HCGUISize.Y / 2));
        public static Rectangle HCGUIDSelect = new Rectangle(HCGUID, new Point(HCGUISize.X, HCGUISize.Y / 2));
        public static int HCGUIUPressCount = 0;
        public static int HCGUIURPressCount = 0;
        public static int HCGUIULPressCount = 0;
        public static int HCGUIDRPressCount = 0;
        public static int HCGUIDLPressCount = 0;
        public static int HCGUIDPressCount = 0;
        /**/
        public void DrawHexControlGUI()
        {
            HCGUIUPressCount++;
            HCGUIURPressCount++;
            HCGUIULPressCount++;
            HCGUIDPressCount++;
            HCGUIDLPressCount++;
            HCGUIDRPressCount++;
            /*  Update values if change in screen   */
            if (topRight.X != CR._graphics.PreferredBackBufferWidth)
            {
                topRight = new Point(CR._graphics.PreferredBackBufferWidth, 0);
                HCGUIMiddle = topRight - new Point(HCGUISize.X * 3, -HCGUISize.Y * 2);
                HCGUIU = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.UP, HCGUIScale);
                HCGUIUR = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.UPRIGHT, HCGUIScale);
                HCGUIUL = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.UPLEFT, HCGUIScale);
                HCGUIDR = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.DOWNRIGHT, HCGUIScale);
                HCGUIDL = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.DOWNLEFT, HCGUIScale);
                HCGUID = HCGUIMiddle + Multiply(HexagonGrid<int, int, int>.DOWN, HCGUIScale);
                HCGUIUSelect = new Rectangle(HCGUIU, new Point(HCGUISize.X, HCGUISize.Y / 2));
                HCGUIURSelect = new Rectangle(HCGUIUR, new Point(HCGUISize.X, HCGUISize.Y / 2));
                HCGUIULSelect = new Rectangle(HCGUIUL, new Point(HCGUISize.X, HCGUISize.Y / 2));
                HCGUIDRSelect = new Rectangle(HCGUIDR, new Point(HCGUISize.X, HCGUISize.Y / 2));
                HCGUIDLSelect = new Rectangle(HCGUIDL, new Point(HCGUISize.X, HCGUISize.Y / 2));
                HCGUIDSelect = new Rectangle(HCGUID, new Point(HCGUISize.X, HCGUISize.Y / 2));
    }
            /**/

            Rectangle bound;
            Point pos;

            /*  U   */
            pos = HCGUIU;
            bound = new Rectangle(pos, HCGUISize);
            CR._spriteBatch.Draw(CR.texs["gui/Arrow_U"], bound, (selected == "cards" || HCGUIUPressCount <= 10 ? Color.Gray : Color.White));
            /*  UR */
            pos = HCGUIUR;
            bound = new Rectangle(pos, HCGUISize);
            CR._spriteBatch.Draw(CR.texs["gui/Arrow_UR"], bound, (selected == "cards" || HCGUIURPressCount <= 10 ? Color.Gray : Color.White));
            /*  UL   */
            pos = HCGUIUL;
            bound = new Rectangle(pos, HCGUISize);
            CR._spriteBatch.Draw(CR.texs["gui/Arrow_UL"], bound, (selected == "cards" || HCGUIULPressCount <= 10 ? Color.Gray : Color.White));
            /*  Middle  */
            bound = new Rectangle(HCGUIMiddle, HCGUISize);
            CR._spriteBatch.Draw(CR.texs["Blank"], bound, Color.White);
            /*  DR   */
            pos = HCGUIDR;
            bound = new Rectangle(pos, HCGUISize);
            CR._spriteBatch.Draw(CR.texs["gui/Arrow_DR"], bound, (HCGUIDRPressCount <= 10 ? Color.Gray : Color.White));
            /*  DL   */
            pos = HCGUIDL;
            bound = new Rectangle(pos, HCGUISize);
            CR._spriteBatch.Draw(CR.texs["gui/Arrow_DL"], bound, (HCGUIDLPressCount <= 10 ? Color.Gray : Color.White));
            /*  D   */
            pos = HCGUID;
            bound = new Rectangle(pos, HCGUISize);
            CR._spriteBatch.Draw(CR.texs["gui/Arrow_D"], bound, (selected == "cards" || HCGUIDPressCount <= 10 ? Color.Gray : Color.White));
            /**/
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
        public bool CanBuild(string thing, List<string> cards)
        {
            /// TODO: Build information
            return true;
        }

        public static void SetSelected(string sel)
        {
            selected = sel;
        }

        /**/
    }
}
