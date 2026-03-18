using System.Numerics;

namespace LibCast {

    public struct Pixel {
        public Color fillColor {get; set;}
        public Color strokeColor {get; set;}= Screen.emptyColor;
        public int depth {get; set;} = int.MaxValue;
        public char character {get; set;} = ' ';
        public int fontSize {get; set;} = Screen.pixelScale*2;
        public MouseCollision ?collisionFamily {get; set;}

        public Pixel(Color fillColor) {
            this.fillColor = fillColor;
        }

        public Pixel(Pixel copy, int newDepth){
            fillColor = copy.fillColor;
            strokeColor = copy.strokeColor;
            depth = newDepth;
            character = copy.character;
            collisionFamily = copy.collisionFamily;
        }

        public Pixel(Color fillColor, Color strokeColor, MouseCollision? collisionFamily = null) {
            this.fillColor = fillColor;
            this.strokeColor = strokeColor;
            this.character = character;
            this.collisionFamily = collisionFamily;
        }

        public Pixel(Color fillColor, Color strokeColor, char character, int fontSize, MouseCollision? collisionFamily = null) {
            this.fillColor = fillColor;
            this.strokeColor = strokeColor;
            this.character = character;
            this.fontSize = fontSize;
            this.collisionFamily = collisionFamily;
        }

        public Pixel(Color fillColor, Color strokeColor, int depth, char character, int fontSize, MouseCollision? collisionFamily = null) {
            this.fillColor = fillColor;
            this.strokeColor = strokeColor;
            this.depth = depth;
            this.character = character;
            this.fontSize = fontSize;
            this.collisionFamily = collisionFamily;
        }
    }

    

    
    public class Screen {

        public static readonly (int width, int height) playGameResolution = (320, 180);
        public static readonly (int width, int height) menuGameResolution = (128, 72);

        public static readonly (int width, int height) playBufferResolution = (1280, 720);
        public static readonly (int width, int height) menuBufferResolution = (1280, 720);

        //Game scaling 
        public static int windowWidth = 1280;
        public static int windowHeight = 720;
        public static int bufferWidth = playBufferResolution.width;
        public static int bufferHeight = playBufferResolution.height;
        public static int gameWidth = playGameResolution.width;
        public static int gameHeight = playGameResolution.height;
        public static int pixelScale = bufferWidth/gameWidth;
        public static RenderTexture2D target;
        public static double scale;
        public static Vector2 mouse;
        public static Vector2 virtualMouse;

        //Game framerate control
        public static double previousTime = Raylib.GetTime();
        public static double currentTime = 0;
        public static double updateDrawTime = 0;
        public static double waitTime = 0;
        public static double deltaTime = 0;
        public static double deltaTimeRatio = 0;
        public static double timeCounter = 0;
        public static int targetFPS = 60;
        public static bool pause = false;

        public static Font smallTextFont;

        public static List<List<Pixel>> screen = new List<List<Pixel>>();


        public static readonly Color emptyColor = new Color(255, 255, 255, 0);
        public static Color fillColor = Color.Black;
        public static Color strokeColor = Color.White;

        public static bool showFPS = true;

        public static bool resized = false;

        public static MouseCollision ?currentMouseCollision;


        public static void Init() {
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);
            Raylib.InitWindow(1280, 720, "Libcast");
            Raylib.SetWindowMinSize(640, 360);
            ApplyBuffer();
            //smallTextFont = Raylib.LoadFontEx("src/assets/font/consolas.ttf", 16, null, 0);
            //smallTextFont = Raylib.LoadFontEx("src/assets/font/pixeled2.ttf", 32, null, 0);
            smallTextFont = Raylib.LoadFontEx("src/assets/font/custom.ttf", 128, null, 0);
            Raylib.SetTextureFilter(smallTextFont.Texture, TextureFilter.Point);

            Raylib.SetTargetFPS(targetFPS);

        }


