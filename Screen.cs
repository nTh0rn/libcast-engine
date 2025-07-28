using System.Numerics;
using Raylib_cs;

namespace LibCast {
    public class Pixel {
        public Color color;
        public double mapX = -1;
        public double mapY = -1;

        public Pixel() { }

        public Pixel(Color color) {
            this.color = color;
        }
        public Pixel(Color color, double mapX = -1, double mapY = -1) : this(color) {
            this.mapX = mapX;
            this.mapY = mapY;
        }
    } 
    public class Screen {

        //Game scaling 
        public static int windowWidth = 1920;
        public static int windowHeight = 1080;
        public static int gameWidth = 1920;
        public static int gameHeight = 1080;
        public static int terminalScale = 15;
        public static int terminalWidth = gameWidth/terminalScale;
        public static int terminalHeight = gameHeight/terminalScale;
        public static RenderTexture2D target;
        public static float scale;
        public static Vector2 mouse;
        public static Vector2 virtualMouse;

        //Game framerate control
        public static double previousTime = Raylib.GetTime();
        public static double currentTime = 0;
        public static double updateDrawTime = 0;
        public static double waitTime = 0;
        public static float deltaTime = 0.0f;
        public static float timeCounter = 0.0f;
        public static int targetFPS = 60;
        public static bool pause = false;

        public static Font terminalFont;

        public static List<List<Pixel>> screen = new List<List<Pixel>>();


        public static void Init() {
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);
            Raylib.InitWindow(windowWidth, windowHeight, "Libcast");
            Raylib.SetWindowMinSize(640, 360);
            target = Raylib.LoadRenderTexture(gameWidth, gameHeight);
            Raylib.SetTextureFilter(target.Texture, TextureFilter.Point);

            terminalFont = Raylib.LoadFontEx("assets/font/consolas.ttf", terminalScale*2, null, 0);
        }

        public static void LoopInit() {
            timeCounter += deltaTime;
            scale = Math.Min((float)Raylib.GetScreenWidth() / gameWidth, (float)Raylib.GetScreenHeight() / gameHeight);
            mouse = Raylib.GetMousePosition();
            virtualMouse = new Vector2(0);

            virtualMouse.X = (mouse.X - (Raylib.GetScreenWidth() - (gameWidth * scale)) * 0.5f) / scale;
            virtualMouse.Y = (mouse.Y - (Raylib.GetScreenHeight() - (gameHeight * scale)) * 0.5f) / scale;
            virtualMouse = Raymath.Vector2Clamp(
                virtualMouse,
                new Vector2(0, 0),
                new Vector2(gameWidth, gameHeight)
            );

            Raylib.BeginTextureMode(target);
        }

        public static void LoopClose() {
            Raylib.EndTextureMode();
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawTexturePro(
                target.Texture,
                new Rectangle(0.0f, 0.0f, target.Texture.Width, -target.Texture.Height),
                new Rectangle(
                    (Raylib.GetScreenWidth() - (gameWidth * scale)) * 0.5f,
                    (Raylib.GetScreenHeight() - (gameHeight * scale)) * 0.5f,
                    gameWidth * scale,
                    gameHeight * scale
                ),
                new Vector2(0, 0),
                0.0f,
                Color.White
            );
            Raylib.EndDrawing();

            if (Raylib.IsKeyPressed(KeyboardKey.F11)) {
                Raylib.ToggleBorderlessWindowed();
            }

            currentTime = Raylib.GetTime();
            updateDrawTime = currentTime - previousTime;

            if (targetFPS > 0) {
                waitTime = (1.0f / (float)targetFPS) - updateDrawTime;
                if (waitTime > 0.0) {
                    Raylib.WaitTime((float)waitTime);
                    currentTime = Raylib.GetTime();
                    deltaTime = (float)(currentTime - previousTime);
                }
            }
            else {
                deltaTime = (float)updateDrawTime;
            }
            previousTime = currentTime;
        }

        public static void Close() {
            Raylib.UnloadRenderTexture(target);
            Raylib.CloseWindow();
        }


        public static void Draw() {
            // Optimize();
            // Console.Write("\u001b[H\u001b[2J");
            // Raylib.ClearBackground(Color.White);
            for (int y = 0; y < screen.Count; y++) {
                for (int x = 0; x < screen[y].Count; x++) {
                    Raylib.DrawRectangle(x*terminalScale, y*terminalScale, terminalScale, terminalScale, screen[y][x].color);
                }
            }
        }

        public static void Resize() {
            screen = new List<List<Pixel>>();
            for (int y = 0; y < terminalHeight; y++) {
                screen.Add(new List<Pixel>());
                for (int x = 0; x < terminalWidth; x++) {
                    screen[y].Add(new Pixel(Color.White));
                }
            }
        }

        public static void DrawBackground(Color color) {
            for (int y = 0; y < terminalHeight; y++) {
                for (int x = 0; x < terminalWidth; x++) {
                    DrawPixel(x, y, color);
                }
            }
        }

        public static void DrawBackground(int r, int g, int b) {
            DrawBackground(new Color(r, g, b));
        }

        public static void DrawRect(int x, int y, int w, int h, Color color) {
            for (int j = 0; j < h; j++) {
                for (int i = 0; i < w; i++) {
                    DrawPixel(x + i, y + j, color);
                }
            }
        }

        public static void DrawPixel(int x, int y, Color color, double mapX = -1, double mapY = -1) {
            try {
                screen[y][x] = new Pixel(color, mapX, mapY);
            }
            catch (Exception e) {
                Console.WriteLine("Big issue stahp: " + e.Message);
            }
        }

        public static void DrawBox(int x, int y, int w, int h, int strokeSize, Color stroke, Color fill) {
            DrawRect(x, y, w, h, stroke);
            DrawRect(x + strokeSize, y + strokeSize, w - strokeSize*2, h - strokeSize*2, fill);
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
    }
}