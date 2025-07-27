using Raylib_cs;
using System.Numerics;

namespace LibCast {
    class Program {
        [STAThread]
        public static void Main() {
            Screen.Init();

            while (!Raylib.WindowShouldClose()) {
                Screen.LoopInit();

                Raylib.ClearBackground(Color.White);
                Raylib.DrawTextEx(Screen.terminalFont, "Hello, world!", new Vector2(12, 12), 32, 0, Color.Black);

                Screen.LoopClose();
            }

            Screen.Close();
        }
    }
}