        public static void LoopStart() {
            timeCounter += deltaTime;
            scale = Math.Min((double)Raylib.GetScreenWidth() / bufferWidth, (double)Raylib.GetScreenHeight() / bufferHeight);
            mouse = Raylib.GetMousePosition();
            virtualMouse = new Vector2(0);

            virtualMouse.X = (float)((mouse.X - (Raylib.GetScreenWidth() - (bufferWidth * scale)) * 0.5) / scale);
            virtualMouse.Y = (float)((mouse.Y - (Raylib.GetScreenHeight() - (bufferHeight * scale)) * 0.5) / scale);
            virtualMouse = Raymath.Vector2Clamp(
                virtualMouse,
                new Vector2(0, 0),
                new Vector2(bufferWidth, bufferHeight)
            );

            Raylib.BeginTextureMode(target);
        }

        public static void LoopEnd() {
            Raylib.EndTextureMode();
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawTexturePro(
                target.Texture,
                new Rectangle(0, 0, target.Texture.Width, -target.Texture.Height),
                new Rectangle(
                    (float)((Raylib.GetScreenWidth() - (bufferWidth * scale)) * 0.5),
                    (float)((Raylib.GetScreenHeight() - (bufferHeight * scale)) * 0.5),
                    (float)(bufferWidth * scale),
                    (float)(bufferHeight * scale)
                ),
                new Vector2(0, 0),
                0,
                Color.White
            );
            Raylib.EndDrawing();

            if (Raylib.IsKeyPressed(KeyboardKey.F11)) {
                ToggleFullscreen();
            }

            deltaTime = Raylib.GetFrameTime();
            deltaTimeRatio = targetFPS/(1.0/deltaTime);
        }

        public static void Close() {
            Raylib.UnloadRenderTexture(target);
            Raylib.CloseWindow();
        }


        public static void Draw() {
            int fontPixelScale = (int)(pixelScale/6.0);
            for (int y = screen.Count - 1; y >= 0; y--) {
                for (int x = screen[y].Count - 1; x >= 0; x--) {
                    Raylib.DrawRectangle(x * pixelScale, y * pixelScale, pixelScale, pixelScale, screen[y][x].fillColor);
                    if (screen[y][x].character != ' ') {
                        char character = screen[y][x].character;
                        int fontSize = screen[y][x].fontSize*pixelScale;
                        if (fontSize < 0) fontSize = pixelScale*2;
                        Raylib.DrawTextEx(smallTextFont, character.ToString(), new Vector2(x * pixelScale+fontPixelScale, (int)(y * pixelScale+fontPixelScale*3)),(int) (fontSize*(1-1.0/6.0)) , 0, screen[y][x].strokeColor);

                    }
                }
            }
        }

        public static void Resize() {
            screen = new List<List<Pixel>>();
            for (int y = 0; y < gameHeight; y++) {
                screen.Add(new List<Pixel>());
                for (int x = 0; x < gameWidth; x++) {
                    screen[y].Add(new Pixel(emptyColor));
                }
            }
        }

        public static void DrawBackground(Color color) {
            Color oldFill = fillColor;
            Fill(color);
            DrawRect(0, 0, gameWidth, gameHeight);
            Fill(oldFill);
        }

        public static void DrawBackground(int r, int g, int b, int a=255) {
            DrawBackground(new Color(r, g, b, a));
        }

        public static void DrawBackground(int c) {
            DrawBackground(c, c, c);
        }

        public static void DrawRect(int x, int y, int w, int h) {
            for (int j = 0; j < h; j++) {
                for (int i = 0; i < w; i++) {
                    DrawPixel(x + i, y + j);
                }
            }
        }



        public static void DrawPixel(int x, int y, Color color, int depth, char character, int fontSize) {
            byte a = color.A;
            if (a != 255) {
                int r = (color.R * a + screen[y][x].fillColor.R * (255 - a)) / 255;
                int g = (color.G * a + screen[y][x].fillColor.G * (255 - a)) / 255;
                int b = (color.B * a + screen[y][x].fillColor.B * (255 - a)) / 255;
                screen[y][x] = new Pixel(new Color(r, g, b, 255), strokeColor, depth, ' ', fontSize, currentMouseCollision);
            } else {
                screen[y][x] = new Pixel(color, strokeColor, depth, character, fontSize, currentMouseCollision);
            }
        }

