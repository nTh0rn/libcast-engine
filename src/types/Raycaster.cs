
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
        
        public static int FOV = 90;
        private static double fovScale;
        // Pitch scaling factors (adjustable globally)
        public static double PitchFovScaleFactor = 0.3;
        public static double PitchDistanceScaleFactor = 0.7;
        private const double DEG2RAD = Math.PI / 180.0;
        private const double RAD2DEG = 180.0 / Math.PI;

        public static void Go(Entity view) {
            pov = view;
            column = 0;
            angle = pov.direction % 360;
            traversalScale = 1000000;
            
            fovScale = Math.Tan(FOV / 2.0 * DEG2RAD);

            // Apply pitch-driven vertical FOV scaling (visual only) by temporarily scaling fovScale
            double pitch = Game.room.player?.pitch ?? 0.0;
            double fovVerticalScale = 1.0 + Math.Abs(pitch / 30.0) * PitchFovScaleFactor;
            double distanceScale = 1.0 + Math.Abs(pitch / 30.0) * PitchDistanceScaleFactor;
            double oldFovScale = fovScale;
            fovScale = Math.Tan((FOV * fovVerticalScale) / 2.0 * (Math.PI / 180.0));

            for (int i = 0; i < Screen.gameWidth; i++) {
                entities.Clear();
                rx = pov.x;
                ry = pov.y;
                da = -Math.Atan((column - Screen.gameWidth / 2.0) / (Screen.gameWidth / 2.0) * fovScale) * RAD2DEG + angle;
                dx = Math.Cos(da * DEG2RAD) / traversalScale;
                dy = -Math.Sin(da * DEG2RAD) / traversalScale;
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

                RoomCell cell = Game.room.room[(int)ry][(int)rx];
                if (cell is EmptyCell) {
                    //Console.WriteLine("rx:" + rx + " ry:" + ry);
                    DrawFloorCeiling(distance);
                } else if(cell is WallCell) {
                    DrawFloorCeiling(distance);
                    DrawRayTexture(distance, offset, Game.room.textures[((WallCell)cell).texture], cell is TallCell);
                }
                column++;
            }
            DrawEntities();
            // restore fovScale after rendering
            fovScale = oldFovScale;
        }

        public static bool inBounds(double x, double y) {
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
            return Math.Abs(Math.Cos((da - angle) * (Math.PI / 180.0))) * distance;
        }

        private static void DrawRay(double distance, bool isCorner = false) {
            distance = Math.Max(0.01, distance);
            double height = Math.Max(1, Math.Min(Screen.gameHeight, Screen.gameHeight / distance));
            for (int i = 0; i < (int)height; i++) {
                Screen.Fill(Color.Green);
                Screen.DrawPixelDepth(column, (int)(Screen.gameHeight / 2.0 - height / 2.0 + i), (int)(distance*100));
            }

            for(int i = 0; i < distance; i++) {
                
            }
        }

        private static void DrawRayTexture(double distance, double xPos, Color[,] texture, bool isTall=false) {
            double pitch = Game.room.player?.pitch ?? 0.0;
            // distanceScale matches original behavior for pitch distance scaling
            double distanceScale = 1.0 + Math.Abs(pitch / 30.0) * PitchDistanceScaleFactor;
            distance = Math.Max(0.01, distance) * distanceScale;
            double height = Math.Max(1, Screen.gameHeight / distance) * (isTall ? 2 : 1);

            double halfHeight = Screen.gameHeight / 2.0;
            double top = halfHeight - height * (isTall ? .75 : 0.5) + pitch;
            double bottom = top + height;

            int yStart = Math.Max(0, (int)Math.Floor(top));
            int yEnd = Math.Min(Screen.gameHeight - 1, (int)Math.Ceiling(bottom) - 1);

            int textureHeight = texture.GetLength(0) - 1;
            int textureWidth = texture.GetLength(1) - 1;

            for (int y = yStart; y <= yEnd; y++) {
                // use pixel center for consistent coverage
                double centerY = y + 0.5;
                if (centerY < top || centerY >= bottom) continue;

                double v = (centerY - top) / height; // 0..1
                int texY = (int)(textureHeight * v);
                int texX = (int)(textureWidth * xPos);

                Color c = texture[texY, texX];
                if (c.A == 0) continue;

                Screen.Fill(c);
                // depth uses the scaled distance (distance already multiplied by distanceScale)
                Screen.DrawPixelDepth(column, y, (int)(distance * 100));
                Screen.Stroke(Color.Blue);
            }
        }

        private static void DrawFloorCeiling(double wallDistance) {
            double pitch = Game.room.player?.pitch ?? 0.0;
            const double PI_OVER_180 = Math.PI / 180.0;
            
            double rayDirX = Math.Cos(da * PI_OVER_180);
            double rayDirY = -Math.Sin(da * PI_OVER_180);
            double cosAngleDiff = Math.Cos((da - angle) * PI_OVER_180);
            double halfHeight = Screen.gameHeight / 2.0;
            
            // Compute wall bounds using pixel-center logic to skip wall-covered rows
            double distanceScale = 1.0 + Math.Abs(pitch / 30.0) * PitchDistanceScaleFactor;
            double scaledDistance = wallDistance * distanceScale;
            double wallHeight = Math.Max(1, Screen.gameHeight / scaledDistance);
            double wallTop = halfHeight - wallHeight * 0.5 + pitch;
            double wallBottom = wallTop + wallHeight;
            
            // Process all rows, skip wall-covered ones using pixel-center comparison
            for (int y = 0; y < Screen.gameHeight; y++) {
                double pixelCenterY = y + 0.5;
                // Skip rows covered by the wall (use pixel center to match wall drawing)
                if (pixelCenterY >= wallTop && pixelCenterY < wallBottom) continue;
                
                double geometricY = pixelCenterY - pitch;
                double denominator = halfHeight - geometricY;
                if (Math.Abs(denominator) < 1e-6) continue;

                double scaledPerp = Screen.gameHeight / (2.0 * denominator);
                double unscaledPerp = scaledPerp / distanceScale;

                if (Math.Abs(cosAngleDiff) < 1e-6) continue;

                double perpAbs = Math.Abs(unscaledPerp);
                double alongDist = perpAbs / Math.Abs(cosAngleDiff);
                double depthPerp = perpAbs * distanceScale;

                double worldX = pov.x + rayDirX * alongDist;
                double worldY = pov.y + rayDirY * alongDist;

                int cellX = (int)Math.Floor(worldX);
                int cellY = (int)Math.Floor(worldY);
                if (!inBounds(cellX, cellY)) continue;

                RoomCell cell = Game.room.room[cellY][cellX];
                bool screenIsFloor = geometricY > halfHeight;
                double pixelPerp = perpAbs;
                bool isFloor = screenIsFloor;
                bool isCeiling = (!screenIsFloor) && (pixelPerp < wallDistance);

                if (isFloor) {
                    Color[,]? texture = null;
                    string? textureKey = cell.floorTexture;
                    if (textureKey != null && Game.room.textures.ContainsKey(textureKey)) {
                        texture = Game.room.textures[textureKey];
                    } else {
                        const int maxSteps = 8;
                        const double step = 0.2;
                        double probeX = worldX;
                        double probeY = worldY;
                        for (int s = 0; s < maxSteps && texture == null; s++) {
                            probeX += rayDirX * step;
                            probeY += rayDirY * step;
                            int px = (int)Math.Floor(probeX);
                            int py = (int)Math.Floor(probeY);
                            if (!inBounds(px, py)) break;
                            var probeCell = Game.room.room[py][px];
                            if (probeCell.floorTexture != null && Game.room.textures.ContainsKey(probeCell.floorTexture)) {
                                textureKey = probeCell.floorTexture;
                                texture = Game.room.textures[textureKey];
                                break;
                            }
                        }
                    }
                    if (texture != null) {
                        double fracX = worldX - Math.Floor(worldX);
                        double fracY = worldY - Math.Floor(worldY);
                        int texW = texture.GetLength(1);
                        int texH = texture.GetLength(0);
                        int texX = ((int)(fracX * texW) % texW + texW) % texW;
                        int texY = ((int)(fracY * texH) % texH + texH) % texH;
                        Color c = texture[texY, texX];
                        if (c.A > 0) {
                            Screen.Fill(c);
                            int depthValue = (int)(depthPerp * 100);
                            Screen.DrawPixelDepth(column, y, depthValue);
                        }
                    }
                } else if (isCeiling) {
                    Color[,]? ctexture = null;
                    string? ctextureKey = cell.ceilingTexture;
                    if (ctextureKey != null && Game.room.textures.ContainsKey(ctextureKey)) {
                        ctexture = Game.room.textures[ctextureKey];
                    } else {
                        const int maxSteps = 8;
                        const double step = 0.2;
                        double probeX = worldX;
                        double probeY = worldY;
                        for (int s = 0; s < maxSteps && ctexture == null; s++) {
                            probeX += rayDirX * step;
                            probeY += rayDirY * step;
                            int px = (int)Math.Floor(probeX);
                            int py = (int)Math.Floor(probeY);
                            if (!inBounds(px, py)) break;
                            var probeCell = Game.room.room[py][px];
                            if (probeCell.ceilingTexture != null && Game.room.textures.ContainsKey(probeCell.ceilingTexture)) {
                                ctextureKey = probeCell.ceilingTexture;
                                ctexture = Game.room.textures[ctextureKey];
                                break;
                            }
                        }
                    }
                    if (ctexture != null) {
                        double fracX = worldX - Math.Floor(worldX);
                        double fracY = worldY - Math.Floor(worldY);
                        int texW = ctexture.GetLength(1);
                        int texH = ctexture.GetLength(0);
                        int texX = ((int)(fracX * texW) % texW + texW) % texW;
                        int texY = ((int)(fracY * texH) % texH + texH) % texH;
                        Color c = ctexture[texY, texX];
                        if (c.A > 0) {
                            Screen.Fill(c);
                            int depthValue = (int)(depthPerp * 100);
                            Screen.DrawPixelDepth(column, y, depthValue);
                        }
                    }
                }
            }
        }

        private static void DrawEntities() {
            const double PI_OVER_180 = Math.PI / 180.0;
            const double INV_PI_OVER_180 = 180.0 / Math.PI;
            double halfScreenWidth = Screen.gameWidth / 2.0;
            int prevColumn = column;

            foreach(Entity entity in Game.room.entities) {
                if(entity is Player) {
                    continue;
                }

                double actualDistance = DistanceBetween(entity, pov);

                double entityDx = entity.x - pov.x;
                double entityDy = entity.y - pov.y;
                
                double entityDa = Math.Atan2(-entityDy, entityDx) * INV_PI_OVER_180;
                
                entityDa = ((entityDa % 360) + 360) % 360;
                
                double angleDiff = angle - entityDa;
                angleDiff = ((angleDiff + 180) % 360) - 180;
                if (angleDiff < -180) angleDiff += 360;
                
                double entityColumn = halfScreenWidth + (Math.Tan(angleDiff * PI_OVER_180) / fovScale) * halfScreenWidth;

                double correctedDistance = Math.Max(0.01, Math.Abs(Math.Cos((entityDa - angle) * PI_OVER_180)) * actualDistance);
                int width = (int)Math.Max(1, (Screen.gameHeight / correctedDistance) / fovScale);
                int halfWidth = width / 2;
                
                double leftEdge = entityColumn - halfWidth;
                double rightEdge = entityColumn + halfWidth;
                
                if (rightEdge < 0 || leftEdge >= Screen.gameWidth) {
                    continue;
                }

                double verifyDa = -Math.Atan((entityColumn - halfScreenWidth) / halfScreenWidth * fovScale) * INV_PI_OVER_180 + angle;
                verifyDa = ((verifyDa % 360) + 360) % 360;
                
                double angleDifference = Math.Abs(verifyDa - entityDa);
                if (angleDifference > 180) angleDifference = 360 - angleDifference;
                if (angleDifference > 1.0) {
                    continue;
                }

                column = (int)Math.Round(entityColumn) - halfWidth;
                if (entity.texture == null || !Game.room.textures.ContainsKey(entity.texture)) {
                    continue;
                }
                Color[,] entityTexture = Game.room.textures[entity.texture];
                double invWidth = 1.0 / width;
                
                for(int i = 0; i < width; i++) {
                    if(column >= 0 && column < Screen.gameWidth) {
                        DrawRayTexture(correctedDistance, i * invWidth, entityTexture);
                    }
                    column++;
                }
            }
            
            column = prevColumn;
        }
        
    }
}