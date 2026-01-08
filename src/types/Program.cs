
using System.Dynamic;

namespace LibCast {
    class Program {
        [STAThread]
        public static void Main() {
            
            Init();
            while (!Raylib.WindowShouldClose()) {
                Screen.LoopStart();
                Game.Loop();
                //Terminal.Draw();
                Screen.Draw();
                Screen.LoopEnd();
            }

            Screen.Close();
        }

        public static void Init() {
            Screen.Init();
            Screen.Resize();
            Terminal.Resize();
            Game.Init();
            Raylib.SetExitKey(KeyboardKey.F12);
            Screen.SetFullscreen(false);
        }
    }
}