

namespace LibCast {
    using static Screen;
    using static UI;

    public enum PauseState {
        RESUME,
        SETTINGS,
        EXIT
    }

    public static class PauseUI {
        public static PauseState pauseState = PauseState.RESUME;

        public static PauseUIPlayButton playButton = new PauseUIPlayButton();
        public static PauseUISettingsButton settingsButton = new PauseUISettingsButton();
        public static PauseUIExitButton exitButton = new PauseUIExitButton();

        public static void Loop() {
            Raycaster.Go(Game.room.player, Game.room, 0, 0, Screen.gameWidth, Screen.gameHeight);
            DrawBackground(0, 0, 0, 100);
            DrawText("Game Paused", 20, 50);
            
            StartCollisionDraw(playButton);
            Fill(pauseState == PauseState.RESUME ? Color.White : Color.Black);
            Stroke(pauseState == PauseState.RESUME ? Color.Black : Color.White);
            DrawText("RESUME", 4, 4, true);
            EndCollisionDraw();

            StartCollisionDraw(settingsButton);
            Fill(pauseState == PauseState.SETTINGS ? Color.White : Color.Black);
            Stroke(pauseState == PauseState.SETTINGS ? Color.Black : Color.White);
            DrawText("SETTINGS", 4, 8, true);
            EndCollisionDraw();
            
            StartCollisionDraw(exitButton);
            Fill(pauseState == PauseState.EXIT ? Color.White : Color.Black);
            Stroke(pauseState == PauseState.EXIT ? Color.Black : Color.White);
            DrawText("EXIT", 4, 12, true);
            EndCollisionDraw();

            if(Raylib.IsCursorHidden()) {
                Raylib.ShowCursor();
            }
            switch(Raylib.GetKeyPressed()) {
                case (int)KeyboardKey.Down:
                    NextState(ref pauseState);
                    break;
                case (int)KeyboardKey.Up:
                    PrevState(ref pauseState);
                    break;
                case (int)KeyboardKey.Escape:
                    SetGameState(GameState.PLAY);
                    return;
                case (int)KeyboardKey.Enter:
                    switch(pauseState) {
                        case PauseState.RESUME:
                            SetGameState(GameState.PLAY);
                            break;
                        case PauseState.SETTINGS:
                            SetGameState(GameState.SETTINGS);
                            break;
                        case PauseState.EXIT:
                            Raylib.CloseWindow();
                            break;

                    }
                    return;
            }

        }

        public static void SetPauseState(PauseState state) {
            pauseState = state;
        }

        public static void NextPauseState() {
            int pauseStateLength = Enum.GetNames(typeof(PauseState)).Length;
            pauseState = (PauseState)((((int)pauseState)+1) % pauseStateLength);
            Console.WriteLine(pauseState);
        }

        public static void PrevPauseState() {
            int pauseStateLength = Enum.GetNames(typeof(PauseState)).Length;
            pauseState = (PauseState)((((int)pauseState)-1) % Enum.GetNames(typeof(PauseState)).Length);
            if(pauseState < 0) pauseState += pauseStateLength;
            Console.WriteLine(pauseState);
        }

        public static void ApplySizing() {
            SetPixelScale(menuBufferResolution.width/menuGameResolution.width);
            SetAndApplyBuffer(menuBufferResolution);
            SetGameSize(menuGameResolution);
        }
    }
}