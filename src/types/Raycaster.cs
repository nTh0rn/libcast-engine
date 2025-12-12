namespace LibCast {

    public struct PixelDepth {
        public Pixel pixel {get; set;}
        public double depth {get; set;}
    }
    public class Raycaster {
        private static List<Entity> entities = new List<Entity>();

        private static double rx, ry, angle, da, dx, dy;
        private static int column, traversalScale;
        private static Entity? pov;
        
        public static int FOV = 110;
        private static double fovScale, pitch, distanceScale;
        private static double wallHeightScale;
        // Pitch scaling factors (adjustable globally)
        public static double PitchFovScaleFactor = 0.0;
        public static double PitchDistanceScaleFactor = 0.0;
        private const double DEG2RAD = Math.PI / 180.0;
        private const double RAD2DEG = 180.0 / Math.PI;
        private const double PI_OVER_180 = Math.PI / 180.0;

        public static void Go(Entity view) {
            pov = view;
            column = 0;
            angle = pov.direction % 360;
            traversalScale = 1000000;
            pitch = Game.room.player?.pitch ?? 0.0;
            
            double oldFovScale = Math.Tan(FOV / 2.0 * DEG2RAD);
            fovScale = oldFovScale;

            // Apply pitch-driven vertical FOV scaling (visual only) by temporarily scaling fovScale
            distanceScale = 1.0 + Math.Abs(pitch / 30.0) * PitchDistanceScaleFactor;
            fovScale = Math.Tan((FOV * (1.0 + Math.Abs(pitch / 30.0) * PitchFovScaleFactor)) / 2.0 * DEG2RAD);
            
            // Precompute wall height scale to keep walls square regardless of FOV
            // For a 1-unit wall to appear square: height_pixels = width_pixels
            // width_pixels = (Screen.gameWidth / 2.0) / tan(FOV/2) / distance
            // So height_pixels should be the same, not Screen.gameHeight / distance
            // Use the active fovScale (may have pitch adjustments) so vertical projection matches horizontal FOV handling
            wallHeightScale = (Screen.gameWidth / 2.0) / fovScale;

            for (int i = 0; i < Screen.gameWidth; i++) {
                entities.Clear();
                rx = pov.x;
                ry = pov.y;
                da = -Math.Atan((column - Screen.gameWidth / 2.0) / (Screen.gameWidth / 2.0) * fovScale) * RAD2DEG + angle;
                while(da < 0) {
                    da = da + 360;
                }
                dx = Math.Cos(da * DEG2RAD) / traversalScale;
                dy = -Math.Sin(da * DEG2RAD) / traversalScale;
                double distance = CastRay(column);

                //Texture offset
                double xOffset = rx - (int)rx;
                double yOffset = ry - (int)ry;
                double offset = (xOffset >= 0.5 ? 1 - xOffset : xOffset) > (yOffset >= 0.5 ? 1 - yOffset : yOffset) ? xOffset : yOffset;
                if((da > 90 && da < 270 && offset == yOffset) ||
                    da > 180 && offset == xOffset) {
                    offset = 1 - offset;
                }

                RoomCell cell = Game.room.room[(int)ry][(int)rx];
                if (cell is EmptyCell) {
                    DrawFloorCeiling(distance);
                } else if(cell is WallCell wallCell && wallCell.texture != null) {
                    DrawFloorCeiling(distance);
                    DrawRayTexture(distance, offset, Game.room.textures[wallCell.texture], cell is TallCell);
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
            double xToBoundary, yToBoundary, xSteps, ySteps, stepAmount;
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

                double xInBox = rx - (int)rx;
                double yInBox = ry - (int)ry;

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
            double distance = DistanceBetween(pov!.x, pov.y, rx, ry);
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
            distance *= distanceScale;
            distance = Math.Max(0.01, distance);
            // Use wallHeightScale instead of Screen.gameHeight to keep walls square
            double height = Math.Max(1, wallHeightScale / distance) * (isTall ? 2 : 1);

            double halfHeight = Screen.gameHeight / 2.0;
            double top = halfHeight - height * (isTall ? .75 : 0.5) + pitch;
            double bottom = top + height;

            int yStart = Math.Max(0, (int)Math.Floor(top));
            int yEnd = Math.Min(Screen.gameHeight - 1, (int)Math.Ceiling(bottom) - 1);

            int textureHeight = texture.GetLength(0) - 1;
            int textureWidth = texture.GetLength(1) - 1;
            double invHeight = 1.0 / height;

            for (int y = yStart; y <= yEnd; y++) {
                // use pixel center for consistent coverage
                double centerY = y + 0.5;
                if (centerY < top || centerY >= bottom) continue;

                double v = (centerY - top) * invHeight; // 0..1
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
            double rayDirX = Math.Cos(da * PI_OVER_180);
            double rayDirY = -Math.Sin(da * PI_OVER_180);
            double cosAngleDiff = Math.Cos((da - angle) * PI_OVER_180);
            double halfHeight = Screen.gameHeight / 2.0;
            double absCosAngleDiff = Math.Abs(cosAngleDiff);
            
            // Compute wall bounds using pixel-center logic to skip wall-covered rows
            // Use wallHeightScale instead of Screen.gameHeight to match square wall drawing
            double wallHeight = Math.Max(1, wallHeightScale / (wallDistance * distanceScale));
            double wallTop = halfHeight - wallHeight * 0.5 + pitch;
            double wallBottom = wallTop + wallHeight;
            
            // Pre-compute frequently used values
            double povX = pov!.x;
            double povY = pov.y;
            
            // Process all rows, skip wall-covered ones using pixel-center comparison
            for (int y = 0; y < Screen.gameHeight; y++) {
                double pixelCenterY = y + 0.5;
                // Skip rows covered by the wall (use pixel center to match wall drawing)
                if (pixelCenterY >= wallTop && pixelCenterY < wallBottom) continue;
                
                double geometricY = pixelCenterY - pitch;
                // Use signed offset from center (positive below center)
                double signedOffset = geometricY - halfHeight;
                if (Math.Abs(signedOffset) < 1e-6) continue;

                if (absCosAngleDiff < 1e-6) continue;

                // --- CORRECTED: compute distance to floor/ceiling row so it matches wall projection ---
                // Eye height is 0.5 world units (walls are 1 unit tall and are centered on screen),
                // so that at the wall bottom/top pixels we get the same distance as the wall ray.
                double eyeHeight = 0.5;
                // rowDistance: distance along the ray (perp) corresponding to this screen row
                // we use wallHeightScale as the focal length (fov-aware), and include distanceScale in denominator
                double rowDistance = (eyeHeight * wallHeightScale) / (signedOffset * distanceScale);

                double alongDist = Math.Abs(rowDistance) / absCosAngleDiff;
                double depthPerp = Math.Abs(rowDistance);

                double worldX = povX + rayDirX * alongDist;
                double worldY = povY + rayDirY * alongDist;

                int cellX = (int)worldX;
                int cellY = (int)worldY;
                if (!inBounds(cellX, cellY)) continue;

                RoomCell cell = Game.room.room[cellY][cellX];
                bool screenIsFloor = geometricY > halfHeight;

                if (screenIsFloor) {
                    DrawTexturedSurface(cell.floorTexture, worldX, worldY, rayDirX, rayDirY, depthPerp, y);
                } else if (Math.Abs(rowDistance) < wallDistance) {
                    DrawTexturedSurface(cell.ceilingTexture, worldX, worldY, rayDirX, rayDirY, depthPerp, y);
                }
            }
        }

        private static void DrawTexturedSurface(string? textureKey, double worldX, double worldY, double rayDirX, double rayDirY, double depthPerp, int y) {
            Color[,]? texture = null;
            if (textureKey != null && Game.room.textures.ContainsKey(textureKey)) {
                texture = Game.room.textures[textureKey];
            } else {
                double probeX = worldX;
                double probeY = worldY;
                for (int s = 0; s < 8 && texture == null; s++) {
                    probeX += rayDirX * 0.2;
                    probeY += rayDirY * 0.2;
                    int px = (int)probeX;
                    int py = (int)probeY;
                    if (!inBounds(px, py)) break;
                    var probeCell = Game.room.room[py][px];
                    if (probeCell.ceilingTexture != null && Game.room.textures.ContainsKey(probeCell.ceilingTexture)) {
                        texture = Game.room.textures[probeCell.ceilingTexture];
                        break;
                    }
                    if (probeCell.floorTexture != null && Game.room.textures.ContainsKey(probeCell.floorTexture)) {
                        texture = Game.room.textures[probeCell.floorTexture];
                        break;
                    }
                }
            }
            if (texture != null) {
                double fracX = worldX - (int)worldX;
                double fracY = worldY - (int)worldY;
                int texW = texture.GetLength(1);
                int texH = texture.GetLength(0);
                int texX = ((int)(fracX * texW) % texW + texW) % texW;
                int texY = ((int)(fracY * texH) % texH + texH) % texH;
                Color c = texture[texY, texX];
                if (c.A > 0) {
                    Screen.Fill(c);
                    Screen.DrawPixelDepth(column, y, (int)(depthPerp * 100));
                }
            }
        }

        private static void DrawEntities() {
            const double INV_PI_OVER_180 = 180.0 / Math.PI;
            double halfScreenWidth = Screen.gameWidth / 2.0;
            int prevColumn = column;

            foreach(Entity entity in Game.room.entities) {
                if(entity is Player || pov == null) continue;

                double entityDx = entity.x - pov.x;
                double entityDy = entity.y - pov.y;
                
                double entityDa = ((Math.Atan2(-entityDy, entityDx) * INV_PI_OVER_180 % 360) + 360) % 360;
                
                double angleDiff = ((angle - entityDa + 180) % 360) - 180;
                
                double entityColumn = halfScreenWidth + Math.Tan(angleDiff * PI_OVER_180) * halfScreenWidth / fovScale;

                double correctedDistance = Math.Max(0.01, Math.Abs(Math.Cos((entityDa - angle) * PI_OVER_180)) * DistanceBetween(entity, pov));
                int width = (int)Math.Max(1, Screen.gameHeight / fovScale / correctedDistance);
                int halfWidth = width / 2;
                
                if (entityColumn + halfWidth < 0 || entityColumn - halfWidth >= Screen.gameWidth) continue;

                double verifyDa = ((-Math.Atan((entityColumn - halfScreenWidth) / halfScreenWidth * fovScale) * INV_PI_OVER_180 + angle) % 360 + 360) % 360;
                double angleDifference = Math.Abs(verifyDa - entityDa);
                if (angleDifference > 180) angleDifference = 360 - angleDifference;
                if (angleDifference > 1.0) continue;

                if (entity.texture == null || !Game.room.textures.ContainsKey(entity.texture)) continue;

                Color[,] entityTexture = Game.room.textures[entity.texture];
                column = (int)Math.Round(entityColumn) - halfWidth;
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
