using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameEngine
{
    public class RenderObject
    {
        public Shader shader;

        public Vector3 position;

        public uint[] indices = new uint[]
        {
            0, 1, 2,
            1, 2, 3
        };
        public float[] vertices = new float[]
        {
            0.0f, 0.0f, 0.0f,
            1.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f,
            1.0f, 1.0f, 0.0f
        };

        public int vao;
        public int vbo;
        public int ebo;
        
        public RenderObject()
        {
        }
        public RenderObject(MeshData mesh, Shader shader, Vector3 position)
        {
            this.shader = shader;

            vertices = mesh.Vertices;
            indices = mesh.Indices;

            this.position = position; 
            UpdatePosition();
        }

        public virtual void Setup()
        {
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int vertexLocation = shader.GetAttribLocation("vPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.EnableVertexAttribArray(vertexLocation);

            ebo = GL.GenBuffer();
            GL.BindVertexArray(ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

            shader.Use();
        }

        public virtual void Render()
        {
            shader.Use();

            var projection = Matrix4.CreateOrthographicOffCenter(0, 8, 0, 8, 0, 100);
            shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(vao);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, indices);
        }

        public virtual void UpdatePosition()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                if (i % 3 == 0)
                {
                    vertices[i] += position.X;
                    vertices[i + 1] += position.Y;
                }
            }
        }
    }
}

