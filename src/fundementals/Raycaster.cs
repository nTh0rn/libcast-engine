using Raylib_cs;

namespace LibCast {
    public class Raycaster : Global {
        private static List<Entity> entities = new List<Entity>();

        private static double rx, ry, angle, da, dx, dy;
        private static int column, traversalScale;
        private static Entity pov;

        public static void Go(Entity view) {
            List<Entity> entities = new List<Entity>();
            pov = view;
            column = 0;
            angle = pov.direction;
            traversalScale = 500;
            da = -Math.Atan((column - Screen.gameWidth / 2.0) / (Screen.gameWidth / 2)) * (180.0 / Math.PI) + angle;
            for (int i = 0; i < Screen.gameWidth; i++) {
                entities.Clear();
                rx = pov.x;
                ry = pov.y;
                da = -Math.Atan((column - Screen.gameWidth / 2.0) / (Screen.gameWidth / 2)) * (180.0 / Math.PI) + angle;
                dx = Math.Cos(da * (Math.PI / 180.0)) / traversalScale;
                dy = -Math.Sin(da * (Math.PI / 180.0)) / traversalScale;
                double distance = CastRay(column);
                Console.WriteLine(distance);

                //Texture offset
                double xOffset = rx - Math.Floor(rx);
                double yOffset = ry - Math.Floor(ry);
                double offset = (xOffset >= 0.5 ? 1 - xOffset : xOffset) > (yOffset >= 0.5 ? 1 - yOffset : yOffset) ? xOffset : yOffset;
                while(da < 0) {
                    da = da + 360;
                }
                if((da > 90 && da < 270 && offset == yOffset) ||
                    da > 180 && offset == xOffset) {
                    offset = 1 - offset;
                }

                DrawRayTexture(distance, offset, (WallCell)Game.room.room[(int)ry][(int)rx]);

                foreach (Entity entity in entities) {
                    DrawEntity(entity);
                }
                column++;
                
            }
            
        }

        private static bool inBounds(double x, double y) {
            return x >= 0 && x < Game.room.getWidth() &&
                y >= 0 && y < Game.room.getHeight() &&
                !(Game.room.room[(int)y][(int)x] is WallCell); 
        }
        private static double CastRay(int column) {
            int counter = 0;
            while (inBounds(rx, ry) && counter++ < 100) {
                foreach (Entity entity in Game.room.entities) {
                    if (entity.CollisionPoint(rx, ry)) {
                        entities.Add(entity);
                    }
                }
                double len = Math.Sqrt(dx * dx + dy * dy);
                                    if (len < 1e-12)
                    {
                        Console.WriteLine("Invalid direction (dx, dy) ≈ (0, 0)");
                        return 0;
                    }

                    dx /= len;
                    dy /= len;

                    // --- Find fractional position inside the current cell ---
                    double xInBox = rx - Math.Floor(rx);
                    double yInBox = ry - Math.Floor(ry);

                    // --- Distance to next cell boundary depending on direction ---
                    double xToBoundary = (dx > 0) ? (1.0 - xInBox) : xInBox;
                    double yToBoundary = (dy > 0) ? (1.0 - yInBox) : yInBox;

                    // --- Compute how far along the ray to move (parameter t) ---
                    double xSteps = (Math.Abs(dx) > 1e-12) ? Math.Abs(xToBoundary / dx) : double.PositiveInfinity;
                    double ySteps = (Math.Abs(dy) > 1e-12) ? Math.Abs(yToBoundary / dy) : double.PositiveInfinity;

                    // --- Choose the smaller step (the next grid boundary) ---
                    double stepAmount = Math.Min(xSteps, ySteps);

                    // --- Safety check ---
                    if (stepAmount <= 0 || double.IsNaN(stepAmount) || double.IsInfinity(stepAmount))
                    {
                        // Nudge slightly to avoid zero-step or numerical deadlock
                        const double epsilon = 1e-6;
                        rx += epsilon * (Math.Sign(dx) == 0 ? 1 : Math.Sign(dx));
                        ry += epsilon * (Math.Sign(dy) == 0 ? 1 : Math.Sign(dy));
                    }
                    else
                    {
                        // Advance to next boundary
                        rx += stepAmount * dx;
                        ry += stepAmount * dy;
                    }

                                }

            double distance = DistanceBetween(pov.x, pov.y, rx, ry);
        return Math.Abs(Math.Sin(Math.Abs(da - (angle + 90)) * (Math.PI / 180.0))) * distance;
    }

    // Compute distance for column rendering
    
    //}


        private static void DrawRay(double distance, bool isCorner = false) {
            distance = Math.Max(0.01, distance);
            double height = Math.Max(1, Math.Min(Screen.gameHeight, Screen.gameHeight / distance));
            for (int i = 0; i < (int)height; i++) {
                Screen.Fill(Raylib_cs.Color.Green);
                Screen.DrawPixel(column, (int)(Screen.gameHeight / 2.0 - height / 2.0 + i));
            }
        }

        private static void DrawRayTexture(double distance, double xPos, WallCell cell) {
            distance = Math.Max(0.01, distance);
            double height = Math.Max(1, Screen.gameHeight / distance);
            for (int i = 0; i < (int)height; i++) {
                if ((int)(Screen.gameHeight / 2.0 - height / 2.0 + i) >= 0 &&
                    (int)(Screen.gameHeight / 2.0 - height / 2.0 + i) < Screen.gameHeight) {
                    Color c;
                    Color[,] texture = Game.room.textures[cell];
                    Screen.Fill(texture[(int)(31 * (i / height)), (int)(31 * (xPos))]);
                    Screen.DrawPixel(column, (int)(Screen.gameHeight / 2.0 - height / 2.0 + i));
                    Screen.Stroke(Color.Blue);
                }
            }
        }

        private static void DrawEntity(Entity entity) {

        }
        
    }
}