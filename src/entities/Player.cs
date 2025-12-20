
using System.Numerics;
using System.Runtime.CompilerServices;

namespace LibCast {
    public class Player : Entity {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.10;
        public double pitch {get; set;} = 0;
        public double mouseSensitivity = 1;

        private double traversalScale = 10;

        public Player(int x, int y) {
            this.x = x;
            this.y = y;
        }


        public override bool Loop() {
            if(UI.gameState != GameState.PLAY) {
                return true;
            }
            if (KeyDown(KeyboardKey.J)) {
                direction += (KeyDown(KeyboardKey.LeftShift) ? 6 : 3)*Screen.deltaTime;
            }
            if (KeyDown(KeyboardKey.L)) {
                direction -= (KeyDown(KeyboardKey.LeftShift) ? 6 : 3)*Screen.deltaTime;
            }
            if (KeyDown(KeyboardKey.I)) {
                pitch += (KeyDown(KeyboardKey.LeftShift) ? 6 : 3)*Screen.deltaTime;
                if(pitch > 42) pitch = 42;
            }
            if (KeyDown(KeyboardKey.K)) {
                pitch -= (KeyDown(KeyboardKey.LeftShift) ? 6 : 3)*Screen.deltaTime;
            }

            MouseControl();

            if (KeyDown(KeyboardKey.A)) {
                moveDirection(direction + 90);
            }
            if (KeyDown(KeyboardKey.D)) {
                moveDirection(direction - 90);
            }
            if (KeyDown(KeyboardKey.W)) {
                moveDirection(direction);
            }
            if (KeyDown(KeyboardKey.S)) {
                moveDirection(direction - 180);
            }



            Raycaster.Go(this);
            DrawPitch(Screen.gameWidth-1,50);
            return true;
        }

        private void MouseControl() {
            Vector2 mouseDelta = Raylib.GetMouseDelta();
            direction -= mouseDelta[0]*Screen.deltaTime;
            Raylib.SetMousePosition(Screen.windowWidth/2,Screen.windowHeight/2);
            if(!Raylib.IsCursorHidden()) {
                Raylib.HideCursor();
            }
            pitch -= mouseDelta[1]*Screen.deltaTime;
            if(pitch > 42) pitch = 42;
            if(pitch < -42) pitch = -42;

        }
        
        private void moveDirection(double dir) {
            double dx = Math.Cos((dir) * (Math.PI / 180.0)) / traversalScale * Screen.deltaTime;
            double dy = -Math.Sin((dir) * (Math.PI / 180.0)) / traversalScale * Screen.deltaTime;

            int dirx = dx > 0 ? 1 : -1;
            int diry = dy > 0 ? 1 : -1;
            double oldX = x, oldY = y;

            if(KeyDown(KeyboardKey.LeftShift)) {
                dx *= 1.5;
                dy *= 1.5;
            }
            
            // Try full movement
            x += dx; y += dy;
            if(IsValidMove(dirx, diry)) return;
            
            // Try X-only slide
            x = oldX + dx; y = oldY;
            if(IsValidMove(dirx, diry)) return;
            
            // Try Y-only slide
            x = oldX; y = oldY + dy;
            if(IsValidMove(dirx, diry)) return;
            
            // Revert to original
            x = oldX; y = oldY;
        }
        
        private bool IsValidMove(int dirx, int diry) {
            int ix = (int)(x+radius*dirx), iy = (int)(y+radius*diry);
            if(iy < 0 || iy >= Game.room.room.Count || ix < 0 || ix >= Game.room.room[iy].Count) return false;
            if(Game.room.room[iy][ix] is not EmptyCell) return false;
            
            foreach(Entity entity in Game.room.entities) {
                if(entity != this && entity.CollisionEntity(this)) return false;
            }
            return true;
        }
        
        private double DistanceToPoint(double px, double py) {
            return Math.Sqrt(Math.Pow(x - px, 2) + Math.Pow(y - py, 2));
        }

        public void DrawPitch(int drawX, int drawY) {
            int radius = 10;
            Screen.Fill(Color.Green);
            for(int i = 0; i < radius*2-2; i++) {
                if(i % 2 == 0) {
                    if(i < radius) {
                        Screen.DrawPixel(drawX-i, drawY);
                    }
                    Screen.DrawPixel(drawX, drawY+i-radius+1);

                }
            }
            //Screen.DrawLine(Screen.gameWidth-10, 10, Screen.gameWidth-1, 10);

            Screen.Fill(Color.Red);
            
            double angle = -pitch * (Math.PI / 180.0);
            
            int endX = drawX - (int)((radius-1) * Math.Cos(angle));
            int endY = drawY + (int)((radius-1) * Math.Sin(angle));
            
            Screen.DrawLine(drawX, drawY, endX, endY);

            Screen.Fill(Color.Green);
            Screen.DrawCircle(drawX, drawY, radius);

        }


    }
}