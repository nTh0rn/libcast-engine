

namespace LibCast {
    using static Screen;
    using static UI;

    public enum PlayState {
        START,
        EXIT
    }

    public class PlayUI {
        public static void ApplySizing() {
            SetPixelScale(8);
            ApplyBuffer(2560, 1440);
            SetGameSize(320, 180);
        }

        public static void Loop() {
            if(KeyPressed(KeyboardKey.Escape)) {
                SetGameState(GameState.PAUSE);
                return;
            }
            return;
        }
    }
}