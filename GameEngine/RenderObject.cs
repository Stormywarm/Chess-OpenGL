using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameEngine
{
    public class RenderObject
    {
        public Shader shader;

        public Vector3 position;

        protected readonly Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 8, 0, 8, 0, 100);

        protected uint[] indices = new uint[]
        {
            0, 1, 2,
            1, 2, 3
        };
        protected float[] shape = new float[]
        {
          //Vertices        //Texture Coords:
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
            1.0f, 1.0f, 0.0f, 1.0f, 1.0f
        };

        public float[] vertices;


        protected int vao;
        protected int vbo;
        protected int ebo;
        
        public RenderObject()
        {
            vertices = new float[shape.Length];
            Array.Copy(shape, vertices, vertices.Length);
        }

        public virtual void Setup()
        {
            UpdatePosition();

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int vertexLocation = shader.GetAttribLocation("vPos");
            Console.WriteLine($"Square vertexloc: {vertexLocation}");
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, 0);
            GL.EnableVertexAttribArray(0);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            shader.Use();
        }

        public virtual void Render()
        {
            shader.Use();

            shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(vao);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, indices);
        }

        public virtual void UpdatePosition()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                if (i % 5 == 0)
                {
                    vertices[i] = position.X + shape[i];
                    vertices[i + 1] = position.Y + shape[i + 1];
                    vertices[i + 2] = position.Z + shape[i + 2];
                }
            }
        }
    }
}

