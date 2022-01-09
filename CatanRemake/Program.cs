using System;

namespace CatanRemake
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new CR();
            game.Run();
        }
    }
}
