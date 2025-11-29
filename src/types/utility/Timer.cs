namespace LibCast {
    public class Timer {
        private double lastTriggerTime;
        private readonly double interval;

        public Timer(double interval) {
            this.interval = interval;
            this.lastTriggerTime = Raylib.GetTime();
        }

        // Call this method each frame to check if the timer should trigger
        public bool Tick() {
            double currentTime = Raylib.GetTime();
            if (currentTime - lastTriggerTime >= interval) {
                lastTriggerTime = currentTime;
                return true;
            }
            return false;
        }

        public void Reset() {
            lastTriggerTime = Raylib.GetTime();
        }

        public double GetRemainingTime() {
            double currentTime = Raylib.GetTime();
            double elapsed = currentTime - lastTriggerTime;
            return Math.Max(0, interval - elapsed);
        }

        public double GetElapsedTime() {
            return Raylib.GetTime() - lastTriggerTime;
        }
    }
}