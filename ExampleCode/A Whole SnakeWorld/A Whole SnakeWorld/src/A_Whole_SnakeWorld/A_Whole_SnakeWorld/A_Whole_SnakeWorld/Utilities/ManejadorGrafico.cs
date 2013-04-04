using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace A_Whole_SnakeWorld
{
    class ManejadorGrafico
    {
        VertexDeclaration vertex;
        BasicEffect basic;

        // for fps measurement
        private int fpsCounter;
        private double elapsedMilliseconds;
        private double fps;

        public ManejadorGrafico(GraphicsDevice device)
        {
            basic = InitializeEffect(device);
            vertex = InitializeVertexDeclaration();
        }
        

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private BasicEffect InitializeEffect(GraphicsDevice device)
        {

            BasicEffect basicEffect = new BasicEffect(device);
            basicEffect.VertexColorEnabled = true;

            Matrix viewMatrix = Matrix.CreateLookAt(
               new Vector3(0.0f, 0.0f, 1.0f),
               Vector3.Zero,
               Vector3.Up
               );

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(
                0,
                (float)device.Viewport.Width,
                (float)device.Viewport.Height,
                0,
                1.0f, 1000.0f);
            Matrix worldMatrix = Matrix.CreateTranslation(device.Viewport.Width / 2f - 150,
                device.Viewport.Height / 2f - 50, 0);

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

            return basicEffect;

        }

        /// <summary>
        /// Initializes the Vertex Declaration
        /// </summary>
        /// <returns></returns>
        protected VertexDeclaration InitializeVertexDeclaration()
        {
            VertexDeclaration vertexDeclaration = new VertexDeclaration(new VertexElement[]
                {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
                }
              );

            return vertexDeclaration;

        }

        /// <summary>
        /// Initializes the point list.
        /// </summary>
        protected VertexPositionColor[] InitializePoints(Vector3[] puntos,Color color)
        {
            VertexPositionColor[] pointList = new VertexPositionColor[puntos.Length];

            for (int i = 0; i < pointList.Length; i++)
            {
                pointList[i] = new VertexPositionColor(puntos[i], color);
            }


            return pointList;
        }

        /// <summary>
        /// Initialize el Buffer
        /// </summary>
        /// <param name="vertexDeclaration"></param>
        /// <param name="pointList"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        protected VertexBuffer InitializeBuffer(GraphicsDeviceManager graphics, VertexDeclaration vertexDeclaration, VertexPositionColor[] pointList, int points)
        {

            // Initialize the vertex buffer, allocating memory for each vertex.
            VertexBuffer vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, vertexDeclaration,
                points, BufferUsage.None);

            // Set the vertex buffer data to the array of vertices.
            vertexBuffer.SetData<VertexPositionColor>(pointList);

            return vertexBuffer;
        }

        /// <summary>
        /// Initializes the line strip.
        /// </summary>
        protected short[] InitializeLineStrip(int points)
        {
            // Initialize an array of indices of type short.
            short[] lineStripIndices = new short[points];

            // Populate the array with references to indices in the vertex buffer.
            for (int i = 0; i < points; i++)
            {
                lineStripIndices[i] = (short)(i);
            }

            return lineStripIndices;
        }

        /// <summary>
        /// Draws the line strip.
        /// </summary>
        protected void DrawLineStrip(GraphicsDevice device, VertexPositionColor[] pointList, short[] lineStripIndices, BasicEffect basicEffect)
        {
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserIndexedPrimitives<VertexPositionColor>(
                 PrimitiveType.LineStrip,
                 pointList,
                 0,   // vertex buffer offset to add to each element of the index buffer
                 pointList.Length,   // number of vertices to draw
                 lineStripIndices,
                 0,   // first index element to read
                 pointList.Length - 1    // number of primitives to draw
             );

            }
        }

        public void DrawLines(GraphicsDeviceManager device, Vector3[] puntos,Color color)
        {

            VertexPositionColor[] point = InitializePoints(puntos,color);
            VertexBuffer vertexBuffer = InitializeBuffer(device, vertex, point, point.Length);
            short[] lineas = InitializeLineStrip(point.Length);
            DrawLineStrip(device.GraphicsDevice ,point, lineas, basic);
        }

        /// <summary>
        /// Dibuja puntos en la pantalla, el spritebatch ya tiene que haberse puesto en begin
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="punto"></param>
        /// <param name="offset"></param>
        /// <param name="puntos"></param>
        /// <param name="color"></param>
        public void DrawPoints(SpriteBatch spriteBatch ,Texture2D punto,float offset,Vector3[] puntos,Color color) {
            
            
        
            float offx = punto.Width / 2 * offset;
            float offy = punto.Height / 2 * offset;
            
            for (int i = 0; i < puntos.Length; i++)
            {
                spriteBatch.Draw(punto, new Vector2(puntos[i].X - offx, puntos[i].Y - offy), null, color, 0, Vector2.Zero,offset, SpriteEffects.None, 0.9f);
               
            }
           
        }

        /// <summary>
        /// Muestra los FPS en pantalla
        /// </summary>
        /// <param name="tiempo">Tiempo por frame</param>
        /// <param name="spriteBatch">SpriteBatch necesario para dibujar en pantalla</param>
        /// <param name="font">Tipo de letra</param>
        public void CounterFPS(float tiempo, SpriteBatch spriteBatch,SpriteFont font) {
            // calculate the frame rate and draw it on the screen
            fpsCounter++;
            elapsedMilliseconds += tiempo;

            if (elapsedMilliseconds > 1000)
            {
                // update the fps once a second
                fps = fpsCounter / elapsedMilliseconds * 1000.0;
                elapsedMilliseconds = 0;
                fpsCounter = 0;
            }

            spriteBatch.DrawString(font, "FPS: " + fps.ToString("0.00"), new Vector2(0, 20), Color.Black);
        }
    }
}