        public static void DrawPixel(int x, int y, Color color, int depth=int.MaxValue) {
            DrawPixel(x, y, color, depth, ' ', 0);
        }
        
        public static void DrawPixel(int x, int y, int depth=int.MaxValue,  char character=' ', int fontSize=-1) {
            DrawPixel(x, y, fillColor, depth, character, fontSize);
        }

        public static void DrawPixelDepth(int x, int y, Color color, int depth) {
            if(screen[y][x].depth >= depth) {
                DrawPixel(x, y, color, depth, ' ', 0);
            }
            //DrawPixel(x, y, depth, color);
        }

        public static void DrawPixelDepth(int x, int y, int depth) {
            DrawPixelDepth(x, y, fillColor, depth);
        }

        

        public static void DrawBox(int x, int y, int w, int h, int strokeSize) {
            Color oldFill = fillColor;
            Fill(strokeColor);
            DrawRect(x, y, w, h);
            Fill(oldFill);
            DrawRect(x + strokeSize, y + strokeSize, w - strokeSize * 2, h - strokeSize * 2);
        }

        public static void Fill(Color color) {
            fillColor = new Color(color.R, color.G, color.B, color.A);
        }

        public static void Fill(int r, int g, int b, int a = 255) {
            Fill(new Color(r, g, b, a));
        }

        public static void Fill(int color) {
            Fill(color, color, color);
        }

        public static void Stroke(Color color) {
            strokeColor = new Color(color.R, color.G, color.B, color.A);
        }

        public static void Stroke(int r, int g, int b, int a = 255) {
            Stroke(new Color(r, g, b, a));
        }

        public static void Stroke(int color) {
            Stroke(color, color, color);
        }

        // public static void DrawText(string input, int x, int y, int fontSize=-1, bool FillBackground=false) {
        //     int xOffset = 0;
        //     foreach (char character in input) {
        //         Pixel pixel = new Pixel(FillBackground ? fillColor : screen[y][xOffset].fillColor, strokeColor, character, fontSize, currentMouseCollision);
        //         if(FillBackground) {
        //             screen[y+1][x + xOffset] = new Pixel(fillColor, strokeColor, screen[y+1][xOffset].character, fontSize, currentMouseCollision);
        //         }
        //         screen[y][x + xOffset] = pixel;
        //         xOffset++;

        //     }
        // }

        public static void DrawText(string input, int x, int y, int fontSize=-1, bool FillBackground=false) {
            int pixelsPerCharacter = (int)Math.Ceiling(Math.Max(1.0, fontSize/2.0));
            if(FillBackground) {
                DrawRect(x, y, input.Length * pixelsPerCharacter, fontSize);
            }
            for(int xOffset = 0; xOffset < input.Length*pixelsPerCharacter; xOffset++) {
                Pixel pixel;
                if(xOffset % pixelsPerCharacter == 0) {
                    pixel = new Pixel(FillBackground ? fillColor : screen[y][x+xOffset].fillColor, strokeColor, input[(int)(xOffset / pixelsPerCharacter)], fontSize, currentMouseCollision);
                } else {
                    pixel = new Pixel(FillBackground ? fillColor : screen[y][x+xOffset].fillColor, strokeColor, ' ', 0, currentMouseCollision);
                }
                screen[y][x + xOffset] = pixel;
            }
        }

