using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
namespace GameEngine
{
    public class Board
    {
        public void Setup(ref float[] vertices, ref uint[] indices, ref int vbo, ref int vao, ref int ebo, ref Shader shader)
        {
            vertices = new float[]
            {
                0, 0, 0,
                1, 0, 0,
                0, 1, 0,
                1, 1, 0
            };

            indices = new uint[]
            {
                0, 1, 2,
                1, 2, 3
            };

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.EnableVertexAttribArray(0);

            ebo = GL.GenBuffer();
            GL.BindVertexArray(ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();
        }
        public Board()
        {
        }
    }
}