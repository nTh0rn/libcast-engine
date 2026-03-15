

namespace LibCast {
    using System.Diagnostics.CodeAnalysis;
    using static Screen;
    using static UI;


    public enum SettingsState {
        FULLSCREEN,
        FOV,
        EXIT
    }

    public static class SettingsUI {
        public static SettingsState settingsState = SettingsState.FULLSCREEN;

        public static SettingsUIFullscreenButton fullscreenButton = new SettingsUIFullscreenButton();
        public static SettingsUIFOVButton fovButton = new SettingsUIFOVButton();
        public static SettingsUIReturnButton returnButton = new SettingsUIReturnButton();

        public static void Loop() {
            Raycaster.Go(Game.room.player, Game.room, 0, 0, Screen.gameWidth, Screen.gameHeight);
            DrawBackground(0, 0, 0, 100);
            
            StartCollisionDraw(fullscreenButton);
            Fill(settingsState == SettingsState.FULLSCREEN ? Color.White : Color.Black);
            Stroke(settingsState == SettingsState.FULLSCREEN ? Color.Black : Color.White);
            DrawText("FULLSCREEN - " + (IsFullscreen() ? "ON" : "OFF"), 4, 4, 2, true);
            EndCollisionDraw();

            StartCollisionDraw(fovButton);
            Fill(settingsState == SettingsState.FOV ? Color.White : Color.Black);
            Stroke(settingsState == SettingsState.FOV ? Color.Black : Color.White);
            DrawText("FOV", 4, 8, 2, true);
            

            Fill(Color.Black);
            DrawRect(10, 7, 40, 4);

            Fill(Color.Gray);
            DrawRect(11, 8, 38, 1);

            int FOVTextOffset = (int)(((Raycaster.FOV-60.0)/70.0)*36.0);
            int FOVSliderOffset = (int)(((Raycaster.FOV-60.0)/70.0)*37.0);

            //Console.WriteLine((int)(((Raycaster.FOV-60.0)/70.0)*36.0));
            Stroke(Color.Gray);
            Fill(Color.Black);
            DrawText("(use arrow keys)", FOVTextOffset > 18 ? 11 : 33, 9, 2, true);
            Stroke(Color.White);
            Fill(Color.Gray);
            DrawText(Raycaster.FOV.ToString(), 11+FOVTextOffset, 9, 2, true);

            Fill(Color.White);
            DrawPixel(11+FOVSliderOffset, 8);
            EndCollisionDraw();

            StartCollisionDraw(returnButton);
            Fill(settingsState == SettingsState.EXIT ? Color.White : Color.Black);
            Stroke(settingsState == SettingsState.EXIT ? Color.Black : Color.White);
            DrawText("RETURN", 4, 12, 2, true);
            EndCollisionDraw();


            if(Raylib.IsCursorHidden()) {
                Raylib.ShowCursor();
            }
            switch(Raylib.GetKeyPressed()) {
                case (int)KeyboardKey.Down:
                    NextState(ref settingsState);
                    break;
                case (int)KeyboardKey.Up:
                    PrevState(ref settingsState);
                    break;
                case (int)KeyboardKey.Escape:
                    SetGameState(GameState.PAUSE);
                    return;
                case (int)KeyboardKey.Enter:
                    switch(settingsState) {
                        case SettingsState.FULLSCREEN:
                            ToggleFullscreen();
                            break;
                        case SettingsState.EXIT:
                            SetGameState(GameState.PAUSE);
                            break;

                    }
                    break;
                case (int)KeyboardKey.Left:
                    if(settingsState == SettingsState.FOV) {
                        DecreaseFOV();
                    }
                    break;
                case (int)KeyboardKey.Right:
                    if(settingsState == SettingsState.FOV) {
                        IncreaseFOV();
                    }
                    break;
            }

        }

        public static void SetSettingsState(SettingsState state) {
            settingsState = state;
        }

        public static void ApplySizing() {
            SetPixelScale(menuBufferResolution.width/menuGameResolution.width);
            SetAndApplyBuffer(menuBufferResolution);
            SetGameSize(menuGameResolution);
        }

        public static void DecreaseFOV() {
            Raycaster.FOV-=2;
            if(Raycaster.FOV < 60) {
                Raycaster.FOV = 60;
            }
        }

        public static void IncreaseFOV() {
            Raycaster.FOV+=2;
            if(Raycaster.FOV > 130) {
                Raycaster.FOV = 130;
            }
        }
    }
}