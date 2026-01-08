

namespace LibCast {
    using static Screen;
    using static UI;

    public enum PlayState {
        START,
        EXIT
    }

    public class PlayUI {
        public static void ApplySizing() {
            SetPixelScale(10);
            ApplyBuffer(2560, 1440);
            SetGameSize(256, 144);
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