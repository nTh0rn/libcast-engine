using Raylib_cs;

namespace LibCast {
    public class Global {
        public static bool KeyDown(KeyboardKey key) {
            return Raylib.IsKeyDown(key);
        }

        public static bool KeyPressed(KeyboardKey key) {
            return Raylib.IsKeyPressed(key);
        }

        public static double DistanceBetween(double x1, double y1, double x2, double y2) {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
}