        public static void DrawLine(double x1, double y1, double x2, double y2) {
            // Bresenham's line algorithm
            int dx = Math.Abs((int)Math.Round(x2) - (int)Math.Round(x1));
            int dy = Math.Abs((int)Math.Round(y2) - (int)Math.Round(y1));
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;
            
            int x = (int)Math.Round(x1);
            int y = (int)Math.Round(y1);
            int x2Int = (int)Math.Round(x2);
            int y2Int = (int)Math.Round(y2);
            
            while (true) {
                if (x >= 0 && x < gameWidth && y >= 0 && y < gameHeight) {
                    DrawPixel(x, y);
                }
                
                if (x == x2Int && y == y2Int) break;
                
                int e2 = 2 * err;
                if (e2 > -dy) {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dx) {
                    err += dx;
                    y += sy;
                }
            }
        }

        public static void DrawCircle(int x, int y, int radius) {
            // Midpoint circle algorithm (Bresenham's circle)
            int xPos = radius;
            int yPos = 0;
            int err = 0;
            
            while (xPos >= yPos) {
                // Draw 8 octants of the circle
                if (x + xPos >= 0 && x + xPos < gameWidth && y + yPos >= 0 && y + yPos < gameHeight)
                    DrawPixel(x + xPos, y + yPos);
                if (x + yPos >= 0 && x + yPos < gameWidth && y + xPos >= 0 && y + xPos < gameHeight)
                    DrawPixel(x + yPos, y + xPos);
                if (x - yPos >= 0 && x - yPos < gameWidth && y + xPos >= 0 && y + xPos < gameHeight)
                    DrawPixel(x - yPos, y + xPos);
                if (x - xPos >= 0 && x - xPos < gameWidth && y + yPos >= 0 && y + yPos < gameHeight)
                    DrawPixel(x - xPos, y + yPos);
                if (x - xPos >= 0 && x - xPos < gameWidth && y - yPos >= 0 && y - yPos < gameHeight)
                    DrawPixel(x - xPos, y - yPos);
                if (x - yPos >= 0 && x - yPos < gameWidth && y - xPos >= 0 && y - xPos < gameHeight)
                    DrawPixel(x - yPos, y - xPos);
                if (x + yPos >= 0 && x + yPos < gameWidth && y - xPos >= 0 && y - xPos < gameHeight)
                    DrawPixel(x + yPos, y - xPos);
                if (x + xPos >= 0 && x + xPos < gameWidth && y - yPos >= 0 && y - yPos < gameHeight)
                    DrawPixel(x + xPos, y - yPos);
                
                if (err <= 0) {
                    yPos += 1;
                    err += 2 * yPos + 1;
                }
                if (err > 0) {
                    xPos -= 1;
                    err -= 2 * xPos + 1;
                }
            }
        }

        public static void DrawFPS() {
            Color oldStroke = strokeColor;
            Stroke(Color.Green);
            DrawText(Raylib.GetFPS().ToString(), 0, 0, 6);
            DrawText(deltaTime.ToString(), 0, 8, 6);
            DrawText(deltaTimeRatio.ToString(), 0, 16, 6);

            Stroke(oldStroke);
        }

        public static void DrawTexture(
            string texture,
            int x,
            int y,
            int? depth = int.MaxValue,
            int width = 0,
            int height = 0,
            int viewX = 0,
            int viewY = 0,
            int viewW = -1,
            int viewH = -1
        ) {
            if(viewW == -1) viewW = Screen.gameWidth;
            if(viewH == -1) viewH = Screen.gameHeight;
            Color[,] tex = Game.room.GetTexture(texture).texture;
            int texH = tex.GetLength(0);
            int texW = tex.GetLength(1);

            double invW = 1.0 / width;
            double invH = 1.0 / height;


            /* Viewport bounds */
            int viewLeft   = viewX;
            int viewTop    = viewY;
            int viewRight  = viewX + viewW;
            int viewBottom = viewY + viewH;


            /* Destination bounds */
            int dstLeft   = x;
            int dstTop    = y;
            int dstRight  = x + width;
            int dstBottom = y + height;


            /* Early reject */
            if (dstRight  <= viewLeft ||
                dstLeft   >= viewRight ||
                dstBottom <= viewTop ||
                dstTop    >= viewBottom)
                return;


            /* Clipped draw area */
            int clipLeft   = Math.Max(dstLeft, viewLeft);
            int clipTop    = Math.Max(dstTop, viewTop);
            int clipRight  = Math.Min(dstRight, viewRight);
            int clipBottom = Math.Min(dstBottom, viewBottom);


            for (int sy = clipTop; sy < clipBottom; sy++) {

                double v = (sy + 0.5 - dstTop) * invH;
                if (v < 0 || v >= 1) continue;

                int texY = Math.Clamp((int)(v * texH), 0, texH - 1);

                for (int sx = clipLeft; sx < clipRight; sx++) {

                    double u = (sx + 0.5 - dstLeft) * invW;
                    if (u < 0 || u >= 1) continue;

                    int texX = Math.Clamp((int)(u * texW), 0, texW - 1);

                    Color c = tex[texY, texX];
                    if (c.A == 0) continue;

                    if (depth != null) {
                        Screen.DrawPixelDepth(sx, sy, c, (int)depth);
                    } else {
                        Screen.DrawPixel(sx, sy, c);
                    }
                }
            }
        }

