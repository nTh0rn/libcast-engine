using Raylib_cs;
using System.Numerics;

namespace LibCast {
    class Program {
        [STAThread]
        public static void Main() {
            Screen.Init();
            Screen.Resize();
            Terminal.Resize();
            Game.Init();

            while (!Raylib.WindowShouldClose()) {
                Screen.LoopStart();
                Game.Loop();
                //Terminal.Draw();
                Screen.Draw();
                Screen.LoopEnd();
            }

            Screen.Close();
        }
    }
}