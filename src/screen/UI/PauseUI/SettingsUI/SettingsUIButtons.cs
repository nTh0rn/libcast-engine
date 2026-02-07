namespace LibCast {
    using static SettingsUI;
    using static UI;
    using static Screen;


    public class SettingsUIReturnButton : MouseCollision {
        
        public SettingsUIReturnButton() {}

        public override void Click() {
            SetGameState(GameState.PAUSE);
        }

        public override void Hover() {
            if(settingsState != SettingsState.EXIT) {
                SetSettingsState(SettingsState.EXIT);
            }
        }
    }

    public class SettingsUIFOVButton : MouseCollision {
        
        public SettingsUIFOVButton() {}

        public override void Click() {
            Console.WriteLine("Use arrow keys to adjust FOV");
        }

        public override void Hover() {
            if(settingsState != SettingsState.FOV) {
                SetSettingsState(SettingsState.FOV);
            }
        }
    }

    public class SettingsUIFullscreenButton : MouseCollision {
        
        public SettingsUIFullscreenButton() {}

        public override void Click() {
            ToggleFullscreen();
        }

        public override void Hover() {
            if(settingsState != SettingsState.FULLSCREEN) {
                SetSettingsState(SettingsState.FULLSCREEN);
            }
        }
    }
}