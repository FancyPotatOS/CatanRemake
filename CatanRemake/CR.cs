using CatanRemake.HexData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using ValueNoise;

namespace CatanRemake
{
    public class CR : Game
    {
        public static ContentManager CONTENT;

        readonly private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        const int hexEdgeSize = 5;
        public static HexagonGrid<CenterData, EdgeData, CornerData> hex = new HexagonGrid<CenterData, EdgeData, CornerData>(hexEdgeSize);

        public static readonly Color WHITE = new Color(1f, 1f, 1f);
        public static readonly Color RED = new Color(209 / 255f, 40 / 255f, 10 / 255f);
        public static readonly Color BLUE = new Color(10 / 255f, 40 / 255f, 209 / 255f);
        public static readonly Color GREEN = new Color(17 / 255f, 245 / 255f, 51 / 255f);
        public static readonly Color YELLOW = new Color(245 / 255f, 245 / 255f, 17 / 255f);
        public static readonly Color ORANGE = new Color(237 / 255f, 167 / 255f, 17 / 255f);
        //public static readonly Color BROWN = new Color(89 / 255f, 63 / 255f, 29 / 255f);
        public static readonly Color PURPLE = new Color(119 / 255f, 29 / 255f, 209 / 255f);
        public static readonly Color CYAN = new Color(27 / 255f, 242 / 255f, 224 / 255f);
        public static readonly Color PINK = new Color(240 / 255f, 101 / 255f, 205 / 255f);

        public static readonly Color[] colors = { Color.Transparent, WHITE, RED, BLUE, GREEN, YELLOW, ORANGE, /** /BROWN,/**/ PURPLE, CYAN, PINK };

        public static readonly Dictionary<string, Texture2D> texs = new Dictionary<string, Texture2D>();

        public const int hexArtCount = 17;
        public const int hexArtStart = 4;
        static readonly string[] allTexNames = new string[]
        {
            "Player1",
            "Player2",
            "Player3",
            "Player4",

            "Blank",
            "Desert",
            "Forest1",
            "Forest2",
            "Forest3",
            "Mountain1",
            "Mountain2",
            "Mountain3",
            "Field1",
            "Field2",
            "Field3",
            "Farm1",
            "Farm2",
            "Farm3",
            "Mesa1",
            "Mesa2",
            "Mesa3",

            "U_D",
            "U_L",
            "U_R",
            "U_D_L_Point",
            "U_D_R_Point",
            "wp",

            "numbers_dot/token",
            "numbers_dot/1",
            "numbers_dot/2",
            "numbers_dot/3",
            "numbers_dot/4",
            "numbers_dot/5",
            "numbers_dot/6",
            "numbers_dot/7",
            "numbers_dot/8",
            "numbers_dot/9",
            "numbers_dot/10",
            "numbers_dot/11",
            "numbers_dot/12",

            "numbers_special/token",
            "numbers_special/1",
            "numbers_special/2",
            "numbers_special/3",
            "numbers_special/4",
            "numbers_special/5",
            "numbers_special/6",
            "numbers_special/7",
            "numbers_special/8",
            "numbers_special/9",
            "numbers_special/10",
            "numbers_special/11",
            "numbers_special/12",

            "numbers_warped/token",
            "numbers_warped/1",
            "numbers_warped/2",
            "numbers_warped/3",
            "numbers_warped/4",
            "numbers_warped/5",
            "numbers_warped/6",
            "numbers_warped/7",
            "numbers_warped/8",
            "numbers_warped/9",
            "numbers_warped/10",
            "numbers_warped/11",
            "numbers_warped/12"
        };

        Texture2D[] players;

        Texture2D blank;

        Texture2D U_D;
        Texture2D U_L;
        Texture2D U_R;

        Texture2D U_D_L_Point;
        Texture2D U_D_R_Point;

        Texture2D wp;

        const int scale = 4;
        const int tileSize = 16;

        long seed;

