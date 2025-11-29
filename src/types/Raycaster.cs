
namespace LibCast {

    public struct PixelDepth {
        public Pixel pixel {get; set;}
        public double depth {get; set;}
    }
    public class Raycaster {
        private static List<Entity> entities = new List<Entity>();

        private static double rx, ry, angle, da, dx, dy;
        private static int column, traversalScale;
        private static Entity pov;

        public static void Go(Entity view) {
            List<Entity> entities = new List<Entity>();
            pov = view;
            column = 0;
            angle = pov.direction % 360;
            traversalScale = 1000000;
            da = -Math.Atan((column - Screen.gameWidth / 2.0) / (Screen.gameWidth / 2)) * (180.0 / Math.PI) + angle;
            for (int i = 0; i < Screen.gameWidth; i++) {
                entities.Clear();
                rx = pov.x;
                ry = pov.y;
                da = -Math.Atan((column - Screen.gameWidth / 2.0) / (Screen.gameWidth / 2)) * (180.0 / Math.PI) + angle;
                dx = Math.Cos(da * (Math.PI / 180.0)) / traversalScale;
                dy = -Math.Sin(da * (Math.PI / 180.0)) / traversalScale;
                double distance = CastRay(column);

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
                //Console.WriteLine("rx:" + rx + " ry:" + ry);
                if (Game.room.room[(int)ry][(int)rx] is EmptyCell) {
                    Console.WriteLine("rx:" + rx + " ry:" + ry);
                } else {
                    DrawRayTexture(distance, offset, Game.room.textures[((WallCell)Game.room.room[(int)ry][(int)rx]).texture]);
                }
                column++;
            }
            DrawEntities();
        }

        private static bool inBounds(double x, double y) {
            return y >= 0 && y < Game.room.getHeight() &&
                x >= 0 && x < Game.room.getWidth((int)y);
        }
        private static double CastRay(int column) {
            int counter = 0;
            double xInBox, yInBox, xToBoundary, yToBoundary, xSteps, ySteps, stepAmount;
            double[] finalStepAmount = {0,0};
            while (counter++ < 100) {
                if(!inBounds(rx, ry)) {
                    rx -= finalStepAmount[0];
                    ry -= finalStepAmount[1];
                    break;
                }
                if(Game.room.room[(int)ry][(int)rx] is WallCell) {
                    break;
                }
                double len = Math.Sqrt(dx * dx + dy * dy);
                if (len < 1e-12) {
                    throw new Exception("Ray vector is 0");
                }

                dx /= len;
                dy /= len;

                xInBox = rx - Math.Floor(rx);
                yInBox = ry - Math.Floor(ry);

                xToBoundary = (dx > 0) ? (1.0 - xInBox) : xInBox;
                yToBoundary = (dy > 0) ? (1.0 - yInBox) : yInBox;

                xSteps = (Math.Abs(dx) > 1e-6) ? Math.Abs(xToBoundary / dx) : double.PositiveInfinity;
                ySteps = (Math.Abs(dy) > 1e-6) ? Math.Abs(yToBoundary / dy) : double.PositiveInfinity;

                stepAmount = Math.Min(xSteps, ySteps);

                if (stepAmount <= 0 || double.IsNaN(stepAmount) || double.IsInfinity(stepAmount)) {
                    finalStepAmount[0] = 1e-6 * (Math.Sign(dx) == 0 ? 1 : Math.Sign(dx));
                    finalStepAmount[1] = 1e-6 * (Math.Sign(dy) == 0 ? 1 : Math.Sign(dy));
                } else {
                    finalStepAmount[0] = stepAmount * dx;
                    finalStepAmount[1] = stepAmount * dy;
                }

                rx += finalStepAmount[0];
                ry += finalStepAmount[1];
            }
            double distance = DistanceBetween(pov.x, pov.y, rx, ry);
            return Math.Abs(Math.Sin(Math.Abs(da - (angle + 90)) * (Math.PI / 180.0))) * distance;
    }

