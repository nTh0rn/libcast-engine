
namespace LibCast {
    public class Timer {
        private float counter = 0f;
        private readonly float interval;

        public Timer(float interval) {
            this.interval = interval;
        }

        public bool Tick(float deltaTime) {
            counter += deltaTime;
            if (counter >= interval) {
                counter -= interval; // subtract instead of reset to reduce drift
                return true;
            }
            return false;
        }

        public void Reset() {
            counter = 0f;
        }

        public float GetRemainingTime() {
            return interval - counter;
        }
    }
}