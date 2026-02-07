
using System.Numerics;
using System.Runtime.CompilerServices;

namespace LibCast {
    public class Player : Raycastable {
        public override bool running { get; set; } = true;
        public override double direction { get; set; } = 0;
        public override double radius { get; set; } = 0.10;
        public override double pitch {get; set;} = 0;
        public override int pitchRange {get; set;} = 144;
        public double mouseSensitivity {get; set;} = 1;
        public double gravityAcceleration {get; set;} = -.2;
        public double gravityVelocity {get; set;} = 0;
        public override EntityTexture? texture {get; set;} = new EntityTexture("src/assets/textures/shai_front.png",
        "src/assets/textures/shai_left.png",
        "src/assets/textures/shai_right.png",
        "src/assets/textures/shai_back.png");


        public Player(int x, int y) {
            this.x = x;
            this.y = y;
        }


        public override bool Loop() {

            

            MouseControl();
            
            if (KeyDown(KeyboardKey.J) || KeyDown(KeyboardKey.Left)) {
                direction += (KeyDown(KeyboardKey.LeftShift) ? 6 : 3)*Screen.deltaTime;
            }
            if (KeyDown(KeyboardKey.L) || KeyDown(KeyboardKey.Right)) {
                direction -= (KeyDown(KeyboardKey.LeftShift) ? 6 : 3)*Screen.deltaTime;
            }
            if (KeyDown(KeyboardKey.I) || KeyDown(KeyboardKey.Up)) {
                pitch += (KeyDown(KeyboardKey.LeftShift) ? 6 : 3)*Screen.deltaTime;
                if(pitch > pitchRange) pitch = pitchRange;
            }
            if (KeyDown(KeyboardKey.K) || KeyDown(KeyboardKey.Down)) {
                pitch -= (KeyDown(KeyboardKey.LeftShift) ? 6 : 3)*Screen.deltaTime;
                if(pitch < -pitchRange) pitch = -pitchRange;
            }


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

            if (KeyPressed(KeyboardKey.B)) {
                HouseStructure house = new HouseStructure(x+2, y+2);
                Console.WriteLine(x + " " + y);
                house.Build();
            }

            if(KeyPressed(KeyboardKey.Space)) {
                gravityVelocity = 5;
            }

            
            z += gravityVelocity * Screen.deltaTime;

            gravityVelocity += gravityAcceleration;

            if(z <= 0) {
                z = 0;
                gravityVelocity = 0;
            }



            Raycaster.Go(this, Game.room);
            Screen.BakeScreenDepth();
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
            if(pitch > pitchRange) pitch = pitchRange;
            if(pitch < -pitchRange) pitch = -pitchRange;
        }
        
        
        public new void DrawPitch(int drawX, int drawY) {
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
            
            double angle = -60.0*(pitch/pitchRange) * (Math.PI / 180.0);
            
            int endX = drawX - (int)((radius-1) * Math.Cos(angle));
            int endY = drawY + (int)((radius-1) * Math.Sin(angle));
            
            Screen.DrawLine(drawX, drawY, endX, endY);

            Screen.Fill(Color.Green);
            Screen.DrawCircle(drawX, drawY, radius);

        }


    }
}