        private static void DrawRay(double distance, bool isCorner = false) {
            distance = Math.Max(0.01, distance);
            double height = Math.Max(1, Math.Min(Screen.gameHeight, Screen.gameHeight / distance));
            for (int i = 0; i < (int)height; i++) {
                Screen.Fill(Raylib_cs.Color.Green);
                Screen.DrawPixelDepth(column, (int)(Screen.gameHeight / 2.0 - height / 2.0 + i), (int)(distance*100));
            }

            for(int i = 0; i < distance; i++) {
                
            }
        }

        private static void DrawRayTexture(double distance, double xPos, Color[,] texture) {
            distance = Math.Max(0.01, distance);
            double height = Math.Max(1, Screen.gameHeight / distance);
            for (int i = 0; i < (int)height; i++) {
                if ((int)(Screen.gameHeight / 2.0 - height / 2.0 + i) >= 0 &&
                    (int)(Screen.gameHeight / 2.0 - height / 2.0 + i) < Screen.gameHeight) {
                    Color c;
                    int textureHeight = (int)Math.Round(Math.Sqrt(texture.Length))-1;

                    c = texture[(int)(textureHeight * (i / height)), (int)(textureHeight * (xPos))];
                    if(c.A == 0) {
                        continue;
                    }
                    Screen.Fill(c);
                    Screen.DrawPixelDepth(column, (int)(Screen.gameHeight / 2.0 - height / 2.0 + i), (int)(distance*100));
                    Screen.Stroke(Color.Blue);
                }
            }
        }

        private static void DrawEntities() {
            foreach(Entity entity in Game.room.entities) {
                if(entity is Player) {
                    continue;
                }

                // Calculate actual distance from player to entity
                double actualDistance = DistanceBetween(entity, pov);

                double entityDx = entity.x - pov.x;
                double entityDy = entity.y - pov.y;
                
                // Calculate the angle from player to entity
                double entityDa = Math.Atan2(-entityDy, entityDx) * (180.0 / Math.PI);
                
                // Normalize entityDa to [0, 360]
                while (entityDa < 0) entityDa += 360;
                while (entityDa >= 360) entityDa -= 360;
                
                // Calculate the angle difference, normalized to [-180, 180]
                double angleDiff = angle - entityDa;
                while (angleDiff > 180) angleDiff -= 360;
                while (angleDiff < -180) angleDiff += 360;
                
                // Calculate column position using normalized angle difference
                double entityColumn = Screen.gameWidth / 2.0 + Math.Tan(angleDiff * (Math.PI / 180.0)) * (Screen.gameWidth / 2.0);

                double correctedDistance = Math.Abs(Math.Sin(Math.Abs(entityDa - (angle + 90)) * (Math.PI / 180.0))) * actualDistance;
                correctedDistance = Math.Max(0.01, correctedDistance);
                
                int width = (int)Math.Max(1, Screen.gameHeight / correctedDistance);
                int halfWidth = width / 2;
                
                double leftEdge = entityColumn - halfWidth;
                double rightEdge = entityColumn + halfWidth;
                
                if (rightEdge < 0 || leftEdge >= Screen.gameWidth) {
                    continue;
                }

                double verifyDa = -Math.Atan((entityColumn - Screen.gameWidth / 2.0) / (Screen.gameWidth / 2)) * (180.0 / Math.PI) + angle;
                while (verifyDa < 0) verifyDa += 360;
                while (verifyDa >= 360) verifyDa -= 360;
                
                double angleDifference = Math.Abs(verifyDa - entityDa);
                if (angleDifference > 180) angleDifference = 360 - angleDifference;
                if (angleDifference > 1.0) {
                    continue;
                }

                int prevColumn = column;
                column = (int)Math.Round(entityColumn);
                Console.WriteLine(correctedDistance);
                column -= width/2;
                for(int i = 0; i < width; i++) {
                    if(column >= 0 && column < Screen.gameWidth) {
                        DrawRayTexture(correctedDistance, ((double)i)/width, Game.room.textures[entity.texture]);
                    }
                    column++;
                }
                
                column = prevColumn;
            }
        }
        
    }
}