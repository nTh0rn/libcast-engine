using System.Numerics;
using Raylib_cs;

namespace LibCast {
    public class Screen {

        //Game scaling 
        public static int windowWidth = 1920;
        public static int windowHeight = 1080;
        public static int gameWidth = 1920;
        public static int gameHeight = 1080;
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


        public static void Init() {
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);
            Raylib.InitWindow(windowWidth, windowHeight, "Libcast");
            Raylib.SetWindowMinSize(640, 360);
            target = Raylib.LoadRenderTexture(gameWidth, gameHeight);
            Raylib.SetTextureFilter(target.Texture, TextureFilter.Bilinear);

            terminalFont = Raylib.LoadFontEx("assets/font/consolas.ttf", 32, null, 0);
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
                //Raylib.ToggleFullscreen();
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
            } else {
                deltaTime = (float)updateDrawTime;
            }
            previousTime = currentTime;
        }

        public static void Close() {
            Raylib.UnloadRenderTexture(target);
            Raylib.CloseWindow();
        }
    }
}