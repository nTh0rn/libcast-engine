using Raylib_cs;
using System.Numerics;

namespace LibCast {
    class Program {
        [STAThread]
        public static void Main() {
            Screen.Init();
            Screen.Resize();
            Terminal.Resize();

            while (!Raylib.WindowShouldClose()) {
                Screen.LoopInit();

                // Raylib.ClearBackground(Color.White);
                
                
                //Console.WriteLine("Yuppers" + Screen.deltaTime);
                //Screen.DrawRect(2, 2, 10, 10, Color.Green);

                Screen.Draw();
                Terminal.Draw();
                Raylib.DrawTextEx(Screen.terminalFont, "Hello, world!", new Vector2(2, 2), 20, 0, Color.Black);
                Screen.LoopClose();
            }

            Screen.Close();
        }
    }
}