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
        public const int hexArtStart = 5;
        public static readonly string[] allTexNames = new string[]
        {
            "Player1",
            "Player2",
            "Player3",
            "Player4",
            "PlayerMain",

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

            "cards/Blank",
            "cards/Desert",
            "cards/Forest1",
            "cards/Forest2",
            "cards/Forest3",
            "cards/Mountain1",
            "cards/Mountain2",
            "cards/Mountain3",
            "cards/Field1",
            "cards/Field2",
            "cards/Field3",
            "cards/Farm1",
            "cards/Farm2",
            "cards/Farm3",
            "cards/Mesa1",
            "cards/Mesa2",
            "cards/Mesa3",

            "cards/Chapel",
            "cards/GreatHall",
            "cards/Knight",
            "cards/Library",
            "cards/Market",
            "cards/Monopoly",
            "cards/RoadBuilding",
            "cards/University",
            "cards/YearOfPlenty",

            "gui/CCards",
            "gui/XHex",
            "gui/BBuild",
            "gui/Arrow_U",
            "gui/Arrow_D",
            "gui/Arrow_UR",
            "gui/Arrow_UL",
            "gui/Arrow_DR",
            "gui/Arrow_DL",

            "gui/BuildingContainer",
            "gui/CityContainer",

            "Selection",
            "wp",
            "gui/Cursor",

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
        public static float scale = 6.5f;
        public static int scaleIndex = 2;
        public static Point cameraPos = new Point(377,594);
        public static Point cameraMax;
        public static Point cameraMin;

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
            IsMouseVisible = false;

            currState = new Board();
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth *= 2;
            _graphics.PreferredBackBufferHeight*= 2;
            _graphics.ApplyChanges();

            UpdateCameraMinMax();

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
            //Point btmLft = CR.cameraPos + new Point(0, CR._graphics.PreferredBackBufferHeight);

            // Start point of 0, 0 hex
            //cameraPos = btmLft + new Point((int)(2 * CR.scale * CR.texs["Blank"].Width), -(int)(Math.Ceiling(((hexEdgeSize + 2.5d) * HexagonGrid<CenterData, EdgeData, CornerData>.DOWNRIGHT.Y) * CR.scale)));

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


        // Keyboard information
        public static KeyboardState keyboardState = new KeyboardState();
        public static List<Keys> newKeys = new List<Keys>();
        public static List<Keys> preKeys = new List<Keys>();
        public static List<Keys> accKeys = new List<Keys>();

        // Mouse state reference
        public static MouseState mouseState = new MouseState();

        // Scroll value calculations
        public static int scrollLast = mouseState.ScrollWheelValue;
        public static int scrollNow = mouseState.ScrollWheelValue;
        public static int scrollDelta = 0;

        // Change in mouse and camera when moving around with mouse
        public static Point mouseMoveOrigin = Point.Zero;
        public static Point mouseMoveCameraOrigin = Point.Zero;

        // Change in mouse position each frame
        public static Point mouseDelta = Point.Zero;
        protected override void Update(GameTime gameTime)
        {
            /*  Update keyboard information */
            keyboardState = Keyboard.GetState();
            preKeys = keyboardState.GetPressedKeys().ToList();
            newKeys = preKeys.FindAll(key => !accKeys.Contains(key));
            accKeys = preKeys;

            /*  Update mouse information*/
            mouseState = Mouse.GetState();

            // Get change to scroll value
            scrollLast = scrollNow;
            scrollNow = mouseState.ScrollWheelValue;
            scrollDelta = scrollNow - scrollLast;
            /**/

            if (newKeys.Contains(Keys.Escape))
                Exit();

            currState = currState.Update();

            if (scrollDelta != 0)
            {
                float scaleBefore = scale;

                scale += (scrollDelta > 0 ? 0.5f : -0.5f);
                scale = Math.Max(5.5f, scale);
                scale = Math.Min(10f, scale);

                float scaleAfter = scale;

                float scaleDelta = (scaleAfter - scaleBefore) / scaleBefore;

                UpdateCameraMinMax();
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

                cameraPos.X = Math.Max(Math.Min(cameraPos.X, cameraMax.X), cameraMin.X);
                cameraPos.Y = Math.Max(Math.Min(cameraPos.Y, cameraMax.Y), cameraMin.Y);
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

            /** /
            // Draw square for fps monitoring
            if (square)
                _spriteBatch.Draw(wp, new Rectangle(new Point(30, 30), new Point(30, 30)), Color.Black);
            square = !square;

            /**/

            _spriteBatch.Draw(texs["gui/Cursor"], new Rectangle(mouseState.Position, new Point(32, 32)), Color.BlueViolet);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        /**/

        public static void UpdateCameraMinMax()
        {
            Point bound = new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            /** /
            cameraMax = new Point(bound.X / 2 - (int)(tileSize * scale) + (int)(HexagonGrid<int, int, int>.UPRIGHT.X * Board.board.arrSize * scale), (int)(bound.Y / 2 + HexagonGrid<int, int, int>.DOWNLEFT.Y * Board.board.arrSize * scale * 2));
            cameraMin = new Point(-bound.X / 2 + (int)(tileSize * scale * 2), -(int)(bound.Y / 2 + HexagonGrid<int, int, int>.UPRIGHT.Y * Board.board.arrSize * scale) + (int)(tileSize * scale));
            /**/

            Point middle = Board.Multiply(HexagonGrid<int, int, int>.UPRIGHT, 4 * scale);

            cameraMax = new Point(middle.X + bound.X / 2 - (int)(tileSize * scale * 2), middle.Y + bound.Y * 5 / 4);
            cameraMin = new Point(middle.X - bound.X / 2 + (int)(tileSize * scale * 2), middle.Y + bound.Y / 4);
        }
    }
}
