using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Robob.GameObjects;

namespace Robob
{
    public partial class GameState : State
    {
        Camera camera = new Camera();
        bool renderMousePosition = false;
        MouseState mouseState;
        Vector3 mousePosition;

        Texture2D robotSelection;
        Texture2D robotSelector;
        public bool selectingRobot = true;
        int selectedRobot = 0;
        Viewport leftRobotView;
        Viewport middleRobotView;
        Viewport rightRobotView;
        Matrix robotProjection;
        Matrix robotView;
        PlayerObject[] robots = new PlayerObject[3];

        bool renderFPS = false;
        float elapsedTime = 0.0f;
        int frameCounter = 0;
        int FPS = 0;
        public int generationCount = 0;
        private bool isloaded = false;

        float aspectRatio = 0.0f;
        Matrix projection;
        SpriteFont utilityFont;

        public void Reset()
        {
            selectingRobot = true;
            selectedRobot = 0;
            generationCount = 0;
        }

        public Level CurrentLevel
        {
            get { return Engine.Levels[Engine.LevelIndex]; }
        }

        public GameState()
        {
            utilityFont = Engine.ContentLoader.Load<SpriteFont>("UtilityFont");
            aspectRatio = Engine.Graphics.GraphicsDevice.Viewport.AspectRatio;

            robotSelection = Engine.ContentLoader.Load<Texture2D>("RobotSelection");
            robotSelector = Engine.ContentLoader.Load<Texture2D>("RobotSelector");
            robots[(int)PlayerObject.PlayerType.Normal] = new PlayerObject();
            robots[(int)PlayerObject.PlayerType.Normal].Mesh = Engine.ContentLoader.Load<Model>("FastRob");
            robots[(int)PlayerObject.PlayerType.Normal].Sphere = Engine.ContentLoader.Load<Model>("Orb");
            robots[(int)PlayerObject.PlayerType.Normal].Translation.Y -= 10.0f;
            robots[(int)PlayerObject.PlayerType.Normal].Translation.Z += 12.0f;
            robots[(int)PlayerObject.PlayerType.Normal].Type = PlayerObject.PlayerType.Normal;
            robots[(int)PlayerObject.PlayerType.Jump] = new PlayerObject();
            robots[(int)PlayerObject.PlayerType.Jump].Mesh = Engine.ContentLoader.Load<Model>("JumpRob");
            robots[(int)PlayerObject.PlayerType.Jump].Sphere = Engine.ContentLoader.Load<Model>("Orb");
            robots[(int)PlayerObject.PlayerType.Jump].Translation.Y -= 10.0f;
            robots[(int)PlayerObject.PlayerType.Jump].Translation.Z += 12.0f;
            robots[(int)PlayerObject.PlayerType.Jump].Type = PlayerObject.PlayerType.Jump;
            robots[(int)PlayerObject.PlayerType.Strong] = new PlayerObject();
            robots[(int)PlayerObject.PlayerType.Strong].Mesh = Engine.ContentLoader.Load<Model>("StrongRob");
            robots[(int)PlayerObject.PlayerType.Strong].Sphere = Engine.ContentLoader.Load<Model>("Orb");
            robots[(int)PlayerObject.PlayerType.Strong].Translation.Y -= 10.0f;
            robots[(int)PlayerObject.PlayerType.Strong].Translation.Z += 12.0f;
            robots[(int)PlayerObject.PlayerType.Strong].Type = PlayerObject.PlayerType.Strong;

            float viewPortAspectRatio = (334.0f - 31.0f) / (231.0f - 57.0f);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
            robotProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), viewPortAspectRatio, 1.0f, 10000.0f);
            robotView = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, -5.0f), Vector3.Zero, Vector3.Up);

            leftRobotView = new Viewport(31, 512 + 57, 303, 174);
            middleRobotView = new Viewport(354, 512 + 57, 303, 174);
            rightRobotView = new Viewport(681, 512 + 57, 303, 174);
        }

        public override void Update(GameTime gameTime)
        {
            if (!selectingRobot)
            {
                HandleGameInput(gameTime);
                CurrentLevel.Update(gameTime);
                CheckObjectInteractions();
                CreateCollision();
            }
            else
            {
                HandleUIInput(gameTime);

                robots[0].Rotation.Y += 0.1f * camera.MoveOffset;
                robots[1].Rotation.Y += 0.1f * camera.MoveOffset;
                robots[2].Rotation.Y += 0.1f * camera.MoveOffset;
            }

            CalculateFPS (gameTime);
        }

        public override void Draw()
        {
            DrawLevelObjects(CurrentLevel);

            Engine.SpriteBatch.Begin();
            {
                if (selectingRobot)
                {
                    Engine.SpriteBatch.Draw(robotSelection, new Rectangle(0, 512, 1024, 256), Color.White);

                    if (selectedRobot == 0)
                        Engine.SpriteBatch.Draw(robotSelector, new Vector2(leftRobotView.X - 5, leftRobotView.Y - 4), Color.White);
                    else if (selectedRobot == 1)
                        Engine.SpriteBatch.Draw(robotSelector, new Vector2(middleRobotView.X - 5, middleRobotView.Y - 4), Color.White);
                    else if (selectedRobot == 2)
                        Engine.SpriteBatch.Draw(robotSelector, new Vector2(rightRobotView.X - 5, rightRobotView.Y - 4), Color.White);
                }

                if (renderFPS)
                    Engine.SpriteBatch.DrawString(utilityFont, FPS.ToString(), Vector2.Zero, Color.White);
            } Engine.SpriteBatch.End();
            Reset3DRendering();

            if (selectingRobot)
            {
                Engine.Graphics.GraphicsDevice.Viewport = leftRobotView;
                DrawGameObject(robots[0], robotProjection, robotView);
                Engine.Graphics.GraphicsDevice.Viewport = middleRobotView;
                DrawGameObject(robots[1], robotProjection, robotView);
                Engine.Graphics.GraphicsDevice.Viewport = rightRobotView;
                DrawGameObject(robots[2], robotProjection, robotView);
                Engine.Graphics.GraphicsDevice.Viewport = new Viewport(0, 0, 1024, 768);
            }
        }

        public override void Activate()
        {
            if (!isloaded)
                Load();

            generationCount = 0;

            camera.Target = CurrentLevel.levelObject;
            GameObject start = new GameObject();
            start.Translation = CurrentLevel.StartPoint;
            camera.Target = start;

            if (CurrentLevel != null)
                CurrentLevel.Activate();
        }

        private void Load()
        {
            Engine.MusicRef.QueueMusic ("Song1");
            Engine.MusicRef.QueueMusic ("Song2");
            Engine.MusicRef.QueueMusic ("Song3");
            Engine.MusicRef.QueueMusic ("Song4");
            Engine.MusicRef.QueueMusic ("Song5");

            Engine.Graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        }

        public void DrawLevelObjects(Level level)
        {
            foreach (GameObject obj in level.GameObjects)
                DrawGameObject(obj, null);
        }

        public void DrawGameObject(GameObject obj, Matrix? proj = null, Matrix? view = null)
        {
            if (obj.Mesh == null)
                return;

            if (obj is PlayerObject)
            {
                PlayerObject po = obj as PlayerObject;
                Matrix[] transforms = new Matrix[obj.Mesh.Bones.Count];
                obj.Mesh.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh m in obj.Mesh.Meshes)
                {
                    foreach (BasicEffect effect in m.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[m.ParentBone.Index] *
                            Matrix.CreateScale(obj.Scale) *
                            Matrix.CreateRotationX(obj.Rotation.X) * Matrix.CreateRotationY(obj.Rotation.Y) * Matrix.CreateRotationZ(obj.Rotation.Z) *
                            Matrix.CreateTranslation(obj.Translation + new Vector3(0.0f, -0.7f, 0.0f));
                        effect.View = view ?? camera.View;
                        effect.Projection = proj ?? projection;
                    }
                    m.Draw();
                }

                transforms = new Matrix[po.Sphere.Bones.Count];
                po.Sphere.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh m in po.Sphere.Meshes)
                {
                    foreach (BasicEffect effect in m.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[m.ParentBone.Index] *
                            Matrix.CreateScale(obj.Scale) *
                            Matrix.CreateRotationZ(po.SphereRotation.Z) * Matrix.CreateRotationX(po.SphereRotation.X) * Matrix.CreateRotationY(po.SphereRotation.Y) *
                            Matrix.CreateTranslation(obj.Translation);
                        effect.View = view ?? camera.View;
                        effect.Projection = proj ?? projection;
                    }
                    m.Draw();
                }
            }
            else
            {
                Matrix[] transforms = new Matrix[obj.Mesh.Bones.Count];
                obj.Mesh.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh m in obj.Mesh.Meshes)
                {
                    foreach (BasicEffect effect in m.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[m.ParentBone.Index] *
                            Matrix.CreateScale(obj.Scale) *
                            Matrix.CreateRotationX(obj.Rotation.X) * Matrix.CreateRotationY(obj.Rotation.Y) * Matrix.CreateRotationZ(obj.Rotation.Z) *
                            Matrix.CreateTranslation(obj.Translation);
                        effect.View = camera.View;
                        effect.Projection = projection;
                    }
                    m.Draw();
                }
            }
        }

        private void HandleGameInput(GameTime gameTime)
        {
            

            // render utility info
            renderMousePosition = Engine.NewKeyState.IsKeyDown (Keys.M);
            renderFPS = Engine.NewKeyState.IsKeyDown (Keys.F);

            mouseState = Mouse.GetState ();
            mousePosition = new Vector3 (mouseState.X, mouseState.Y, 0.0f);

            // camera movement
            camera.Update (gameTime);

            if (CurrentLevel.CurrentCharacter != null)
            {
                if ((Engine.NewKeyState.IsKeyDown(Keys.Space) && Engine.LastKeyState.IsKeyUp(Keys.Space))  || 
                    (Engine.NewPadState.IsButtonDown(Buttons.A) && Engine.LastPadState.IsButtonUp(Buttons.A)))
                    selectingRobot = true;
                if (Engine.NewKeyState.IsKeyDown(Keys.W)  || (Engine.NewPadState.IsButtonDown(Buttons.DPadUp)))
                    MoveObject(CurrentLevel.CurrentCharacter, GameObject.Direction.North);
                if (Engine.NewKeyState.IsKeyDown(Keys.S) || (Engine.NewPadState.IsButtonDown(Buttons.DPadDown)))
                    MoveObject(CurrentLevel.CurrentCharacter, GameObject.Direction.South);
                if (Engine.NewKeyState.IsKeyDown(Keys.A) || (Engine.NewPadState.IsButtonDown(Buttons.DPadLeft)))
                    MoveObject(CurrentLevel.CurrentCharacter, GameObject.Direction.East);
                if (Engine.NewKeyState.IsKeyDown(Keys.D) || (Engine.NewPadState.IsButtonDown(Buttons.DPadRight)))
                    MoveObject(CurrentLevel.CurrentCharacter, GameObject.Direction.West);

                if (Engine.NewKeyState.IsKeyDown (Keys.Escape) || Engine.NewPadState.IsButtonDown (Buttons.Back))
                {
                    Engine.NextLevelIndex = 0;
                    Engine.LevelIndex = 0;
                    Engine.SetState (StateType.Title);
                }

                if(Engine.NewPadState.ThumbSticks.Left.X > 0.0f)
                    MoveObject(CurrentLevel.CurrentCharacter, GameObject.Direction.West); 
                else if(Engine.NewPadState.ThumbSticks.Left.X < 0.0f)
                    MoveObject(CurrentLevel.CurrentCharacter, GameObject.Direction.East);
                if (Engine.NewPadState.ThumbSticks.Left.Y > 0.0f)
                    MoveObject(CurrentLevel.CurrentCharacter, GameObject.Direction.North);
                else if (Engine.NewPadState.ThumbSticks.Left.Y < 0.0f)
                    MoveObject(CurrentLevel.CurrentCharacter, GameObject.Direction.South); 
            }
        }

        private void HandleUIInput(GameTime gameTime)
        {
            // render utility info
            renderMousePosition = Engine.NewKeyState.IsKeyDown(Keys.M);
            renderFPS = Engine.NewKeyState.IsKeyDown(Keys.F);

            mouseState = Mouse.GetState();
            mousePosition = new Vector3(mouseState.X, mouseState.Y, 0.0f);

            // camera movement
            camera.Update(gameTime);

            if (Engine.NewKeyState.IsKeyDown (Keys.Escape) || Engine.NewPadState.IsButtonDown (Buttons.Back))
            {
                Engine.SetState (StateType.Title);
                Engine.NextLevelIndex = 0;
                Engine.EngineRef.ReloadLevels ();
            }

            if ((Engine.NewKeyState.IsKeyDown(Keys.Space) && !Engine.LastKeyState.IsKeyDown(Keys.Space)) ||
                    (Engine.NewPadState.IsButtonDown(Buttons.Y) && Engine.LastPadState.IsButtonUp(Buttons.Y)))
                ++selectedRobot;
            if ((Engine.NewKeyState.IsKeyDown(Keys.D) && !Engine.LastKeyState.IsKeyDown(Keys.D)) ||
                    (Engine.NewPadState.IsButtonDown(Buttons.DPadRight) && Engine.LastPadState.IsButtonUp(Buttons.DPadRight)))
                ++selectedRobot;
            else if ((Engine.NewKeyState.IsKeyDown(Keys.A) && !Engine.LastKeyState.IsKeyDown(Keys.A)) || 
                    (Engine.NewPadState.IsButtonDown(Buttons.DPadLeft) && Engine.LastPadState.IsButtonUp(Buttons.DPadLeft)))
                --selectedRobot;
            else if ((Engine.NewKeyState.IsKeyDown(Keys.Enter) && Engine.LastKeyState.IsKeyUp (Keys.Enter))  || 
                    (Engine.NewPadState.IsButtonDown(Buttons.A) && Engine.LastPadState.IsButtonUp(Buttons.A)))
            {
                if (CurrentLevel.RobotsAllowed[selectedRobot])
                {
                    // Move the character from input to output
                    if (CurrentLevel.CurrentCharacter != null)
                    {
                        CurrentLevel.stateHistory.Input.Remove (CurrentLevel.CurrentCharacter);
                        CurrentLevel.stateHistory.Output.Add (CurrentLevel.CurrentCharacter);
                    }

                    CurrentLevel.CurrentCharacter = CurrentLevel.CreatePlayerObject(robots[selectedRobot].Type);
                    CurrentLevel.stateHistory.Input.Add (CurrentLevel.CurrentCharacter);
                    CurrentLevel.GameObjects.Add(CurrentLevel.CurrentCharacter);
                    CurrentLevel.SetAtSpawn(CurrentLevel.CurrentCharacter);
                    CurrentLevel.stateHistory.Start(gameTime);

                    camera.Target = CurrentLevel.CurrentCharacter;

                    selectingRobot = false;

                    foreach (var gObject in CurrentLevel.GameObjects)
                        gObject.Reset();

                    generationCount++;
                    if (generationCount == 1)
                        CurrentLevel.Start ();
                }
                else
                    Engine.Professor.ShowTimed ("You have not unlocked \nthat Robob yet!", 2.0f);
            }

            if (selectedRobot < 0)
                selectedRobot = 2;
            else if (selectedRobot > 2)
                selectedRobot = 0;
        }

        public void MoveObject (GameObject obj, GameObject.Direction direction)
        {
            Vector3 lookVector = obj.lastHeading = GetLookVectorFromDirection (direction);
            Vector3 moveAmount = (lookVector * camera.MoveOffset) * 0.9f;
            Vector3 newLocation = CurrentLevel.CurrentCharacter.Translation + moveAmount;

            // Get data needed for the four corners
            BoundingBox bb = CurrentLevel.UpdateBoundingBox (CurrentLevel.CurrentCharacter.Mesh, Matrix.CreateScale(CurrentLevel.CurrentCharacter.Scale));
            float modelWidth = bb.Max.X - bb.Min.X;
            float modelHeight = bb.Max.Z - bb.Min.Z;
            if (obj is PlayerObject)
            {
                PlayerObject po = obj as PlayerObject;
                Vector3 pos, neg;
                switch (direction)
                {
                    case GameObject.Direction.North:
                        po.SphereRotation.X += camera.MoveOffset;

                        pos = new Vector3(0.0f, 0.0f, 0.0f);
                        neg = new Vector3(0.0f, 0.0f, 0.0f);
                        if((pos - po.Rotation).Length() < (neg - po.Rotation).Length())
                            po.Rotation = Vector3.SmoothStep(po.Rotation, pos, 0.05f);
                        else
                            po.Rotation = Vector3.SmoothStep(po.Rotation, neg, 0.05f);
                        break;
                    case GameObject.Direction.South:
                        po.SphereRotation.X -= camera.MoveOffset;

                        pos = new Vector3(0.0f, MathHelper.ToRadians(180.0f), 0.0f);
                        neg = new Vector3(0.0f, -MathHelper.ToRadians(180.0f), 0.0f);
                        if((pos - po.Rotation).Length() < (neg - po.Rotation).Length())
                            po.Rotation = Vector3.SmoothStep(po.Rotation, pos, 0.05f);
                        else
                            po.Rotation = Vector3.SmoothStep(po.Rotation, neg, 0.05f);
                        break;
                    case GameObject.Direction.East:
                        po.SphereRotation.Z -= camera.MoveOffset;

                        po.Rotation = Vector3.SmoothStep(po.Rotation, new Vector3(0.0f, MathHelper.ToRadians(90.0f), 0.0f), 0.05f);
                        break;
                    case GameObject.Direction.West:
                        po.SphereRotation.Z += camera.MoveOffset;

                        po.Rotation = Vector3.SmoothStep(po.Rotation, new Vector3(0.0f, MathHelper.ToRadians(-90.0f), 0.0f), 0.05f);
                        break;
                    default:
                        break;
                }
            }

            // Reenable this!
           // Vector2 collisionLocation = CurrentLevel.WorldToTexel (newLocation);

            Collidable playerCollidable = new Collidable (
                newLocation.X - (modelWidth / 2f),
                newLocation.Z - (modelHeight / 2f),
                newLocation.X + (modelWidth / 2f),
                newLocation.Z + (modelHeight / 2f));

            Collidable sourceCollidable = new Collidable (
                 CurrentLevel.CurrentCharacter.Translation.X - (modelWidth / 2f),
                 CurrentLevel.CurrentCharacter.Translation.Z - (modelHeight / 2f),
                 CurrentLevel.CurrentCharacter.Translation.X + (modelWidth / 2f),
                 CurrentLevel.CurrentCharacter.Translation.Z + (modelHeight / 2f));

            foreach (Collidable collideGeometry in CurrentLevel.CollisionGeometry)
            {
                if (collideGeometry.Bounds.Intersects (playerCollidable.Bounds))
                {
                    if (collideGeometry.Dense)
                        return;
                }
            }

            if (CheckInteractables (CurrentLevel.CurrentCharacter, ref playerCollidable.Bounds, ref sourceCollidable.Bounds, lookVector))
                return;

            if (CurrentLevel.CurrentCharacter != null)
                CurrentLevel.CurrentCharacter.Translation = newLocation;
        }

        public Vector3 GetLookVectorFromDirection(GameObject.Direction direction)
        {
            switch (direction)
            {
                case GameObject.Direction.North:
                    return new Vector3 (camera.World.Forward.X, 0f, camera.World.Forward.Z);
                case GameObject.Direction.South:
                    return -new Vector3 (camera.World.Forward.X, 0f, camera.World.Forward.Z);
                case GameObject.Direction.East:
                    return new Vector3 (camera.World.Left.X, 0f, camera.World.Left.Z);
                case GameObject.Direction.West:
                    return new Vector3 (camera.World.Right.X, 0f, camera.World.Right.Z);
            }

            return Vector3.Zero;
        }

        private void CalculateFPS(GameTime gameTime)
        {
            // calculate FPS
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter++;
            if (elapsedTime > 1)
            {
                FPS = frameCounter;
                frameCounter = 0;
                elapsedTime = 0.0f;
            }
        }

        private void Reset3DRendering()
        {
            Engine.Graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Engine.Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            Engine.Graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        }
    }
}