        public static void BakeScreenDepth() {
            for(int y = 0; y < gameHeight; y++) {
                for(int x = 0; x < gameWidth; x++) {
                    screen[y][x] = new Pixel(screen[y][x], int.MaxValue);
                }
            }
        }


        

        // public static void DrawText(int x, int y, string input) {
        //     for (int i = 0; i < input.Length; i++) {
        //         try {
        //             DrawPixel(x + i, y, input.Substring(i, 1));
        //         }
        //         catch (Exception) {
        //         }
        //     }
        // }

        public static void SetWindowSize(int windowWidth, int windowHeight) {
            Screen.windowWidth = windowWidth;
            Screen.windowHeight = windowHeight;
            Close();
            Program.Init();
        }

        public static void SetFullscreen(bool fullscreen) {
            Raylib.SetWindowState(ConfigFlags.ResizableWindow);
            Raylib.SetWindowPosition(50, 50);
            if(fullscreen != IsFullscreen()) {
                if(fullscreen) {
                Raylib.SetWindowSize(1920, 1080);
                } else {
                Raylib.SetWindowSize(1280, 720);
                }
                Raylib.ToggleFullscreen();
                
            }
        }

        public static void ToggleFullscreen() {
            SetFullscreen(!IsFullscreen());
        }

        public static bool IsFullscreen() {
            return Raylib.IsWindowState(ConfigFlags.FullscreenMode);
        }

        public static void ApplyScreenSettings(int windowWidth, int windowHeight, int bufferWidth, int bufferHeight, int gameWidth, int gameHeight, bool fullscreen) {
            if(IsFullscreen()) {
                Raylib.ToggleFullscreen();
            }
            Raylib.SetWindowState(ConfigFlags.ResizableWindow);
            Raylib.SetWindowPosition(0, 50);
            SetBufferSize((bufferWidth, bufferHeight));
            SetWindowSize(windowWidth, windowHeight);
            SetGameSize((gameWidth, gameHeight));
            SetFullscreen(fullscreen);
        }

        public static void SetGameSize((int width, int height) res) {
            gameWidth = res.width;
            gameHeight = res.height;
        }

        public static void SetBufferSize((int width, int height) res) {
            bufferWidth = res.width;
            bufferHeight = res.height;
        }

        public static void ApplyBuffer() {
            target = Raylib.LoadRenderTexture(bufferWidth, bufferHeight);
            Raylib.SetTextureFilter(target.Texture, TextureFilter.Point);
        }

        public static void SetAndApplyBuffer((int width, int height) res) {
            SetBufferSize(res);
            ApplyBuffer();
        }

        public static void SetPixelScale(int pixelScale) {
            Screen.pixelScale = pixelScale;
        }

        public static void StartCollisionDraw(MouseCollision mouseCollision) {
            currentMouseCollision = mouseCollision;
        }

        public static void EndCollisionDraw() {
            currentMouseCollision = null;
        }
    }
}