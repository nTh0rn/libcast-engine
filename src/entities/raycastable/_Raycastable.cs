
using System.Numerics;
using System.Runtime.CompilerServices;

namespace LibCast {
    public abstract class Raycastable : Entity {
        public abstract double pitch {get; set;}
        public abstract int pitchRange {get; set;}

        public void DrawPitch(int drawX, int drawY) {
            int r = 10;
            Screen.Fill(Color.Green);
            for(int i = 0; i < r*2-2; i++) {
                if(i % 2 == 0) {
                    if(i < r) {
                        Screen.DrawPixel(drawX-i, drawY);
                    }
                    Screen.DrawPixel(drawX, drawY+i-r+1);

                }
            }

            Screen.Fill(Color.Red);
            
            double angle = -pitch * (Math.PI / 180.0);
            
            int endX = drawX - (int)((r-1) * Math.Cos(angle));
            int endY = drawY + (int)((r-1) * Math.Sin(angle));
            
            Screen.DrawLine(drawX, drawY, endX, endY);

            Screen.Fill(Color.Green);
            Screen.DrawCircle(drawX, drawY, r);

        }


    }
}