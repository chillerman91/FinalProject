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
    public class Level
    {
        public Level(string model)
            : this (model, false)
        {
        }

        public Level (string model, bool flipLevel)
		{
			stateHistory = new StateHistory();
            GameObjects = new List<GameObject>();
            Collidables = new List<GameObject> ();
            CollisionGeometry = new List<Collidable> ();

            if (!string.IsNullOrEmpty (model))
            {
                this.levelModel = model;
                levelObject = CreateLevel(flipLevel);
                GameObjects.Add (levelObject);
            }

            collision = Engine.ContentLoader.Load<Texture2D> ("LevelOneCollision");
            Colors = new Color[collision.Width * collision.Height];
            collision.GetData (Colors);
		}

        public void SetAtSpawn(GameObject gameObject)
        {
            gameObject.Translation = StartPoint;
        }

        public virtual void Activate()
        {
        }

        public virtual void Start()
        {
            start = DateTime.Now;
        }

        public Vector3 StartPoint;
        public Vector3 EndPoint;
    	public Texture2D CollisionMap;
        public Color[] Colors;
        public BoundingBox BB;
		public List<GameObject> GameObjects;
        public List<GameObject> Collidables;
		public StateHistory stateHistory;
        public GameObject levelObject;
        public PlayerObject CurrentCharacter;
        public List<Collidable> CollisionGeometry;
        public int currentID = 0;
        public float yLevel = 1.0f;
        public string levelModel;
        public bool[] RobotsAllowed = new bool[] { true, false, false };
        public DateTime start;

        private Texture2D collision;

		public virtual void Update (GameTime gameTime)
		{
		    stateHistory.Update (gameTime);

		    foreach (var gameObject in GameObjects)
                gameObject.Update (gameTime);
		}

        public void ParseCollisionMap()
        {
            Colors = new Color[CollisionMap.Width * CollisionMap.Height];

            CollisionMap.GetData<Color>(Colors);
            BB = BoundingBox.CreateFromSphere(GameObjects[0].Mesh.Meshes[0].BoundingSphere);

            for (int y = 0; y < CollisionMap.Height; ++y)
            {
                for (int x = 0; x < CollisionMap.Width; ++x)
                {
                    if (Colors[(y * CollisionMap.Width) + x] == new Color(0, 0, 255)) // start point
                        StartPoint = TexelToWorld(new Vector2(x, y));
                    else if (Colors[(y * CollisionMap.Width) + x] == new Color(0, 0, 255)) // end point
                        EndPoint = TexelToWorld(new Vector2(x, y));
                    else if (Colors[(y * CollisionMap.Width) + x] == Color.White)
                        continue;
                    else
                        Colors[(y * CollisionMap.Width) + x] = Color.Black;
                }
            }
        }

        public BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            } 

            // Create and return bounding box
            return new BoundingBox(min, max);
        }

        public Vector3 TexelToWorld(Vector2 tex)
        {
            Vector3 transformVector = levelObject.Translation * new Vector3(-1, 0, 0);
            BoundingBox bb = UpdateBoundingBox(GameObjects[0].Mesh, Matrix.CreateTranslation(transformVector));
            float modelWidth = bb.Max.X - bb.Min.X;
            float modelHeight = bb.Max.Z - bb.Min.Z;
            float texelWidth = modelWidth / CollisionMap.Width;
            float texelHeight = modelHeight / CollisionMap.Height;

            return new Vector3((tex.X - CollisionMap.Width) * -texelWidth, yLevel, (tex.Y - CollisionMap.Height) * -texelHeight);
        }

        public Color lastSampledColor = Color.White;
        public bool ValidPosition (Vector2 tex)
        {
            return true;
            bool isValidPoint = !(tex.Y < 0 || tex.Y > CollisionMap.Height || tex.X < 0 || tex.X > CollisionMap.Width);
            
            if (!isValidPoint)
                return false;

            int index = ((int)tex.Y * (int)CollisionMap.Width) + (int)tex.X;
            lastSampledColor = Colors[index];

            bool isBlack = lastSampledColor != Color.White;
            if (isBlack)
                return false;

            return isValidPoint && !isBlack;
        }
                
        public Vector2 WorldToTexel (Vector3 pos)
        {
            Vector3 transformVector = levelObject.Translation * new Vector3(-1, 0, 0);
            BoundingBox bb = UpdateBoundingBox (GameObjects[0].Mesh, Matrix.CreateTranslation (transformVector));
            float modelWidth = bb.Max.X - bb.Min.X;
            float modelHeight = bb.Max.Z - bb.Min.Z;
            float texelWidth = CollisionMap.Width / modelWidth;
            float texelHeight = CollisionMap.Height / modelHeight;
         
            return new Vector2(CollisionMap.Width - (texelWidth * pos.X), CollisionMap.Height - (texelHeight * pos.Z));
        }

        public GameObject CreateLevel()
        {
            return CreateLevel (false);
        }

        public GameObject CreateLevel(bool flip)
        {
            GameObject level = new GameObject
            {
                ID = ++currentID,
                Mesh = Engine.ContentLoader.Load<Model> (levelModel),
                Translation = new Vector3 (0, 0, 0),
                Rotation = flip ? new Vector3(0, (float)Math.PI, 0) : new Vector3 (0, 0, 0)
            };

            BB = UpdateBoundingBox (level.Mesh, Matrix.Identity);
            level.Translation += new Vector3 ((BB.Max.X - BB.Min.X) / 2, 0, (BB.Max.Z - BB.Min.Z) / 2);

            return level;
        }

        public BoxObject CreateCrate(Vector3 location, Vector3 direction, float maxMovement)
        {
            return new BoxObject
            {
                Mesh = Engine.ContentLoader.Load<Model> ("Crate"),
                Scale = new Vector3 (0.75f, 0.75f, 0.75f),
                Translation = location,
                Original = location,
                Direction = direction,
                MaxMovement = maxMovement,
                Dense = true,
                ID = ++currentID
            };
        }

        public TriggerZone CreateTrigger(Collidable collidable, Action action)
        {
            return new TriggerZone (collidable, action)
            {
                ID = ++currentID
            };
        }

        public ButtonObject CreateButton(Vector3 location, GameObject target)
        {
            return this.CreateButton (location, target, null);
        }

        public ButtonObject CreateButton (Vector3 location, GameObject target, Action action)
		{
			return new ButtonObject
			{
                ID = ++currentID,
                Mesh = Engine.ContentLoader.Load<Model> ("Button"),
				Translation = location,
                Targets = new List<GameObject> (new [] { target }),
                Action = action
			};
		}

        public CrystalObject CreateCrystal (Vector3 location)
        {
            return new CrystalObject
            {
                ID = ++currentID,
                Mesh = Engine.ContentLoader.Load<Model> ("Crystal"),
                Scale = new Vector3 (2f, 2.0f, 2f),
                Translation = location
            };
        }

        public DoorObject CreateDoor (Vector3 location, int buttonCount, float rotation)
		{
			return new DoorObject (buttonCount)
			{
                Mesh = Engine.ContentLoader.Load<Model> ("Door"),
                Scale = new Vector3(0.5f, 0.5f, 0.5f),
                Rotation = new Vector3 (0, MathHelper.ToRadians (rotation), 0),
				Translation = location,
                Original = location,
                Dense = true,
				ID = ++currentID
			};
		}

        public PlayerObject CreatePlayerObject(PlayerObject.PlayerType type)
        {
            return new PlayerObject
            {
                ID = ++currentID,
                Type = type,
                Mesh = Engine.ContentLoader.Load<Model>(type == PlayerObject.PlayerType.Normal ?  "FastRob" : type == PlayerObject.PlayerType.Jump ? "JumpRob" : "StrongRob"),
                Sphere = Engine.ContentLoader.Load<Model>("Orb"),
                Translation = StartPoint,
                //Rotation = type == PlayerObject.PlayerType.Normal ? Vector3.Zero : type == PlayerObject.PlayerType.Jump ? Vector3.Zero : new Vector3(0.0f, MathHelper.ToRadians(90.0f), 0.0f),
                Scale = new Vector3(0.25f, 0.25f, 0.25f)
            };
        }

        private bool keyPressed(KeyboardState oldState, KeyboardState newState, Keys key)
        {
            return oldState.IsKeyUp (key) && newState.IsKeyDown (key);
        }
    }
}
