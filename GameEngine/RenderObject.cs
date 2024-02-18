using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameEngine
{/*
    public class RenderObject : IDisposable
    {
        public MeshData Mesh { get; }
        public Vector4 Colour { get; }

        public Shader shader;

        readonly float[] vertices;
        readonly uint[] indices;

        int vao;
        int vbo;
        int ebo;

        public RenderObject(MeshData mesh)
        {
            Mesh = mesh;
            //this.shader = shader;

            vertices = mesh.vertices;
            indices = mesh.indices;
        }

        public void Setup(int id)
        {

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            GL.VertexAttribPointer(id, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.EnableVertexAttribArray(id);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();
        }

        public void Render()
        {

            shader.Use();
            shader.SetVector4("colour", new Vector4(1, 0, 1, 1));
            Matrix4 orthographicPerspective = Matrix4.CreateOrthographicOffCenter(0.0f, 8.0f, 0.0f, 8.0f, 0f, 100);
            shader.SetMatrix4("projection", orthographicPerspective);

            GL.BindVertexArray(vao);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, indices);
            /*
            float[] vertices = mesh.vertices;
            uint[] indices = mesh.indices;

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.EnableVertexAttribArray(0);

            EBO = GL.GenBuffer();
            GL.BindVertexArray(EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, indices);*/
        }
/*
        public void Dispose()
        {
            GL.DeleteVertexArray(vao);
        }
    }
}*/

