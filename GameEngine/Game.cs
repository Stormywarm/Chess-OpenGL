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

        public Board board;

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.ClearColor(0.03f, 0.0f, 0.09f, 1.0f);

            board = new Board(8, 8);

            for (int i = 0; i < board.squares.Length; i++)
            {
                board.squares[i].Setup();
            }
            for (int i = 0; i < board.pieces.Length; i++)
            {
                board.pieces[i].Setup();
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            float time = (float)args.Time;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Title = $"FPS: {1f / args.Time:0}";

            GL.Clear(ClearBufferMask.ColorBufferBit);

            for (int i = 0; i < board.squares.Length; i++)
            {
                board.squares[i].Render();
            }
            for (int i = 0; i < board.pieces.Length; i++)
            {
                board.pieces[i].Render();
            }
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
