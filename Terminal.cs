
using System.Reflection.PortableExecutable;
using Raylib_cs;
using System.Numerics;

namespace LibCast {
    public class Char {
        public int x;
        public int y;
        public char character;
        public Color stroke;
        public Color fill;

        public Char(char character, int x, int y, Color stroke, Color fill) {
            this.character = character;
            this.x = x;
            this.y = y;
            this.stroke = stroke;
            this.fill = fill;
        }

        public void Draw() {
            Raylib.DrawRectangle(x*Screen.terminalScale, y*Screen.terminalScale*2, Screen.terminalScale, Screen.terminalScale*2, fill);
            Raylib.DrawTextEx(Screen.terminalFont, character.ToString(), new Vector2(x*Screen.terminalScale, y*Screen.terminalScale*2+((int)Screen.terminalScale/5)), Screen.terminalScale*2, 0, stroke);
        }
    }

    public class Terminal {
        public static List<List<Char>> screen = new List<List<Char>>();
        public static int cursorX = 0;
        public static int cursorY = 0;

        public static Color fill = Color.Black;
        public static Color stroke = Color.White;

        public static Timer flashTimer = new Timer(0.3f);
        public static int toggle = 0;

        public static void Print(String text) {
            foreach (char character in text) {
                Console.WriteLine("CursorY: " + cursorY);
                Console.WriteLine("ScreenY: " + screen.Count());

                Console.WriteLine("CursorX: " + cursorX);
                Console.WriteLine("ScreenX: " + screen[cursorY].Count());


                screen[cursorY][cursorX] = new Char(character, cursorX, cursorY, Color.White, Color.Black);
                cursorX++;
                if (cursorX > screen[cursorY].Count()-1) {
                    PrintLine();
                }
            }
        }

        public static void PrintLine(String text = "") {
            Print(text);
            cursorX = 0;
            if (cursorY > screen.Count() - 2) {
                screen.RemoveAt(0);
                screen.Add(new List<Char>());
                for (int x = 0; x < Screen.terminalWidth; x++) {
                    screen[cursorY].Add(new Char(' ', 0, 0, Color.White, Color.Black));
                }
            } else {
                cursorY++;
            }
        }

        public static void Draw() {


            for (int y = 0; y < screen.Count(); y++) {
                for (int x = 0; x < screen[y].Count(); x++) {
                    screen[y][x].y = y;
                    screen[y][x].x = x;
                    screen[y][x].Draw();
                }
            }

            if (flashTimer.Tick(Screen.deltaTime)) {
                PrintLine("This is a test + " + Screen.deltaTime);
            }


        }
        
        public static void Resize() {
            screen = new List<List<Char>>();
            for (int y = 0; y < Screen.terminalHeight/2; y++) {
                screen.Add(new List<Char>());
                for (int x = 0; x < Screen.terminalWidth; x++) {
                    screen[y].Add(new Char(' ', 0, 0, Color.White, Color.Black));
                }
            }
        }
    }
}