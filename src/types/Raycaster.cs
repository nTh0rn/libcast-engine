using System.ComponentModel.Design;
using System.Net;

namespace LibCast {

    //Information about each wall hit.
    public struct WallInfo {
        public double distance;
        public double rx;
        public double ry;
        public string texture;

        public WallInfo(double d, double x, double y, string tex) {
            distance = d;
            rx = x;
            ry = y;
            texture = tex;
        }
    }


    public static class Raycaster {
        
        public static int FOV = 110;
        private const double DEG2RAD = Math.PI / 180.0;
        private const double RAD2DEG = 180.0 / Math.PI;
        private const double PI_OVER_180 = Math.PI / 180.0;
        public static int maxRaySurfaces = 32;

        private static Raycastable pov = null!;
        private static Room room = null!;

        private static double fovScale, pitch, wallHeightScale;
        private static int xPosition, yPosition, viewWidth, viewHeight, viewDepth;

        public static void Go(Raycastable? view, Room gameRoom, int xPos=0, int yPos=0, int vWidth=-1, int vHeight=-1, int dep=0) {
            xPosition = xPos;
            yPosition = yPos;
            viewDepth = dep;
            if(vWidth == -1) {
                viewWidth = Screen.gameWidth;
                viewHeight = Screen.gameHeight;
            } else {
                viewWidth = vWidth;
                viewHeight = vHeight;
            }
            
            if (view == null) return;
            pov = view;
            room = gameRoom;
            pitch = (pov.pitch + pov.z)*(viewWidth/256.0);
            fovScale = Math.Tan(FOV / 2.0 * DEG2RAD);
            wallHeightScale = viewWidth / 2.0 / fovScale;

            double viewAngle = pov.direction % 360;
            double viewX = pov.x;
            double viewY = pov.y;
            
            DrawSkyBox();
            Parallel.For(0, viewWidth, col => {
                double da = -Math.Atan((col - viewWidth / 2.0) / (viewWidth / 2.0) * fovScale) * RAD2DEG + viewAngle;
                while (da < 0) da += 360;
                while (da >= 360) da -= 360;
                DrawFloorCeiling(col, da, viewAngle);
            });
            DrawEntities(viewAngle);
            Parallel.For(0, viewWidth, col => {
                double da = -Math.Atan((col - viewWidth / 2.0) / (viewWidth / 2.0) * fovScale) * RAD2DEG + viewAngle;
                while (da < 0) da += 360;
                while (da >= 360) da -= 360;

                double dx = Math.Cos(da * DEG2RAD);
                double dy = -Math.Sin(da * DEG2RAD);
                //DrawFloorCeiling(col, da, viewAngle);
                WallInfo[] walls = CastRay(dx, dy, FloorWorldCoord(viewX), FloorWorldCoord(viewY), viewX, viewY, da, viewAngle);
                foreach (WallInfo wall in walls.Reverse()) {
                    double xOffset = wall.rx - Math.Floor(wall.rx);
                    double yOffset = wall.ry - Math.Floor(wall.ry);

                    bool hitVertical = Math.Abs(xOffset - 0.5) > Math.Abs(yOffset - 0.5);
                    double offset = Math.Clamp(hitVertical ? yOffset : xOffset, 0, 1);

                    if (hitVertical) { if (dx < 0) offset = 1.0 - offset; }
                    else { if (dy > 0) offset = 1.0 - offset; }
                    DrawRayTexture(wall.distance, offset, wall.texture, room.textures[wall.texture].height, col);
                }

                
            });



            Screen.DrawText(pov.x + " " + pov.y, 10, 10);
        }

        private static bool inBounds(double x, double y) {
            return room.room.ContainsKey((FloorWorldCoord(x), FloorWorldCoord(y)));
        }

        private static bool inBounds((double x, double y) coord) {
            return inBounds(coord.x, coord.y);
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
                    RoomCell prevCell = room.room[(prevX, prevY)];
                    if (prevCell.texture.westIn != null || prevCell.texture.northIn != null) {
                        string? innerTex = (side == 0) ? ((stepX > 0) ? prevCell.texture.eastIn : prevCell.texture.westIn)
                                                    : ((stepY > 0) ? prevCell.texture.southIn : prevCell.texture.northIn);
                        if (innerTex != null && distances.Count < maxRaySurfaces) {
                            distances.Add(new WallInfo(correctedDist, wallHitX, wallHitY, innerTex));
                        }
                    }
                }