        public static readonly Random rng = new Random();

        bool square = false;

        public CR()
        {

           _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "CONTENT";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            CONTENT = Content;

            foreach (string name in allTexNames)
            {
                texs.Add(name, CONTENT.Load<Texture2D>(name));
            }

            players = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                players[i] = texs["Player" + (i + 1)];
            
            blank = CONTENT.Load<Texture2D>("Blank");

            U_D = texs["U_D"];
            U_L = texs["U_L"];
            U_R = texs["U_R"];

            U_D_L_Point = texs["U_D_L_Point"];
            U_D_R_Point = texs["U_D_R_Point"];

            wp = texs["wp"];

            /*  Initialize hex data */
            for (int j = hex.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < hex.arrSize; i++)
                {
                    if (hex.InRange(i, j))
                    {
                        // Draw from code
                        hex.SetAt(new CenterData(), i, j);
                    }
                }
            }
            ResetBoard();
            /**/

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }


        public static List<Keys> newKeys = new List<Keys>();
        public static List<Keys> preKeys = new List<Keys>();
        public static List<Keys> accKeys = new List<Keys>();
        protected override void Update(GameTime gameTime)
        {

            preKeys = Keyboard.GetState().GetPressedKeys().ToList();
            newKeys = preKeys.FindAll(key => !accKeys.Contains(key));
            accKeys = preKeys;

            if (newKeys.Contains(Keys.Q))
                Exit();

            if (newKeys.Contains(Keys.Y))
            {
                ResetBoard();
            }
            if (newKeys.Contains(Keys.U))
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

                for (int i = 0; i < hex.arrSize; i++)
                {
                    for (int j = hex.range[i][0]; j <= hex.range[i][1]; j++)
                    {

                        // Choose a random direction
                        HexagonGrid<CenterData, EdgeData, CornerData>.Corners randDir = dir[(int)(rng.NextDouble() * dir.Length)];

                        // Random color in colors
                        CornerData cd = new CornerData
                        {
                            hasSettlement = true,
                            playerID = (int)(rng.NextDouble() * (colors.Length - 1) + 1)
                        };
                        hex.SetAtCorner(cd, i, j, randDir);
                    }
                }
            }
            if (newKeys.Contains(Keys.I))
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

