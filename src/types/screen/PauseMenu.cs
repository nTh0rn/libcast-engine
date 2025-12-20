

namespace LibCast {
    using static Screen;
    using static UI;
    public static class PauseMenu {
        public static int[] gameDimensions = {128, 72};
        public static void Draw() {
            //DrawBackground(0);
            Random r = new Random();
            for(int i = 0; i < 100; i++) {
                Screen.Fill(r.Next(255));
                DrawPixel(r.Next(gameWidth), r.Next(gameHeight));
            }
            DrawText("What the fuckk", 10, 10);

            if(Raylib.IsCursorHidden()) {
                Raylib.ShowCursor();
            }
            switch(Raylib.GetKeyPressed()) {
                case (int)KeyboardKey.Down:
                    settingsState = (SettingsState)((((int)settingsState)+1) % settingsStateLength);
                    Console.WriteLine("Down");
                    SetPixelScale(10);
                    ApplyScreenSettings(1280, 720, 1280, 720, 128, 72, false);
                    break;
                case (int)KeyboardKey.Up:
                    settingsState = (SettingsState)((((int)settingsState)-1) % settingsStateLength);
                    Console.WriteLine("Up");
                    SetPixelScale(5);
                    ApplyScreenSettings(1920, 1080, 1280, 720, 256, 144, false);
                    break;
                case (int)KeyboardKey.Escape:
                    gameState = GameState.PLAY;
                    UI.PlayApplySizing();
                    return;
            }


        }

        public static void ApplySizing() {
            SetPixelScale(10);
            ApplyBuffer(1280, 720);
            SetGameSize(128, 72);
        }
    }
}