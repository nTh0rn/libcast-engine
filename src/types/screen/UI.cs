namespace LibCast {
    using static GameState;
    using static Screen;
    public enum GameState {
            PLAY,
            PAUSE,
            SETTINGS,
            MAINMENU,
        }

        public enum PauseState {
            CONTINUE,
            SETTINGS,
            EXIT
        }

        public enum PlayState {
            START,
            EXIT
        }

        public enum SettingsState {
            RESOLUTION,
            FULLSCREEN,
            FOV,
            APPLY,
            EXIT
        }

    public static class UI {
        

        

        /**
        1920x1080 16/9 50%
        2560x1440 16/9 20%
        2560x1600 16/10 5%

        Steamdeck is 1280x800 16/10
        */
        public static int[,] resolutions = new int[,]{
            {1280, 720, 1280, 720, 10},
            {1280, 800, 2560, 1600, 10},
            {1920, 1080, 2560, 1440, 10}, 
            {1920, 1200, 2560, 1600, 10},
            {2560, 1440, 2560, 1440, 10},
            {2560, 1600, 2560, 1600, 10}
            };
        public static int[] gameDimensions = {256, 144};
        public static int resolution = 2;

        public static GameState gameState = PLAY;
        public static PauseState pauseState = PauseState.CONTINUE;
        public static SettingsState settingsState = SettingsState.RESOLUTION;

        public static int settingsStateLength = Enum.GetNames(typeof(SettingsState)).Length;


        

        public static void Go() {
            switch(gameState) {
                case PLAY:
                    PlayMenu();
                    break;
                case PAUSE:
                    PauseMenu.Draw();
                    break;
                case SETTINGS:
                    //SettingsMenu.Draw();
                    break;
                case MAINMENU:
                    //MainMenu.Draw();
                    break;
            }
        }

        public static void PlayMenu() {
            if(gameWidth != gameDimensions[0] || gameHeight != gameDimensions[1]) {
                ApplyBuffer(gameDimensions[0]*10, gameDimensions[1]*10);
                SetGameSize(gameDimensions);
            }
            if(KeyPressed(KeyboardKey.Escape)) {
                gameState = PAUSE;
                return;
            }
            return;
        }

        public static void PlayApplySizing() {
            ApplyBuffer(2560, 1440);
            SetGameSize(256, 144);
        }

    }
}