                for (int i = 0; i < hex.arrSize; i++)
                {
                    for (int j = hex.range[i][0]; j <= hex.range[i][1]; j++)
                    {

                        // Random color in colors
                        EdgeData ed; ed.roadID = (int)(rng.NextDouble() * (colors.Length - 1) + 1);
                        HexagonGrid<CenterData, EdgeData, CornerData>.Edges randDir = dir[(int)(rng.NextDouble() * dir.Length)];
                        hex.SetAtEdge(ed, i, j, randDir);
                    }
                }
            }
            if (newKeys.Contains(Keys.O))
            {
                for (int i = 0; i < hex.arrSize; i++)
                {
                    for (int j = hex.range[i][0]; j <= hex.range[i][1]; j++)
                    {
                        if (rng.NextDouble() > 0.80)
                        {
                            int rangeStart = 2;
                            int rangeSize= 10;

                            hex.GetAt(i, j).number = (int)(rng.NextDouble() * rangeSize + rangeStart);
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            // Bottom left of screen
            Point btmLft = new Point(0, _graphics.PreferredBackBufferHeight);

            // Start point of 0, 0 hex
            Point start = btmLft + new Point(2 * scale * blank.Width, -(int)(Math.Ceiling(((hexEdgeSize + 2.5d) * HexagonGrid<CenterData, EdgeData, CornerData>.DOWNRIGHT.Y) * scale)));

            /*  Draw every hexagon   */
            for (int j = hex.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < hex.arrSize; i++)
                {
                    if (hex.InRange(i, j))
                    {
                        // Draw from code
                        DrawHex(texs[hex.GetAt(i, j).tex], i, j, start, Color.White);
                    }
                }
            }

            /*  Draw every token    */
            for (int j = hex.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < hex.arrSize; i++)
                {
                    if (hex.InRange(i, j))
                    {
                        DrawToken(i, j, start, Color.White);
                    }
                }
            }

            /**/

            /*  Draw every Edge  */
            for (int j = hex.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < hex.arrSize; i++)
                {
                    if (hex.InRange(i, j))
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
                            int val = hex.GetAtEdge(i, j, dir[y]).roadID;
                            if (val == 0)
                                continue;
                            else
                                DrawEdge(i, j, start, dir[y], colors[val]);
                        }
                    }
                }
            }

            /*  Draw every corner    */
            for (int j = hex.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < hex.arrSize; i++)
                {
                    if (hex.InRange(i, j))
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
                            CornerData corner = hex.GetAtCorner(i, j, dir[y]);
                            if (!corner.hasSettlement)
                                continue;
                            else
                                DrawCorner(i, j, start, dir[y], colors[corner.playerID]);
                        }
                    }
                }
            }

            /*  Draw every player    * /
            for (int j = hex.arrSize - 1; -1 < j; j--)
            {
                for (int i = 0; i < hex.arrSize; i++)
                {
                    if (hex.InRange(i, j))
                    {
                        CenterData cd = hex.GetAt(i, j);

                        foreach (int id in cd.players)
                            DrawPlayers(i, j, start);
                    }
                }
            }

            /**/

            // Draw square for fps monitoring
            if (square)
                _spriteBatch.Draw(wp, new Rectangle(new Point(30, 30), new Point(30, 30)), Color.Black);
            square = !square;

            _spriteBatch.End();

            base.Draw(gameTime);
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
                pos = GetDrawPos(i, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPLEFTPOINT, scale);
                tex = U_D_L_Point;
            }
            else if (corner == HexagonGrid<CenterData, EdgeData, CornerData>.Corners.UPRIGHT)
            {
                // Get UPRIGHT point for corner
                pos = GetDrawPos(i, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHTPOINT, scale);
                tex = U_D_R_Point;
            }
            else if (corner == HexagonGrid<CenterData, EdgeData, CornerData>.Corners.DOWNLEFT)
            {
                // Get UPLEFT point for corner (i, j - 1)
                pos = GetDrawPos(i, j - 1, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPLEFTPOINT, scale);
                tex = U_D_L_Point;
            }
            else if (corner == HexagonGrid<CenterData, EdgeData, CornerData>.Corners.DOWNRIGHT)
            {
                // Get UPRIGHT point for corner (i, j - 1)
                pos = GetDrawPos(i, j - 1, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHTPOINT, scale);
                tex = U_D_R_Point;
            }
            else if (corner == HexagonGrid<CenterData, EdgeData, CornerData>.Corners.RIGHT)
            {

                // Get UPLEFT point for corner (i + 1, j)
                pos = GetDrawPos(i + 1, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPLEFTPOINT, scale);
                tex = U_D_L_Point;
            }
            // Default to left
            else
            {
                // Get UPRIGHT point for corner (i - 1, j - 1)
                pos = GetDrawPos(i - 1, j - 1, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHTPOINT, scale);
                tex = U_D_R_Point;
            }

            // Size of tile
            Point size = new Point(tileSize * scale, tileSize * scale);
            Rectangle bound = new Rectangle(pos, size);

            _spriteBatch.Draw(tex, bound, c);
        }

        public void DrawEdge(int i, int j, Point start, HexagonGrid<CenterData, EdgeData, CornerData>.Edges edge, Color c)
        {
            Point pos;
            Texture2D tex;
            // Determine type of corner
            if (edge == HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UPLEFT)
            {
                pos = GetDrawPos(i, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPLEFTEDGE, scale);
                tex = U_L;
            }
            else if (edge == HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UPRIGHT)
            {
                pos = GetDrawPos(i, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHTEDGE, scale);
                tex = U_R;
            }
            else if (edge == HexagonGrid<CenterData, EdgeData, CornerData>.Edges.DOWNLEFT)
            {
                pos = GetDrawPos(i - 1, j - 1, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHTEDGE, scale);
                tex = U_R;
            }
            else if (edge == HexagonGrid<CenterData, EdgeData, CornerData>.Edges.DOWNRIGHT)
            {
                pos = GetDrawPos(i + 1, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPLEFTEDGE, scale);
                tex = U_L;
            }
            else if (edge == HexagonGrid<CenterData, EdgeData, CornerData>.Edges.UP)
            {
                pos = GetDrawPos(i, j, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPEDGE, scale);
                tex = U_D;
            }
            // Default to down
            else
            {
                pos = GetDrawPos(i, j - 1, start) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPEDGE, scale);
                tex = U_D;
            }

            // Size of tile
            Point size = new Point(tileSize * scale, tileSize * scale);
            Rectangle bound = new Rectangle(pos, size);

            _spriteBatch.Draw(tex, bound, c);
        }

        public void DrawPlayers(int i, int j, Point start)
        {
            /**/
            return;
            /** /
            CenterData cd = hex.GetAt(i, j);

            for (int ii = 0; ii < cd.next; ii++)
            {
                DrawPlayer(i, j, start, colors[cd.players[ii]], ii);
            }
            /**/
        }

        public void DrawPlayer(int i, int j, Point start, Color c, int playerIndex)
        {
            Point pos = GetDrawPos(i, j, start);
            // Size of tile
            Point size = new Point(tileSize * scale, tileSize * scale);
            Rectangle bound = new Rectangle(pos, size);

            Texture2D tex = players[playerIndex];

            _spriteBatch.Draw(tex, bound, c);
        }

        public void DrawToken(int i, int j, Point start, Color c)
        {
            Point pos = GetDrawPos(i, j, start);
            // Size of tile
            Point size = new Point(tileSize * scale, (int)(tileSize * scale / 1.75f));
            Rectangle bound = new Rectangle(pos, size);

            CenterData cd = hex.GetAt(i, j);
            if (cd.number != -1)
            {
                string texName = "numbers_dot/" + ((cd.number == 0) ? "token" : "" + cd.number);
                Texture2D tex = texs[texName];

                _spriteBatch.Draw(tex, bound, c);
            }
        }

        public void DrawHex(Texture2D hex, int i, int j, Point start, Color c)
        {
            Point pos = GetDrawPos(i, j, start);
            Point size = new Point(tileSize * scale, tileSize * scale);
            Rectangle bound = new Rectangle(pos, size);

            _spriteBatch.Draw(hex, bound, c);
        }

        // Get position hex will be drawn
        public static Point GetDrawPos(int i, int j, Point start)
        {
            int[] URD = hex.GetURD(i, j);
            return Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.UPRIGHT, URD[0] * scale) + Multiply(HexagonGrid<CenterData, EdgeData, CornerData>.DOWN, URD[1] * scale) + start;
        }

        public void ResetBoard()
        {
            seed++;
            RNG.seed = seed;

            // Fill hex with 0 - 3
            for (int i = 0; i < hex.arrSize; i++)
            {
                for (int j = 0; j < hex.arrSize; j++)
                {
                    if (hex.InRange(i, j))
                    {
                        /** /
                        int val = (int)(ValueNoise.ValueNoise.Noise2D((double)i / 2.5, (double)j / 2.5) * tiles.Length);
                        hex.GetAt(i, j).tileID = val;
                        /**/
                        hex.GetAt(i, j).tex = allTexNames[4 + (int)(rng.NextDouble() * hexArtCount)];
                        /**/
                    }
                }
            }
        }

        public static Point Multiply(Point p, int c)
        {
            return new Point(p.X * c, p.Y * c);
        }

        public static Point Multiply(Point p, double c)
        {
            return new Point((int)(p.X * c), (int)(p.Y * c));
        }
    }
}
