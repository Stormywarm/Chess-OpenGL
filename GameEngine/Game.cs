﻿using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

using GameEngine.BitboardUtil;

namespace GameEngine
{
    class Game : GameWindow
    {
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, APIVersion = new Version(4, 1), Flags = ContextFlags.ForwardCompatible }) { }

        public Board board;

        BitboardManager bitboard;

        Piece heldPiece;
        Square originSquare;

        bool isWhiteToMove = true;

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.ClearColor(0.03f, 0.0f, 0.09f, 1.0f);

            board = new Board();
            board.Setup();

            bitboard = new BitboardManager(board.squares);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            Vector3 mousePos = new Vector3(GetCursorBoardPosition().X, GetCursorBoardPosition().Y, 0);
            //Move the mouse position by half of the pieces width and height,
            //so that the piece is moved to the centre of the cursor
            Vector3 mousePosCentred = new Vector3(mousePos.X - 0.5f, mousePos.Y - 0.5f, -0.5f);

            if (MouseState.IsButtonPressed(MouseButton.Left))
            {
                if (GetCursorCoord().IsValidSquare())
                {
                    originSquare = board.GetSquare(GetCursorCoord());
                    heldPiece = originSquare.piece;
                }
            }

            if (MouseState.IsButtonDown(MouseButton.Left))
            {
                heldPiece.position = mousePosCentred;
                heldPiece.UpdatePosition();
            }

            if (MouseState.IsButtonReleased(MouseButton.Left) && (heldPiece.PieceType != Piece.None))
            {
                Console.WriteLine("Mouse released");
                if (!GetCursorCoord().IsValidSquare())
                {
                    ReleasePiece();
                }
                else
                {
                    Square destSquare = board.GetSquare(GetCursorCoord());
                    Move move = new Move(originSquare.coord, destSquare.coord);

                    bool isValidMove = board.TryMakeMove(move, ref isWhiteToMove, ref bitboard);
                    if (!isValidMove)
                    {
                        ReleasePiece();
                    }
                }
            }

            if (KeyboardState.IsKeyReleased(Keys.P))
            {
                Console.Clear();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            Title = $"FPS: {1f / args.Time:0}";

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(1, 1, 1, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Squares must be rendered before pieces,
            //to avoid issues with blending.
            board.RenderSquares();
            board.RenderPieces();
            //Ugly sorting fix to ensure the currently held piece
            //is rendered after the other pieces
            if (heldPiece != null && heldPiece.PieceType != Piece.None)
            {
                heldPiece.Render();
            }

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            //TODO: find a way to resize on mac retina
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        Vector2 GetCursorBoardPosition()
        {
            //Inverse the y-axis so that the bottom-right corner is (0, 0)
            float inversedY = ClientSize.Y - MouseState.Y;

            Vector2 normalizedCursorPos = new Vector2(MouseState.X / ClientSize.X, inversedY / ClientSize.Y);
            normalizedCursorPos *= 8;

            Vector2 cursorBoardPos = new Vector2(normalizedCursorPos.X, normalizedCursorPos.Y);

            return cursorBoardPos;
        }

        Coord GetCursorCoord()
        {
            Vector2 pos = new Vector2((int)GetCursorBoardPosition().X, (int)Math.Floor(GetCursorBoardPosition().Y));
            return new Coord(pos);
        }

        void ReleasePiece()
        {
            heldPiece.position = new Vector3(originSquare.position.X, originSquare.position.Y, -1);
            heldPiece.UpdatePosition();
        }
    }
}
