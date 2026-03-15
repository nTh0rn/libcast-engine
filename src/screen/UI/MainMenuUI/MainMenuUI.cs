

namespace LibCast {
    using static Screen;
    using static UI;

    public enum MainMenuState {
        PLAY,
        EXIT
    }

    public static class MainMenuUI {
        public static MainMenuState mainMenuState = MainMenuState.PLAY;
        public static MainMenuUIPlayButton playButton = new MainMenuUIPlayButton();
        public static MainMenuUIExitButton exitButton = new MainMenuUIExitButton();

        public static void Loop() {
            if(pixelScale != menuBufferResolution.width/menuGameResolution.width) {
                ApplySizing();
            }
            DrawBackground(0, 0, 0);
            
            StartCollisionDraw(playButton);
            Fill(mainMenuState == MainMenuState.PLAY ? Color.White : Color.Black);
            Stroke(mainMenuState == MainMenuState.PLAY ? Color.Black : Color.White);
            DrawText("PLAY", 4, 4, 2, true);
            EndCollisionDraw();

            StartCollisionDraw(exitButton);
            Fill(mainMenuState == MainMenuState.EXIT ? Color.White : Color.Black);
            Stroke(mainMenuState == MainMenuState.EXIT ? Color.Black : Color.White);
            DrawText("EXIT", 4, 8, 2, true);
            EndCollisionDraw();


            if(Raylib.IsCursorHidden()) {
                Raylib.ShowCursor();
            }
            switch(Raylib.GetKeyPressed()) {
                case (int)KeyboardKey.Down:
                    NextState(ref mainMenuState);
                    break;
                case (int)KeyboardKey.Up:
                    PrevState(ref mainMenuState);
                    break;
                case (int)KeyboardKey.Enter:
                    switch(mainMenuState) {
                        case MainMenuState.PLAY:
                            SetGameState(GameState.PLAY);
                            break;
                        case MainMenuState.EXIT:
                            Raylib.CloseWindow();
                            break;

                    }
                    break;
            }

        }

        public static void SetMainMenuState(MainMenuState state) {
            mainMenuState = state;
        }

        public static void ApplySizing() {
            SetPixelScale(menuBufferResolution.width/menuGameResolution.width);
            SetAndApplyBuffer(menuBufferResolution);
            SetGameSize(menuGameResolution);
        }

        public static void DecreaseFOV() {
            Raycaster.FOV -= 2;
            if(Raycaster.FOV < 60) {
                Raycaster.FOV = 60;
            }
        }

        public static void IncreaseFOV() {
            Raycaster.FOV += 2;
            if(Raycaster.FOV > 130) {
                Raycaster.FOV = 130;
            }
        }
    }
}