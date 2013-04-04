using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Robob
{
    public class Camera
    {
        public Vector3 Translation;
        public float Yaw;
        public Matrix YawRotation;
        public float Pitch;
        public Matrix PitchRotation;
        public float Distance;
        public Matrix View;
        public Matrix World;
        public GameObject Target;
        public float MoveSpeed = 5.0f;
        public float MoveDelay = 0.05f;
        public float MoveTime = 0.0f;
        public float MoveOffset;

        public Camera()
        {
            Yaw = 0.0f;
            Pitch = 45.0f;
            YawRotation = Matrix.Identity;
            PitchRotation = Matrix.Identity;
            Distance = 30.0f;
        }

        public void Update(GameTime gameTime)
        {
            MoveTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            MoveOffset = (float)gameTime.ElapsedGameTime.TotalSeconds / MoveDelay;
            if (MoveTime > MoveDelay)
            {
                if (Engine.NewKeyState.IsKeyDown (Keys.Up))
                    Pitch += MoveSpeed * MoveOffset;
                if (Engine.NewKeyState.IsKeyDown (Keys.Down))
                    Pitch -= MoveSpeed * MoveOffset;
                /*if (Engine.NewKeyState.IsKeyDown(Keys.Left))
                    Yaw += MoveSpeed * MoveOffset;
                if (Engine.NewKeyState.IsKeyDown(Keys.Right))
                    Yaw -= MoveSpeed * MoveOffset;*/
                if (Engine.NewKeyState.IsKeyDown (Keys.O))
                    Distance -= MoveSpeed * MoveOffset;
                if (Engine.NewKeyState.IsKeyDown (Keys.L))
                    Distance += MoveSpeed * MoveOffset;
            }

            if (Pitch > 50.0f)
                Pitch = 50.0f;
            else if (Pitch < 15.0f)
                Pitch = 15.0f;
            if (Yaw > 360.0f)
                Yaw = 0.0f;
            else if (Distance < 20.0f)
                Distance = 20.0f;
            else if (Distance > 50.0f)
                Distance = 50.0f;

            PitchRotation = Matrix.CreateRotationX(MathHelper.ToRadians(Pitch));
            YawRotation = Matrix.CreateRotationY(MathHelper.ToRadians(Yaw));
            Vector3 v = Vector3.Transform(Vector3.Backward * -Distance, PitchRotation);
            v = Vector3.Transform(v, YawRotation);
            
            if (Target != null)
                View = Matrix.CreateLookAt(Target.Translation + v, Target.Translation, Vector3.Up);

            World = Matrix.Invert(View);
        }
    }
}