                // Outer Face Check
                RoomCell nextCell = room.room[(mapX, mapY)];
                string? outerTex = (side == 0) ? ((stepX > 0) ? nextCell.texture.westOut : nextCell.texture.eastOut)
                                            : ((stepY > 0) ? nextCell.texture.northOut : nextCell.texture.southOut);
                if (outerTex != null && distances.Count < maxRaySurfaces) {
                    distances.Add(new WallInfo(correctedDist, wallHitX, wallHitY, outerTex));
                }
                hitCount++;
            }
            return distances.ToArray();
        }


        private static void DrawRay(double distance, int column) {
            // Ensure column stays within viewport bounds
            if (column < 0 || column >= viewWidth) return;

            distance = Math.Max(0.01, distance);
            double h = Math.Max(1, Math.Min(viewHeight, viewHeight / distance));
            for (int i = 0; i < (int)h; i++) {
                Screen.Fill(Color.Green);
                Screen.DrawPixelDepth(column + xPosition, (int)(viewHeight / 2.0 - h / 2.0 + i) + yPosition, (int)(distance * 100)+viewDepth);
            }
        }

        private static void DrawRayTexture(double distance, double xPos, string? stringTexture, int wallHeight, int column) {
            if (stringTexture == null || !room.textures.TryGetValue(stringTexture, out var texture)) return;

            double h = Math.Max(1, wallHeightScale / distance) * (wallHeight/room.textures[stringTexture].texture.GetLength(1));
            double baseHeight = wallHeightScale / distance;
            double top = viewHeight / 2.0 + pitch - h + 0.5 * baseHeight;

            int yStart = Math.Max(0, (int)Math.Floor(top));
            int yEnd = Math.Min(viewHeight - 1, (int)Math.Ceiling(top + h) - 1);

            int texW = room.textures[stringTexture].texture.GetLength(1);
            int texH = room.textures[stringTexture].texture.GetLength(0);
            int texX = Math.Clamp((int)(xPos * (texW - 1)), 0, texW - 1);

            // Ensure column stays within viewport bounds
            if (column < 0 || column >= viewWidth) return;

            for (int y = yStart; y <= yEnd; y++) {
                double v = (y + 0.5 - top) / h;
                if (v < 0 || v >= 1) continue;

                int texY = Math.Clamp((int)(v * (texH - 1)), 0, texH - 1);
                Color c = room.textures[stringTexture].texture[texY, texX];
                if (c.A == 0) continue;

                Screen.DrawPixelDepth(column + xPosition, y + yPosition, (int)(distance * 100)+viewDepth, c); 
            }
        }



        private static void DrawFloorCeiling(int column, double da, double viewAngle) {
            // Ensure column stays within viewport bounds
            if (column < 0 || column >= viewWidth) return;

            double rayDirX = Math.Cos(da * PI_OVER_180);
            double rayDirY = -Math.Sin(da * PI_OVER_180);
            double cosAngleDiff = Math.Cos((da - viewAngle) * PI_OVER_180);

            if (Math.Abs(cosAngleDiff) < 1e-6) return;

            double halfHeight = viewHeight / 2.0;
            double horizon = halfHeight + pitch;

            const double cameraHeight = 0.5;
            const double ceilingWorldHeight = 1.0;

            double povX = pov.x;
            double povY = pov.y;

            for (int y = 0; y < viewHeight; y++) {
                double pixelCenterY = y + 0.5;
                double dy = pixelCenterY - horizon;

                if (Math.Abs(dy) < 1e-6)
                    continue;

                bool isFloor = dy > 0;

                double planeDelta = isFloor
                    ? cameraHeight
                    : ceilingWorldHeight - cameraHeight;

                double rowDistance = (planeDelta * wallHeightScale) / Math.Abs(dy);
                if (rowDistance <= 0) continue;

                double rayDistance = rowDistance / cosAngleDiff;

                double worldX = povX + rayDirX * rayDistance;
                double worldY = povY + rayDirY * rayDistance;

                (int x, int y) cellPos = FloorWorldCoords((worldX, worldY));

                if (!inBounds(cellPos)) continue;

                RoomCell cell = room.room[(cellPos.x, cellPos.y)];
                string? texKey = isFloor
                    ? cell.texture.bottom
                    : cell.texture.top;

                if (texKey == null) continue;

                DrawTexturedSurface(texKey, worldX, worldY, rowDistance, y, column, isFloor);
            }
        }


        private static void DrawTexturedSurface(string? textureKey, double worldX, double worldY, double depthPerp, int y, int column, bool isFloor) {
            // Ensure column and row stay within viewport bounds
            if (column < 0 || column >= viewWidth || y < 0 || y >= viewHeight) return;

            if (textureKey == null || !room.textures.TryGetValue(textureKey, out var texture)) return;

            double fracX = worldX - Math.Floor(worldX);
            double fracY = worldY - Math.Floor(worldY);

            if(!isFloor) {
                fracX = 1.0-fracX;
            }
            
            int texW = room.textures[textureKey].texture.GetLength(1);
            int texH = room.textures[textureKey].texture.GetLength(0);
            int texX = Math.Clamp((int)(fracX * texW), 0, texW - 1);
            int texY = Math.Clamp((int)(fracY * texH), 0, texH - 1);

            Color c = room.textures[textureKey].texture[texY, texX];
            if(c.A == 0) return;
            Screen.DrawPixelDepth(column + xPosition, y + yPosition, (int)(depthPerp * 100)+viewDepth, c);
        }

        private static void DrawEntities(double viewAngle)
{
    const double INV_PI_OVER_180 = 180.0 / Math.PI;

    double halfScreenWidth = viewWidth * 0.5;
    double halfScreenHeight = viewHeight * 0.5;

    List<(string texture, int x, int y, int depth, int width, int height)>
        entityDepthSorted =
        new List<(string, int, int, int, int, int)>();

    foreach (Entity entity in room.entitiesInRange)
    {
        if (entity == pov || entity.texture == null)
            continue;

        double dx = entity.x - pov.x;
        double dy = entity.y - pov.y;

        // Angle to entity
        double entityAngle =
            ((Math.Atan2(-dy, dx) * INV_PI_OVER_180) + 360) % 360;

        double angleDiff =
            ((viewAngle - entityAngle + 180) % 360) - 180;

        double facingCos = Math.Cos(angleDiff * PI_OVER_180);

        // Behind camera
        if (facingCos <= 0.0)
            continue;

        // Horizontal projection
        double entityColumn =
            halfScreenWidth +
            Math.Tan(angleDiff * PI_OVER_180) *
            halfScreenWidth / fovScale;

        double correctedDistance =
            Math.Max(0.01, facingCos * DistanceBetween(entity, pov));

        // Base projected size (256 reference)
        int baseSize =
            (int)(wallHeightScale / correctedDistance);

        if (baseSize <= 0)
            continue;

        int width = baseSize;

        int depth = (int)(correctedDistance * 100);

        if (depth > 3000)
            continue;

        int screenX =
            (int)Math.Round(entityColumn) - width / 2;

        /* ---------------- Direction / Texture ---------------- */

        int d1 = (int)entity.direction % 360;
        if (d1 < 0) d1 += 360;

        int d2 = (int)pov.direction % 360;
        if (d2 < 0) d2 += 360;

        int delta = d1 - d2;

        if (delta > 180) delta -= 360;
        if (delta < -180) delta += 360;

        int absDelta = Math.Abs(delta);

        var texSet = entity.texture.Value;

        string texture;

        if (absDelta <= 45)
        {
            texture = texSet.back;
        }
        else if (absDelta <= 135)
        {
            texture = delta > 0
                ? texSet.left
                : texSet.right;
        }
        else
        {
            texture = texSet.front;
        }

        /* ---------------- Height from Aspect ---------------- */

        var tex =
            room.textures[texture].texture;

        int texW = tex.GetLength(1);
        int texH = tex.GetLength(0);

        // Aspect-based height
        int drawHeight =
            baseSize * texH / texW;

        /* ---------------- Vertical Placement ---------------- */

        // Floor line = center + pitch
        int floorY =
    (int)(halfScreenHeight + pitch + baseSize / 2.0);

        // Bottom-aligned
        int top = floorY - drawHeight;

        /* ---------------- Store ---------------- */

        entityDepthSorted.Add((
            texture,
            screenX,
            top,
            depth,
            width,
            drawHeight
        ));
    }

    /* -------- Depth Sort -------- */

    entityDepthSorted.Sort(
        (a, b) => b.depth.CompareTo(a.depth)
    );

    /* -------- Draw -------- */

    foreach (
        (string texture, int x, int y,
         int depth, int width, int height)
        entity in entityDepthSorted
    )
    {
        int screenX = entity.x + xPosition;
        int screenY = entity.y + yPosition;

        int screenRightX = screenX + entity.width;
        int screenBottomY = screenY + entity.height;

        int viewRightX = xPosition + viewWidth;
        int viewBottomY = yPosition + viewHeight;

        // Viewport clip
        if (screenRightX <= xPosition ||
            screenX >= viewRightX ||
            screenBottomY <= yPosition ||
            screenY >= viewBottomY)
            continue;

        Screen.DrawTexture(
            entity.texture,
            screenX,
            screenY,
            entity.depth + viewDepth,
            entity.width,
            entity.height,
            xPosition,
            yPosition,
            viewWidth,
            viewHeight
        );
    }
}


        

        private static void DrawSkyBox() {

    if (room.skyboxTexture == null) return;

    double angle = pov.direction % 360.0;
    if (angle < 0) angle += 360.0;


    double pixelsPerDegree = viewWidth / (double)FOV;
    int skyboxWidth = (int)(pixelsPerDegree * 360.0);


    int skyHeight =
        viewHeight +
        (int)(pov.pitchRange * 2 * viewWidth / 256.0);


    int y =
        (int)(pitch - (pov.pitchRange * viewWidth / 256.0)) +
        yPosition;


    int x =
        (int)((1.0 - angle / 360.0) * skyboxWidth);




    int baseX = xPosition - x;


    Screen.DrawTexture(
        room.skyboxTexture,
        baseX,
        y,
        null,
        skyboxWidth,
        skyHeight,
        xPosition,
        yPosition,
        viewWidth,
        viewHeight
    );

    Screen.DrawTexture(
        room.skyboxTexture,
        baseX + skyboxWidth,
        y,
       null,
        skyboxWidth,
        skyHeight,
        xPosition,
        yPosition,
        viewWidth,
        viewHeight
    );
}

    }
}
