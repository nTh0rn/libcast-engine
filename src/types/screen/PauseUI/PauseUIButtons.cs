

namespace LibCast {
    using static PauseUI;
    using static UI;

    public class PauseUIPlayButton : MouseCollision {
        

        public PauseUIPlayButton() {
            
        }

        

        public override void Click() {
            SetGameState(GameState.PLAY);
        }

        public override void Hover() {
            if(pauseState != PauseState.RESUME) {
                SetPauseState(PauseState.RESUME);
            }
        }
    }

    public class PauseUIExitButton : MouseCollision {
        

        public PauseUIExitButton() {
            
        }

        

        public override void Click() {
            Raylib.CloseWindow();
        }

        public override void Hover() {
            if(pauseState != PauseState.EXIT) {
                SetPauseState(PauseState.EXIT);
            }
        }
    }

    public class PauseUISettingsButton : MouseCollision {
        

        public PauseUISettingsButton() {
            
        }

        

        public override void Click() {
            SetGameState(GameState.SETTINGS);
        }

        public override void Hover() {
            if(pauseState != PauseState.SETTINGS) {
                SetPauseState(PauseState.SETTINGS);
            }
        }
    }
}