using System.Numerics;
using System.Reflection.PortableExecutable;
using Raylib_cs;

namespace LibCast {
    public class Pixel {
        public Color fillColor;
        public Color strokeColor = Screen.emptyColor;
        public char character = ' ';
        public double roomX = -1;
        public double roomY = -1;

        public Pixel() { }

        public Pixel(Color fillColor) {
            this.fillColor = fillColor;
        }

        public Pixel(Color fillColor, Color strokeColor, char character = ' ') : this(fillColor) {
            this.strokeColor = strokeColor;
            this.character = character;
        }


        public Pixel(Color color, double roomX = -1, double roomY = -1) : this(color) {
            this.roomX = roomX;
            this.roomY = roomY;
        }

        public Pixel(Color fillColor, Color strokeColor, char character = ' ', double roomX = -1, double roomY = -1) : this(fillColor, strokeColor, character) {
            this.roomX = roomX;
            this.roomY = roomY;
        }

        public void SetRoomCoords(int x, int y) {
            roomX = x;
            roomY = y;
        }
    }
    public class Screen {

        //Game scaling 
        public static int windowWidth = 1920;
        public static int windowHeight = 1080;
        public static int bufferWidth = 1920;
        public static int bufferHeight = 1080;
        public static int gameWidth = 192;
        public static int gameHeight = 108;
        public static int pixelScale = 10;
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
        public static double timeCounter = 0;
        public static int targetFPS = 30;
        public static bool pause = false;

        public static Font textFont;

        public static List<List<Pixel>> screen = new List<List<Pixel>>();

        public static readonly Color emptyColor = new Color(255, 255, 255, 0);
        public static Color fillColor = Color.Black;
        public static Color strokeColor = Color.White;

        public static bool showFPS = true;


        public static void Init() {
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);
            Raylib.InitWindow(windowWidth, windowHeight, "Libcast");
            Raylib.SetWindowMinSize(640, 360);
            target = Raylib.LoadRenderTexture(bufferWidth, bufferHeight);
            textFont = Raylib.LoadFontEx("assets/font/consolas.ttf", 16, null, 0);
            Raylib.SetTextureFilter(target.Texture, TextureFilter.Point);

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
                Raylib.ToggleBorderlessWindowed();
            }

            currentTime = Raylib.GetTime();
            updateDrawTime = currentTime - previousTime;

            if (targetFPS > 0) {
                waitTime = (1 / (double)targetFPS) - updateDrawTime;
                if (waitTime > 0.0) {
                    Raylib.WaitTime((double)waitTime);
                    currentTime = Raylib.GetTime();
                    deltaTime = (double)(currentTime - previousTime);
                }
            }
            else {
                deltaTime = (double)updateDrawTime;
            }
            previousTime = currentTime;
        }

        public static void Close() {
            Raylib.UnloadRenderTexture(target);
            Raylib.CloseWindow();
        }


        public static void Draw() {
            for (int y = screen.Count - 1; y >= 0; y--) {
                for (int x = screen[y].Count - 1; x >= 0; x--) {
                    Raylib.DrawRectangle(x * pixelScale, y * pixelScale, pixelScale, pixelScale, screen[y][x].fillColor);
                    if (screen[y][x].character != ' ') {
                        for (int i = 0; i < 5; i++) {
                            Raylib.DrawTextEx(textFont, screen[y][x].character.ToString(), new Vector2(x * pixelScale, y * pixelScale + 5), Screen.pixelScale * 2, 0, screen[y][x].strokeColor);
                        }
                    }
                }
            }
            Terminal.DrawCursor();
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

        public static void DrawBackground(int r, int g, int b) {
            DrawBackground(new Color(r, g, b));
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


        public static void DrawPixel(int x, int y, double roomX = -1, double roomY = -1) {
            try {
                screen[y][x] = new Pixel(fillColor, roomX, roomY);
            }
            catch (Exception e) {
                Console.WriteLine("Big issue stahp: " + e.Message);
            }
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

        public static void DrawText(string input, int x, int y) {
            int xOffset = 0;
            foreach (char letter in input) {
                screen[y][x + xOffset].character = letter;
                screen[y][x + xOffset].strokeColor = strokeColor;
                xOffset++;
            }
        }

        public static void DrawFPS() {
            Color oldStroke = strokeColor;
            Stroke(Color.Green);
            DrawText(Math.Round((double)1 / Screen.deltaTime, 2).ToString(), 0, 0);
            Stroke(oldStroke);
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