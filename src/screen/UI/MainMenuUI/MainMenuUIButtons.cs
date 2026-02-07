namespace LibCast {
    using static MainMenuUI;
    using static UI;
    using static Screen;


    public class MainMenuUIPlayButton : MouseCollision {
        
        public MainMenuUIPlayButton() {}

        public override void Click() {
            SetGameState(GameState.PLAY);
        }

        public override void Hover() {
            if(mainMenuState != MainMenuState.PLAY) {
                SetMainMenuState(MainMenuState.PLAY);
            }
        }
    }

    public class MainMenuUIExitButton : MouseCollision {
        
        public MainMenuUIExitButton() {}

        public override void Click() {
            Raylib.CloseWindow();
        }

        public override void Hover() {
            if(mainMenuState != MainMenuState.EXIT) {
                SetMainMenuState(MainMenuState.EXIT);
            }
        }
    }
}