using System.ComponentModel.Design;
using System.Net;

namespace LibCast {

    //Information about each wall hit.
    public struct WallInfo {
        public double distance;
        public double rx;
        public double ry;
        public string? texture;
        public bool isTall;

        public WallInfo(double d, double x, double y, string? tex, bool tall) {
            distance = d;
            rx = x;
            ry = y;
            texture = tex;
            isTall = tall;
        }
    }


    public static class Raycaster {
        
        public static int FOV = 110;
        private const double DEG2RAD = Math.PI / 180.0;
        private const double RAD2DEG = 180.0 / Math.PI;
        private const double PI_OVER_180 = Math.PI / 180.0;
        private const int maxRaySurfaces = 15;

        private static Raycastable pov = null!;
        private static Room room = null!;

        private static double fovScale, pitch, wallHeightScale;

        public static void Go(Raycastable? view, Room gameRoom) {
            if (view == null) return;
            pov = view;
            room = gameRoom;
            pitch = (pov.pitch + pov.z)*(Screen.gameWidth/256.0);
            fovScale = Math.Tan(FOV / 2.0 * DEG2RAD);
            wallHeightScale = Screen.gameWidth / 2.0 / fovScale;

            double viewAngle = pov.direction % 360;
            double viewX = pov.x;
            double viewY = pov.y;
            
            DrawSkyBox();

            Parallel.For(0, Screen.gameWidth, col => {
                double da = -Math.Atan((col - Screen.gameWidth / 2.0) / (Screen.gameWidth / 2.0) * fovScale) * RAD2DEG + viewAngle;
                while (da < 0) da += 360;
                while (da >= 360) da -= 360;

                double dx = Math.Cos(da * DEG2RAD);
                double dy = -Math.Sin(da * DEG2RAD);

                WallInfo[] walls = CastRay(dx, dy, (int)viewX, (int)viewY, viewX, viewY, da, viewAngle);
                
                foreach (WallInfo wall in walls.Reverse()) {
                    double xOffset = wall.rx - Math.Floor(wall.rx);
                    double yOffset = wall.ry - Math.Floor(wall.ry);

                    bool hitVertical = Math.Abs(xOffset - 0.5) > Math.Abs(yOffset - 0.5);
                    double offset = Math.Clamp(hitVertical ? yOffset : xOffset, 0, 1);

                    if (hitVertical) { if (dx < 0) offset = 1.0 - offset; }
                    else { if (dy > 0) offset = 1.0 - offset; }
                    DrawRayTexture(wall.distance, offset, wall.texture, wall.isTall, col);
                }
                if(walls.Count() == 0) {
                    DrawFloorCeiling(maxRaySurfaces*2, col, da, viewAngle);
                } else {
                    DrawFloorCeiling(walls[walls.GetLength(0)-1].distance, col, da, viewAngle);
                }
            });


            DrawEntities(viewAngle);
        }

        private static bool inBounds(double x, double y) {
            return y >= 0 && y < room.getHeight() &&
                x >= 0 && x < room.getWidth((int)y);
        }

