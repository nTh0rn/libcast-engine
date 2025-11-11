
namespace LibCast {
    public class Timer {
        private double counter = 0;
        private readonly double interval;

        public Timer(double interval) {
            this.interval = interval;
        }

        public bool Tick(double deltaTime) {
            counter += deltaTime;
            if (counter >= interval) {
                counter -= interval;
                return true;
            }
            return false;
        }

        public void Reset() {
            counter = 0;
        }

        public double GetRemainingTime() {
            return interval - counter;
        }
    }
}