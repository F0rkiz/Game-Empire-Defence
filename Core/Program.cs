using System;
using Empire_Defence.Core;

namespace Empire_Defence
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Game1();
            game.Run();
        }
    }
}
