using CatanRemake.HexData;
using CatanRemake.States;
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
        /*  Game Classes   */
        public static ContentManager CONTENT;

        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;

        /*  Constants   */
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

        /*  Drawing Data*/
        public static readonly Dictionary<string, Texture2D> texs = new Dictionary<string, Texture2D>();

        public const int hexArtCount = 17;
        public const int hexArtStart = 4;
        public static readonly string[] allTexNames = new string[]
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
        Texture2D wp;

        /*  Camera Data */
        public const int tileSize = 16;
        public const float scrollSensitivity = 0.008695f;
        public static float scale = 4;
        public static int scaleIndex = 2;
        public static Point cameraPos = Point.Zero;

        /*  RNG Data   */
        public static long seed;
        public static readonly Random rng = new Random();

        /*  State Constants */
        IState currState;
        public const int hexEdgeSize = 5;

        public CR()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "CONTENT";
            IsMouseVisible = true;

            currState = new Board();
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

            wp = texs["wp"];

            // Bottom left of screen
            Point btmLft = CR.cameraPos + new Point(0, CR._graphics.PreferredBackBufferHeight);

            // Start point of 0, 0 hex
            cameraPos = btmLft + new Point((int)(2 * CR.scale * CR.texs["Blank"].Width), -(int)(Math.Ceiling(((hexEdgeSize + 2.5d) * HexagonGrid<CenterData, EdgeData, CornerData>.DOWNRIGHT.Y) * CR.scale)));

            /*  Initialize hex data * /
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


        public static KeyboardState keyboardState = new KeyboardState();
        public static List<Keys> newKeys = new List<Keys>();
        public static List<Keys> preKeys = new List<Keys>();
        public static List<Keys> accKeys = new List<Keys>();
        public static MouseState mouseState = new MouseState();
        public static int scrollStart = mouseState.ScrollWheelValue;
        public static int scrollLast = mouseState.ScrollWheelValue;
        public static int scrollNow = mouseState.ScrollWheelValue;
        public static int scrollDelta = 0;
        public static Point mouseMoveOrigin = Point.Zero;
        public static Point mouseMoveCameraOrigin = Point.Zero;
        protected override void Update(GameTime gameTime)
        {
            /*  Update keyboard information */
            keyboardState = Keyboard.GetState();
            preKeys = keyboardState.GetPressedKeys().ToList();
            newKeys = preKeys.FindAll(key => !accKeys.Contains(key));
            accKeys = preKeys;

            /*  Update mouse information*/
            mouseState = Mouse.GetState();

            scrollLast = scrollNow;
            scrollNow = mouseState.ScrollWheelValue;
            scrollDelta = scrollNow - scrollLast;
            /**/

            if (newKeys.Contains(Keys.Q))
                Exit();

            currState.Update();

            if (preKeys.Contains(Keys.LeftShift))
            {
                if (preKeys.Contains(Keys.W))
                {
                    cameraPos.Y += 2;
                }
                else if (preKeys.Contains(Keys.S))
                {
                    cameraPos.Y -= 2;
                }
                if (preKeys.Contains(Keys.A))
                {
                    cameraPos.X += 2;
                }
                else if (preKeys.Contains(Keys.D))
                {
                    cameraPos.X -= 2;
                }
            }

            if (scrollDelta != 0)
            {
                float scaleBefore = scale;

                scale += (scrollDelta > 0 ? 0.5f : -0.5f);
                scale = Math.Max(0.5f, scale);
                scale = Math.Min(10f, scale);

                float scaleAfter = scale;

                float scaleDelta = (scaleAfter - scaleBefore) / scaleBefore;
            }

            // If middle button pressed
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                // If origin not set
                if (mouseMoveOrigin == Point.Zero)
                {
                    // Store origin point
                    mouseMoveOrigin = mouseState.Position;
                    
                    // Save camera before moved
                    mouseMoveCameraOrigin = cameraPos;
                }

                Point mouseDelta = (mouseMoveOrigin - mouseState.Position);
                cameraPos = mouseMoveCameraOrigin - mouseDelta;
            }
            else
            {
                mouseMoveOrigin = Point.Zero;
            }

            /** /
            if (newKeys.Contains(Keys.Y))
            {
                ResetBoard();
            }
            /** /
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
            /** /
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
            /** /
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
            /**/

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            currState.Draw();

            _spriteBatch.Draw(blank, new Rectangle(new Point(-16, -16) + cameraPos, new Point(32, 32)), Color.White);

            /** /
            // Draw square for fps monitoring
            if (square)
                _spriteBatch.Draw(wp, new Rectangle(new Point(30, 30), new Point(30, 30)), Color.Black);
            square = !square;

            /**/

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        /**/
    }
}
