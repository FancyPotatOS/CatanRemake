using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatanRemake.States
{
    class BuildingState : IState
    {
        Board savedBoard;

        public BuildingState(Board board)
        {
            savedBoard = board;
        }

        public IState Update()
        {
            if (CR.newKeys.Contains(Keys.C))
            {
                Board.SetSelected("cards");
                return savedBoard;
            }
            else if (CR.newKeys.Contains(Keys.X))
            {
                Board.SetSelected("hex");
                return savedBoard;
            }


            return this;
        }

        public static readonly Point containerSize = new Point(512, 512);
        public static Rectangle containerBound = new Rectangle(new Point(CR._graphics.PreferredBackBufferWidth / 2 - containerSize.X / 2, CR._graphics.PreferredBackBufferHeight / 2 - containerSize.Y / 2), containerSize);
        public void Draw()
        {
            savedBoard.Draw();

            DrawGUIBackground();

        }

        public void DrawGUIBackground()
        {
            if (CR._graphics.PreferredBackBufferWidth / 2 != containerBound.X)
                containerBound = new Rectangle(new Point(CR._graphics.PreferredBackBufferWidth / 2 - containerSize.X / 2, CR._graphics.PreferredBackBufferHeight / 2 - containerSize.Y / 2), containerSize);

            CR._spriteBatch.Draw(CR.texs["gui/BuildingContainer"], containerBound, Color.White);
        }
    }
}
