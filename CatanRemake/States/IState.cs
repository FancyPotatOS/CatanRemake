using System;
using System.Collections.Generic;
using System.Text;

namespace CatanRemake.States
{
    interface IState
    {
        public IState Update();

        public void Draw();
    }
}
