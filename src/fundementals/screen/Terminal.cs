
using System.Reflection.PortableExecutable;
using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Data;

namespace LibCast {
    public class Char {
        public int x;
        public int y;
        public char character;
        public Color fill;
        public Color stroke;

        public Char(char character, int x, int y, Color fill, Color stroke) {
            this.character = character;
            this.x = x;
            this.y = y;
            this.fill = fill;
            this.stroke = stroke;
        }

        public Char(Char from) {
            x = from.x;
            y = from.y;
            character = from.character;
            fill = from.fill;
            stroke = from.stroke;
        }

        public void InvertColors() {
            stroke = new Color(255 - stroke.R, 255 - stroke.G, 255 - stroke.B);
            fill = new Color(255 - fill.R, 255 - fill.G, 255 - fill.B);
        }
    }

    public class Terminal {
        public static List<List<Char>> screen = new List<List<Char>>();
        public static int terminalWidth = 112;
        public static int terminalHeight = 30;

        public static int cursorX = 0;
        public static int cursorY = 0;

        public static Color fillColor = Color.Black;
        public static Color strokeColor = Color.White;

        public static Timer cursorFlashTimer = new Timer(0.5);
        public static int cursorFlashToggle = 0;

        public static Char cursorChar = new Char(' ', cursorX, cursorY, Color.Black, Color.White);

        public static string userInput = "";

        public static char[] typableSpecialChars = { ' ', '>', '<' , '$', '^', '=', '+', '|', '`', '~'};

        public static bool userIsTyping = false;

        public static void Print(string text) {

            String[] textLines = text.Split(new string[] { "\n" }, StringSplitOptions.None);
            for (int i = 0; i < textLines.Length; i++) {
                foreach (char character in textLines[i]) {
                    screen[cursorY][cursorX] = new Char(character, cursorX, cursorY, fillColor, strokeColor);
                    MoveCursorX();
                }
                if (i > 0 && i < textLines.Length - 1) {
                    Newline();
                }
            }
        }

        public static void Print(string input, int x, int y) {
            int oldCursorX = cursorX;
            int oldCursorY = cursorY;
            cursorX = x;
            cursorY = y;
            Print(input);
            cursorX = oldCursorX;
            cursorY = oldCursorY;
        }

        public static void PrintLine(String text = "") {
            Print(text);
            Newline();
        }

        public static void Newline(int amount = 1) {
            for (int i = 0; i < amount; i++) {
                cursorX = 0;
                if (cursorY > screen.Count() - 2) {
                    screen.RemoveAt(0);
                    screen.Add(new List<Char>());
                    for (int y = 0; y < terminalHeight; y++) {
                        for (int x = 0; x < terminalWidth; x++) {
                            if (y == (terminalHeight) - 1) {
                                screen[y].Add(new Char(' ', x, y, Color.Black, Color.White));
                            }
                            else {
                                screen[y][x].y = y;
                                screen[y][x].x = x;
                            }
                        }
                    }
                }
                else {
                    cursorY++;
                }
            }
        }

        public static void MoveCursorX(int amount = 1) {
            if (amount > 0) {
                for (int i = 0; i < amount; i++) {
                    cursorX++;
                    if (cursorX > screen[cursorY].Count() - 1) {
                        Newline();
                    }
                }
            }
            else {
                cursorX = Math.Max(0, cursorX + amount);
            }
        }

        public static void DrawCursor() {
            if (cursorFlashTimer.Tick(Screen.deltaTime)) {
                cursorChar.InvertColors();
                cursorFlashToggle++;
                Console.WriteLine("Inverted: " + cursorFlashToggle);
            }
            Raylib.DrawRectangle(cursorX * Screen.pixelScale + Screen.pixelScale*8, cursorY * Screen.pixelScale*2 + Screen.pixelScale*6, Screen.pixelScale, Screen.pixelScale*2, cursorChar.fill);
        }

        public static void Draw() {
            Color oldFill = Screen.fillColor;
            Color oldStroke = Screen.strokeColor;
            Screen.Fill(Color.Black);
            Screen.DrawRect(8, 6, terminalWidth, terminalHeight*2);
            for (int y = 0; y < screen.Count(); y++) {
                for (int x = 0; x < screen[y].Count(); x++) {
                    oldFill = Screen.fillColor;
                    oldStroke = Screen.strokeColor;
                    Screen.Fill(screen[y][x].fill);
                    Screen.Stroke(screen[y][x].stroke);
                    Screen.DrawRect(x + 8, y * 2 + 6, 1, 2);
                    Screen.DrawText(screen[y][x].character.ToString(), x + 8, y * 2 + 6);
                }
            }
            Screen.Fill(oldFill);
            Screen.Stroke(oldStroke);
            DrawCursor();
        }

        public static void GetUserCommand() {
            String userCommand = getUserCommand();
            if (userCommand != "") {
                userInput = "";
                PrintLine(userCommand);
            }
        }

        public static void Resize() {
            screen = new List<List<Char>>();
            for (int y = 0; y < terminalHeight; y++) {
                screen.Add(new List<Char>());
                for (int x = 0; x < terminalWidth; x++) {
                    screen[y].Add(new Char(' ', x, y, Color.Black, Color.White));
                }
            }
        }

        public static String getUserCommand() {
            Print("?> ", 0, cursorY);
            cursorX = Math.Max(3, cursorX);

            int key = Raylib.GetKeyPressed();
            if (key == 257) { //enter
                Newline();
                return userInput;
            }
            else if (key == 259) { //backspace
                UntypeChar();
            }

            int pressedChar = Raylib.GetCharPressed();
            if (char.IsLetterOrDigit((char)pressedChar) || char.IsPunctuation((char)pressedChar) || Array.IndexOf(typableSpecialChars, (char)pressedChar) != -1) {
                TypeChar((char)pressedChar);
            }

            return "";
        }

        public static void TypeChar(char c) {
            Print(c.ToString());
            userInput += c;
        }
        public static void UntypeChar() {
            if (cursorX > 3) {
                userInput = userInput.Substring(0, userInput.Length - 1);
                MoveCursorX(-1);
                Color oldFill = fillColor;
                Color oldStroke = strokeColor;
                Fill(Color.Black);
                Stroke(Color.White);
                Print(" ");
                Fill(oldFill);
                Stroke(oldStroke);
                MoveCursorX(-1);
            }
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
    }
}