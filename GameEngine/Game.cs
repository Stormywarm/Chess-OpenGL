using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GameEngine
{
    class Game : GameWindow
    {
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, APIVersion = new Version(4, 1) }){ }

        private int quadvbo, quadvao;

        Shader screenShader;

        Framebuffer framebuffer;

        public Board board;

        Piece heldPiece;
        Square originSquare;

        readonly float[] quadVertices = new float[]
        {
            -1.0f,  1.0f,  0.0f, 1.0f,
            -1.0f, -1.0f,  0.0f, 0.0f,
             1.0f, -1.0f,  1.0f, 0.0f,

            -1.0f,  1.0f,  0.0f, 1.0f,
             1.0f, -1.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f, 1.0f
        };

        protected override void OnLoad()
        {
            base.OnLoad();

            framebuffer = new Framebuffer(ClientSize.X, ClientSize.Y);
            framebuffer.Bind();

            SetupScreenQuad();

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.ClearColor(0.03f, 0.0f, 0.09f, 1.0f);

            board = new Board();

            board.Setup();
            framebuffer.Unbind();
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
                heldPiece = GetSelectedSquare().piece;
                originSquare = GetSelectedSquare();
            }

            if (MouseState.IsButtonDown(MouseButton.Left))
            {
                heldPiece.position = mousePosCentred;
                heldPiece.UpdatePosition();
            }

            if (MouseState.IsButtonReleased(MouseButton.Left) && heldPiece.PieceType != Piece.None)
            {
                Square destSquare = GetSelectedSquare();
                Move move = new Move(originSquare.coord, destSquare.coord);

                ulong validMoveMask = board.GetMovesBitboard(originSquare);

                ulong destMask = destSquare.coord.ToBitBoard();

                bool canMoveToDest = (validMoveMask | destMask) == validMoveMask;
                bool isValidMove = originSquare != destSquare;

                Console.WriteLine(Convert.ToString((long)validMoveMask, 2) + ", " + Convert.ToString((long)destMask, 2));

                if (isValidMove && canMoveToDest)
                {
                    board.MakeMove(move);
                    Console.WriteLine($"Made move {(string)move}");
                    Console.WriteLine($"Destination square now holds a {Piece.pieceToChar[destSquare.piece.PieceTypeNoColour]}");
                }
                else
                {
                    heldPiece.position = new Vector3(originSquare.position.X, originSquare.position.Y, -1);
                    heldPiece.UpdatePosition();
                    heldPiece = new Piece(Piece.None);
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

            framebuffer.Bind();

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(1, 1, 1, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Squares must be rendered before pieces,
            //to avoid issues with blending.
            board.RenderSquares();
            board.RenderPieces();
            //Ugly sorting fix to ensure the currently held piece
            //is rendered after the other pieces
            if(heldPiece != null && heldPiece.PieceType != Piece.None)
            {
                heldPiece.Render();
            }

            framebuffer.Unbind();

            GL.ClearColor(1, 1, 1, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderScreenQuad();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            framebuffer = new Framebuffer(e.Width, e.Height);
            GL.Viewport(0, 0, e.Width, e.Height);
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

        Vector2 GetCursorSquarePosition()
        {
            return new Vector2((int)GetCursorBoardPosition().X, (int)Math.Floor(GetCursorBoardPosition().Y));
        }

        Square GetSelectedSquare()
        {
            Vector2 pos = GetCursorSquarePosition();

            return board.GetSquare(pos);
        }

        void SetupScreenQuad()
        {
            screenShader = new Shader("Shaders/Screen/shader.vert", "Shaders/Screen/shader.frag");

            quadvbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadvbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * quadVertices.Length, quadVertices, BufferUsageHint.StaticDraw);

            quadvao = GL.GenVertexArray();
            GL.BindVertexArray(quadvao);

            int vertexLocation = screenShader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, sizeof(float) * 4, 0);
            GL.EnableVertexAttribArray(vertexLocation);

            int texCoordLocation = screenShader.GetAttribLocation("aTexCoords");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, sizeof(float) * 4, sizeof(float) * 2);
            GL.EnableVertexAttribArray(texCoordLocation);

            screenShader.Use();
        }

        void RenderScreenQuad()
        {
            screenShader.Use();
            GL.BindVertexArray(quadvao); 

            GL.Disable(EnableCap.DepthTest);

            GL.BindTexture(TextureTarget.Texture2D, framebuffer.texture);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