        private static WallInfo[] CastRay(double dx, double dy, int mapX, int mapY, double povX, double povY, double rayAngle, double viewAngle) {
            List<WallInfo> distances = new List<WallInfo>(maxRaySurfaces);

            double deltaDistX = (Math.Abs(dx) < 1e-9) ? 1e30 : Math.Abs(1.0 / dx);
            double deltaDistY = (Math.Abs(dy) < 1e-9) ? 1e30 : Math.Abs(1.0 / dy);
            double sideDistX, sideDistY;
            int stepX, stepY;

            if (dx < 0) { stepX = -1; sideDistX = (povX - mapX) * deltaDistX; }
            else { stepX = 1; sideDistX = (mapX + 1.0 - povX) * deltaDistX; }

            if (dy < 0) { stepY = -1; sideDistY = (povY - mapY) * deltaDistY; }
            else { stepY = 1; sideDistY = (mapY + 1.0 - povY) * deltaDistY; }

            int hitCount = 0;
            int side = 0;

            while (hitCount < 50) {
                if (sideDistX < sideDistY) {
                    sideDistX += deltaDistX; mapX += stepX; side = 0;
                } else {
                    sideDistY += deltaDistY; mapY += stepY; side = 1;
                }

                if (!inBounds(mapX, mapY)) break;

                double perpWallDist = (side == 0) ? (sideDistX - deltaDistX) : (sideDistY - deltaDistY);
                double wallHitX = povX + perpWallDist * dx;
                double wallHitY = povY + perpWallDist * dy;
                double correctedDist = perpWallDist * Math.Cos((rayAngle - viewAngle) * PI_OVER_180);

                // Inner Face Check
                int prevX = mapX - (side == 0 ? stepX : 0);
                int prevY = mapY - (side == 1 ? stepY : 0);
                if (inBounds(prevX, prevY)) {
                    RoomCell prevCell = room.room[prevY][prevX];
                    if (prevCell.texture.westIn != null || prevCell.texture.northIn != null) {
                        string? innerTex = (side == 0) ? ((stepX > 0) ? prevCell.texture.eastIn : prevCell.texture.westIn)
                                                    : ((stepY > 0) ? prevCell.texture.southIn : prevCell.texture.northIn);
                        if (innerTex != null && distances.Count < maxRaySurfaces) {
                            distances.Add(new WallInfo(correctedDist, wallHitX, wallHitY, innerTex, prevCell is TallCell));
                        }
                    }
                }

                // Outer Face Check
                RoomCell nextCell = room.room[mapY][mapX];
                if (nextCell.stopRay) {
                    string? solidTex;
                    if (side == 0) solidTex = (stepX > 0) ? nextCell.texture.westOut : nextCell.texture.eastOut;
                    else solidTex = (stepY > 0) ? nextCell.texture.northOut : nextCell.texture.southOut;

                    if (solidTex != null && distances.Count < maxRaySurfaces) {
                        distances.Add(new WallInfo(correctedDist, wallHitX, wallHitY, solidTex, nextCell is TallCell));
                    }
                    break;
                } else {
                    string? outerTex = (side == 0) ? ((stepX > 0) ? nextCell.texture.westOut : nextCell.texture.eastOut)
                                                : ((stepY > 0) ? nextCell.texture.northOut : nextCell.texture.southOut);
                    if (outerTex != null && distances.Count < maxRaySurfaces) {
                        distances.Add(new WallInfo(correctedDist, wallHitX, wallHitY, outerTex, nextCell is TallCell));
                    }
                }
                hitCount++;
            }
            return distances.ToArray();
        }


        private static void DrawRay(double distance, int column) {
            distance = Math.Max(0.01, distance);
            double height = Math.Max(1, Math.Min(Screen.gameHeight, Screen.gameHeight / distance));
            for (int i = 0; i < (int)height; i++) {
                Screen.Fill(Color.Green);
                Screen.DrawPixelDepth(column, (int)(Screen.gameHeight / 2.0 - height / 2.0 + i), (int)(distance * 100));
            }
        }

        private static void DrawRayTexture(double distance, double xPos, string? stringTexture, bool isTall, int column) {
            if (stringTexture == null || !room.textures.TryGetValue(stringTexture, out var texture)) return;

            double height = Math.Max(1, wallHeightScale / distance) * (isTall ? 2 : 1);
            double top = (Screen.gameHeight / 2.0) - height * (isTall ? 0.75 : 0.5) + pitch;

            int yStart = Math.Max(0, (int)Math.Floor(top));
            int yEnd = Math.Min(Screen.gameHeight - 1, (int)Math.Ceiling(top + height) - 1);

            int texW = texture.GetLength(1);
            int texH = texture.GetLength(0);
            int texX = Math.Clamp((int)(xPos * (texW - 1)), 0, texW - 1);

            for (int y = yStart; y <= yEnd; y++) {
                double v = (y + 0.5 - top) / height;
                if (v < 0 || v >= 1) continue;

                int texY = Math.Clamp((int)(v * (texH - 1)), 0, texH - 1);
                Color c = texture[texY, texX];
                if (c.A == 0) continue;

                Screen.DrawPixelDepth(column, y, (int)(distance * 100), c); 
            }
        }



        private static void DrawFloorCeiling(double wallDistance, int column, double da, double viewAngle) {
            double rayDirX = Math.Cos(da * PI_OVER_180);
            double rayDirY = -Math.Sin(da * PI_OVER_180);
            double cosAngleDiff = Math.Cos((da - viewAngle) * PI_OVER_180);

            if (Math.Abs(cosAngleDiff) < 1e-6) return;

            double halfHeight = Screen.gameHeight / 2.0;
            double horizon = halfHeight + pitch;

            const double cameraHeight = 0.5;
            const double ceilingWorldHeight = 1.0;

            double povX = pov!.x;
            double povY = pov.y;

            double wallHeight = Math.Max(1, wallHeightScale / (wallDistance));
            double wallTop = horizon - wallHeight * 0.5;
            double wallBottom = wallTop + wallHeight;

            for (int y = 0; y < Screen.gameHeight; y++) {
                double pixelCenterY = y + 0.5;
                if (pixelCenterY >= wallTop && pixelCenterY < wallBottom) continue;

                double dy = pixelCenterY - horizon;
                if (Math.Abs(dy) < 1e-6) continue;

                bool isFloor = dy > 0;
                double planeDelta = isFloor ? cameraHeight : ceilingWorldHeight - cameraHeight;
                double rowDistance = (planeDelta * wallHeightScale) / (Math.Abs(dy));
                if (rowDistance <= 0) continue;
                if (!isFloor && rowDistance >= wallDistance) continue;

                double rayDistance = rowDistance / cosAngleDiff;
                double worldX = povX + rayDirX * rayDistance;
                double worldY = povY + rayDirY * rayDistance;

                int cellX = (int)worldX;
                int cellY = (int)worldY;
                if (!inBounds(cellX, cellY)) continue;

                RoomCell cell = room.room[cellY][cellX];
                string? texKey = isFloor ? cell.texture.bottom : cell.texture.top;
                
                DrawTexturedSurface(texKey, worldX, worldY, rowDistance, y, column, isFloor);
            }
        }

