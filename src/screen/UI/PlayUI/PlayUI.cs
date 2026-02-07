

namespace LibCast {
    using static Screen;
    using static UI;

    public enum PlayState {
        START,
        EXIT
    }

    public class PlayUI {
        public static void ApplySizing() {
            SetPixelScale(playBufferResolution.width/playGameResolution.width);
            SetAndApplyBuffer(playBufferResolution);
            SetGameSize(playGameResolution);
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