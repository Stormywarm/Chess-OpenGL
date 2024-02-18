using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GameEngine
{
    class Game : GameWindow
    {
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }){ }

        public Board board = new Board();

        Shader shader;

        int vbo;
        int vao;
        int ebo;

        uint[] indices;
        float[] vertices;

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.03f, 0.0f, 0.09f, 1.0f);

            board.Setup(ref vertices, ref indices, ref vbo, ref vao, ref ebo, ref shader);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            //float time = (float)args.Time;

            Title = $"FPS: {1f / args.Time:0}";

            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();

            var projection = Matrix4.CreateOrthographicOffCenter(0, 8, 0, 8, 0, 100);
            shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(vao);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, indices);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