        private static void DrawTexturedSurface(string? textureKey, double worldX, double worldY, double depthPerp, int y, int column, bool isFloor) {
            if (textureKey == null || !room.textures.TryGetValue(textureKey, out var texture)) return;

            double fracX = worldX - Math.Floor(worldX);
            double fracY = worldY - Math.Floor(worldY);

            if(!isFloor) {
                fracX = 1.0-fracX;
            }
            
            int texW = texture.GetLength(1);
            int texH = texture.GetLength(0);
            int texX = Math.Clamp((int)(fracX * texW), 0, texW - 1);
            int texY = Math.Clamp((int)(fracY * texH), 0, texH - 1);

            Color c = texture[texY, texX];
            if(c.A == 0) return;
            Screen.DrawPixelDepth(column, y, (int)(depthPerp * 100), c);
        }

        private static void DrawEntities(double viewAngle) {
            const double INV_PI_OVER_180 = 180.0 / Math.PI;
            double halfScreenWidth = Screen.gameWidth * 0.5;
            double halfScreenHeight = Screen.gameHeight * 0.5;

            List<(string texture, int x, int y, int depth, int width, double height)> entityDepthSorted = new List<(string texture, int x, int y, int depth, int width, double height)>();

            foreach (Entity entity in room.entities) {
                if (entity is Player || entity.texture == null) continue;
                if (entity.texture == null || !room.textures.ContainsKey(entity.texture)) continue;

                double dx = entity.x - pov.x;
                double dy = entity.y - pov.y;

                double entityAngle = ((Math.Atan2(-dy, dx) * INV_PI_OVER_180) + 360) % 360;
                double angleDiff = ((viewAngle - entityAngle + 180) % 360) - 180;

                double facingCos = Math.Cos(angleDiff * PI_OVER_180);
                if (facingCos <= 0.0) continue;

                double entityColumn = halfScreenWidth + Math.Tan(angleDiff * PI_OVER_180) * halfScreenWidth / fovScale;
                double correctedDistance = Math.Max(0.01, facingCos * DistanceBetween(entity, pov));

                int projectedHeight = (int)(wallHeightScale / correctedDistance);
                int width = projectedHeight;
                if (width <= 0) continue;

                int top = (int)(halfScreenHeight - (projectedHeight * 0.5) + pitch);
                int depth = (int)(correctedDistance * 100);
                if(depth > 3000) continue;
                int screenX = (int)Math.Round(entityColumn) - width / 2;

                if(entity.isTall) {
                    projectedHeight*=2;
                    top = top - (int)projectedHeight/2;
                }
                entityDepthSorted.Add((entity.texture, screenX, top, depth, width, projectedHeight));

            }

            entityDepthSorted.Sort((a, b) => b.depth.CompareTo(a.depth));

            foreach((string texture, int x, int y, int depth, int width, int height) entity in entityDepthSorted) {
                //Console.WriteLine(entity.depth);
                Screen.DrawTexture(entity.texture, entity.x, entity.y, entity.depth, entity.width, entity.height);
            }

            
        }

        

        private static void DrawSkyBox() {
            if(room.skyboxTexture == null) return;
            double angle = pov.direction % 360.0;
            if (angle < 0) angle += 360.0;


            // skybox represents full 360 degrees
            double pixelsPerDegree = Screen.gameWidth / FOV;
            int skyboxWidth = (int)(pixelsPerDegree * 360.0);

            // vertical offset (unchanged from your logic)
            int y = (int)(pitch - (pov.pitchRange*Screen.gameWidth/256.0));

            // horizontal offset based on angle
            int x = (int)((1.0 - angle / 360.0) * skyboxWidth);

            // draw twice for wrapping
            Screen.DrawBackground(0);
            Screen.DrawTexture(room.skyboxTexture, -x, y, null, skyboxWidth, Screen.gameHeight + pov.pitchRange*2*Screen.gameWidth/256);
            Screen.DrawTexture(room.skyboxTexture, -x + skyboxWidth, y, null, skyboxWidth, Screen.gameHeight + pov.pitchRange*2*Screen.gameWidth/256);
        }
    }
}
