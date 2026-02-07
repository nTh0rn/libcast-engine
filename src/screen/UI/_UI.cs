namespace LibCast {
    using System.Numerics;
    using static GameState;
    using static Screen;

    public enum GameState {
        PLAY,
        PAUSE,
        SETTINGS,
        MAINMENU,
    }

    public static class UI {


        public static GameState gameState = MAINMENU;

        public static List<MouseCollision> mouseCollisions = new List<MouseCollision>();

        public static int settingsStateLength = Enum.GetNames(typeof(SettingsState)).Length;

        public static int mouseX, mouseY;


        
        

        public static void Go() {
            UpdateMouseXY();
            switch(gameState) {
                case PLAY:
                    PlayUI.Loop();
                    break;
                case PAUSE:
                    PauseUI.Loop();
                    break;
                case SETTINGS:
                    SettingsUI.Loop();
                    break;
                case MAINMENU:
                    MainMenuUI.Loop();
                    break;
            }
            MouseCollision mouseCollision = screen[mouseY][mouseX].collisionFamily;
            if(mouseCollision != null) {
                mouseCollision.Hover();
                if(Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                    mouseCollision.Click();
                }
            }
            
        }

        public static bool GameStateIsPlay() {
            return gameState == PLAY;
        }

        public static void SetGameState(GameState state) {
            gameState = state;
            switch(gameState) {
                case PLAY:
                    PlayUI.ApplySizing();
                    break;
                case PAUSE:
                    PauseUI.ApplySizing();
                    break;
                case SETTINGS:
                    SettingsUI.ApplySizing();
                    break;
                case MAINMENU:
                    MainMenuUI.ApplySizing();
                    break;
            }
        }

        public static void NextState<TEnum>(ref TEnum state) where TEnum : struct, Enum {
            var states = (TEnum[])Enum.GetValues(typeof(TEnum));
            int index = Array.IndexOf(states, state);
            if (index < 0) index = 0;
            state = states[(index + 1) % states.Length];
        }

        public static void PrevState<TEnum>(ref TEnum state) where TEnum : struct, Enum {
            var states = (TEnum[])Enum.GetValues(typeof(TEnum));
            int index = Array.IndexOf(states, state);
            if (index < 0) index = 0;
            state = states[(index - 1 + states.Length) % states.Length];
        }

        public static Vector2 GetMousePosition() {

            int windowWidth  = Raylib.GetScreenWidth();
            int windowHeight = Raylib.GetScreenHeight();

            float scale = Math.Min(
                (float)windowWidth  / gameWidth,
                (float)windowHeight / gameHeight
            );

            float offsetX = (windowWidth  - gameWidth  * scale) * 0.5f;
            float offsetY = (windowHeight - gameHeight * scale) * 0.5f;

            Vector2 mouse = Raylib.GetMousePosition();
            return new Vector2((mouse.X - offsetX) / scale, (mouse.Y - offsetY) / scale);
        }

        public static void UpdateMouseXY() {
            Vector2 mousePos = GetMousePosition();
            mouseX = (int)mousePos.X;
            mouseY = (int)mousePos.Y;
            if(mouseX < 0) mouseX = 0;
            if(mouseX > gameWidth-1) mouseX = gameWidth-1;
            if(mouseY < 0) mouseY = 0;
            if(mouseY > gameHeight-1) mouseY = gameHeight-1;
        }
    }
}