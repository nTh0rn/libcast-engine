namespace LibCasgt {

    public struct PixelDepth {
        public Pixel pixel {get; set;}
        public double depth {get; set;}
    }
    public struct DistanceRXY {
        public double distance, rx, ry;
        public string? texture;
        public bool isTall;

        public DistanceRXY(double d, double x, double y, string? t, bool tall) {
            distance = d; rx = x; ry = y; texture = t; isTall = tall;
        }
    }
    public class Raycaster {
        
private static int column;
    private static Entity? pov;
    private static double da, dx, dy, fovScale, pitch, wallHeightScale, distanceScale;
    public static int FOV = 110;
    private const double DEG2RAD = Math.PI / 180.0;

    public struct DistanceHit {
        public double dist, wx, wy, wallX;
        public string? tex;
        public bool isTall;
        public DistanceHit(double d, double x, double y, double wX, string? t, bool tall) {
            dist = d; wx = x; wy = y; wallX = wX; tex = t; isTall = tall;
        }
    }

    public static void Go(Entity view) {
        pov = view;
        pitch = Game.room.player?.pitch ?? 0.0;
        distanceScale = 1.0 + Math.Abs(pitch / 30.0) * 0.0; // Set scale factor here
        fovScale = Math.Tan((FOV / 2.0) * DEG2RAD);
        wallHeightScale = (Screen.gameWidth / 2.0) / fovScale;

        for (column = 0; column < Screen.gameWidth; column++) {
            da = -Math.Atan((column - Screen.gameWidth / 2.0) / (Screen.gameWidth / 2.0) * fovScale) * (180.0 / Math.PI) + pov.direction;
            dx = Math.Cos(da * DEG2RAD); 
            dy = -Math.Sin(da * DEG2RAD);

            foreach (var hit in CastRay()) {
                // Pass distance AND world coordinates to floor/ceiling logic
                DrawFloorCeiling(hit.dist, hit.wx, hit.wy); 
                DrawRayTexture(hit.dist, hit.wallX, hit.tex, hit.isTall);
            }
        }
        DrawEntities();
    }

    private static List<DistanceHit> CastRay() {
        var hits = new List<DistanceHit>();
        int mx = (int)pov!.x, my = (int)pov.y;
        double ddx = Math.Abs(1 / dx), ddy = Math.Abs(1 / dy);
        int sx = dx < 0 ? -1 : 1, sy = dy < 0 ? -1 : 1;
        double sdx = (dx < 0 ? pov.x - mx : mx + 1 - pov.x) * ddx;
        double sdy = (dy < 0 ? pov.y - my : my + 1 - pov.y) * ddy;

        for (int i = 0; i < 50; i++) {
            int side = (sdx < sdy) ? 0 : 1;
            if (side == 0) { sdx += ddx; mx += sx; } else { sdy += ddy; my += sy; }

            if (my < 0 || my >= Game.room.getHeight() || mx < 0 || mx >= Game.room.getWidth(my)) break;

            double dist = (side == 0 ? sdx - ddx : sdy - ddy);
            
            // Calculate exact world coordinates where the ray hit the wall/boundary
            double worldX = pov.x + dist * dx;
            double worldY = pov.y + dist * dy;

            // Texture offset (0.0 to 1.0)
            double wallX = (side == 0) ? worldY : worldX;
            wallX -= Math.Floor(wallX);
            if ((side == 0 && dx > 0) || (side == 1 && dy < 0)) wallX = 1 - wallX;

            double corrDist = dist * Math.Cos((da - pov.direction) * DEG2RAD);
            RoomCell prev = Game.room.room[my - (side == 1 ? sy : 0)][mx - (side == 0 ? sx : 0)];
            RoomCell next = Game.room.room[my][mx];

            string? inT = (side == 0) ? (sx > 0 ? prev.facesInside.right : prev.facesInside.left) : (sy > 0 ? prev.facesInside.down : prev.facesInside.up);
            if (inT != null) hits.Add(new DistanceHit(corrDist, worldX, worldY, wallX, inT, prev is TallCell));

            string? outT = (side == 0) ? (sx > 0 ? next.facesOutside.left : next.facesOutside.right) : (sy > 0 ? next.facesOutside.up : next.facesOutside.down);
            if (outT != null) hits.Add(new DistanceHit(corrDist, worldX, worldY, wallX, outT, next is TallCell));

            if (hits.Count >= 10) break;
        }
        return hits;
    }

    private static void DrawRayTexture(double dist, double xPos, string? texName, bool tall) {
        if (texName == null) return;
        Color[,] tex = Game.room.textures[texName];
        double d = Math.Max(0.01, dist * (1.0 + Math.Abs(pitch / 30.0) * 0.0)); // Inlined distanceScale
        double h = (wallHeightScale / d) * (tall ? 2 : 1);
        double top = (Screen.gameHeight / 2.0) - h * (tall ? 0.75 : 0.5) + pitch;

        int th = tex.GetLength(0) - 1, tw = tex.GetLength(1) - 1;
        int yStart = Math.Max(0, (int)top), yEnd = Math.Min(Screen.gameHeight - 1, (int)(top + h));

        for (int y = yStart; y <= yEnd; y++) {
            int ty = (int)(((y - top) / h) * th);
            Color c = tex[Math.Clamp(ty, 0, th), (int)(xPos * tw)];
            if (c.A == 0) continue;
            Screen.Fill(c);
            Screen.DrawPixelDepth(column, y, (int)(d * 100));
        }
    }
    public static bool inBounds(double x, double y) {
        return y >= 0 && y < Game.room.getHeight() &&
               x >= 0 && x < Game.room.getWidth((int)y);
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

        

        private static void DrawFloorCeiling(double wallDistance, double hitX, double hitY) {
    double cosDiff = Math.Cos((da - pov!.direction) * DEG2RAD);
    if (Math.Abs(cosDiff) < 1e-6) return;

    double horizon = (Screen.gameHeight / 2.0) + pitch;
    const double camH = 0.5, ceilH = 1.0;

    // Calculate the screen bounds of the wall segment
    double wH = Math.Max(1, wallHeightScale / (wallDistance * distanceScale));
    double wTop = horizon - wH * 0.5, wBottom = wTop + wH;

    for (int y = 0; y < Screen.gameHeight; y++) {
        double py = y + 0.5;
        if (py >= wTop && py < wBottom) continue; // Skip if we are on the wall

        double dyCenter = py - horizon;
        if (Math.Abs(dyCenter) < 1e-6) continue;

        bool isFloor = dyCenter > 0;
        double planeHeight = isFloor ? camH : ceilH - camH;

        // rowDistance is the perpendicular distance to the floor/ceiling point
        double rowDist = (planeHeight * wallHeightScale) / (Math.Abs(dyCenter) * distanceScale);
        
        // Only draw ceiling if it's closer than the wall; floors draw up to the wall
        if (rowDist <= 0 || (!isFloor && rowDist >= wallDistance)) continue;

        // Project the point into world space using the ray direction
        double rayDist = rowDist / cosDiff;
        double wx = pov.x + dx * rayDist;
        double wy = pov.y + dy * rayDist;

        int cx = (int)wx, cy = (int)wy;
        if (!inBounds(cx, cy)) continue;

        RoomCell cell = Game.room.room[cy][cx];
        string? tex = isFloor ? cell.floorTexture : cell.ceilingTexture;
        
        if (tex != null) {
            DrawTexturedSurface(tex, wx, wy, rowDist, y);
        }
    }
}

        private static void DrawTexturedSurface(
            string? textureKey,
            double worldX,
            double worldY,
            double depthPerp,
            int y
        ) {
            if (textureKey == null) return;
            if (!Game.room.textures.TryGetValue(textureKey, out var texture)) return;

            double fracX = worldX - Math.Floor(worldX);
            double fracY = worldY - Math.Floor(worldY);

            int texW = texture.GetLength(1);
            int texH = texture.GetLength(0);

            int texX = (int)(fracX * texW);
            int texY = (int)(fracY * texH);

            texX = (texX % texW + texW) % texW;
            texY = (texY % texH + texH) % texH;

            Color c = texture[texY, texX];
            if (c.A == 0) return;

            Screen.Fill(c);
            Screen.DrawPixelDepth(column, y, (int)(depthPerp * 100));
        }

        private static void DrawEntities() {
            const double INV_DEG2RAD = 180.0 / Math.PI;

            double halfScreenWidth  = Screen.gameWidth * 0.5;
            double halfScreenHeight = Screen.gameHeight * 0.5;

            foreach (Entity entity in Game.room.entities) {
                if (entity is Player || pov == null) continue;

                string texName = entity.texture;
                if (texName == null || !Game.room.textures.ContainsKey(texName)) continue;

                double dx = entity.x - pov.x;
                double dy = entity.y - pov.y;

                double entityAngle =
                    ((Math.Atan2(-dy, dx) * INV_DEG2RAD) + 360) % 360;

                double angleDiff = ((pov.direction - entityAngle + 180) % 360) - 180;

                // === FRONT-HEMISPHERE TEST (THE FIX) ===
                double facingCos = Math.Cos(angleDiff * DEG2RAD);
                if (facingCos <= 0.0)
                    continue;

                double entityColumn =
                    halfScreenWidth +
                    Math.Tan(angleDiff * DEG2RAD) *
                    halfScreenWidth / fovScale;

                double correctedDistance =
                    Math.Max(0.01, facingCos * DistanceBetween(entity, pov));

                double distanceScaled = correctedDistance * distanceScale;

                // Single, consistent projection scale
                double projectedHeight = wallHeightScale / distanceScaled;
                int width = (int)Math.Round(projectedHeight);

                if (width <= 0) continue;

                double top =
                    halfScreenHeight - projectedHeight * 0.5 + pitch;

                int depth = (int)(distanceScaled * 100);

                int screenX = (int)Math.Round(entityColumn) - width / 2;

                DrawTexture(
                    texName,
                    screenX,
                    top,
                    depth,
                    width,
                    projectedHeight
                );
            }
        }

        private static void DrawTexture(
            string texture,
            int x,
            double y,
            int depth = int.MaxValue,
            int width = 0,
            double height = 0
        ) {
            Color[,] tex = Game.room.textures[texture];
            int texH = tex.GetLength(0);
            int texW = tex.GetLength(1);

            if (width <= 0 && height <= 0) {
                width = texW;
                height = texH;
            }
            else if (width <= 0) {
                width = (int)Math.Round(height * texW / texH);
            }
            else if (height <= 0) {
                height = width * (double)texH / texW;
            }

            if (width <= 0 || height <= 0) return;

            double invW = 1.0 / width;
            double invH = 1.0 / height;

            int yStart = Math.Max(0, (int)Math.Floor(y));
            int yEnd   = Math.Min(Screen.gameHeight - 1,
                                (int)Math.Ceiling(y + height) - 1);

            for (int sx = Math.Max(0, x);
                sx < x + width && sx < Screen.gameWidth;
                sx++) {

                double u = (sx - x + 0.5) * invW;
                int texX = (int)(u * (texW - 1));

                for (int sy = yStart; sy <= yEnd; sy++) {
                    double centerY = sy + 0.5;
                    if (centerY < y || centerY >= y + height) continue;

                    double v = (centerY - y) * invH;
                    int texY = (int)(v * (texH - 1));

                    Color c = tex[texY, texX];
                    if (c.A == 0) continue;

                    Screen.Fill(c);
                    Screen.DrawPixelDepth(sx, sy, depth);
                }
            }
        }
    }
}
