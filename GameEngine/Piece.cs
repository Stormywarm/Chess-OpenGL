using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GameEngine
{
    public class Piece : RenderObject
    {
        public const int None = 0;
        public const int King = 1;
        public const int Queen = 2;
        public const int Bishop = 3;
        public const int Knight = 4;
        public const int Rook = 5;
        public const int Pawn = 6;

        public const int White = 8;
        public const int Black = 16;

        public int PieceType { get; }
        public int PieceTypeNoColour { get; }

        public bool IsWhite { get; }

        public bool madeDoublePawnMove = false;

        public readonly static Dictionary<int, char> pieceToChar = new Dictionary<int, char>()
        {
            { Piece.None,   '_' },
            { Piece.Pawn,   'P' },
            { Piece.King,   'K' },
            { Piece.Queen,  'Q' },
            { Piece.Rook,   'R' },
            { Piece.Bishop, 'B' },
            { Piece.Knight, 'N' }
        };

        const string atlasPath = "Resources/Textures/PieceAtlas.png";

        protected Texture texture;

        public Piece(int pieceType)
        {
            vertices = new float[shape.Length];
            Array.Copy(shape, vertices, vertices.Length);

            PieceType = pieceType;
            PieceTypeNoColour = pieceType & ~(Piece.White | Piece.Black);

            IsWhite = (PieceTypeNoColour | Piece.White) == pieceType;

            shader = new Shader("Resources/Shaders/Piece/shader.vert", "Resources/Shaders/Piece/shader.frag");
        }

        public Piece(Piece piece)
        {
            vertices = piece.vertices;

            PieceType = piece.PieceType;
            PieceTypeNoColour = piece.PieceTypeNoColour;
            IsWhite = piece.IsWhite;

            texture = piece.texture;

            shader = piece.shader;

            vbo = piece.vbo;
            vao = piece.vao;
            ebo = piece.vbo;
        }

        public override void Setup()
        {
            UpdatePosition();

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.DynamicDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int vertexLocation = shader.GetAttribLocation("vPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, 0);
            GL.EnableVertexAttribArray(0);

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, sizeof(float) * 5, sizeof(float) * 3);
            GL.EnableVertexAttribArray(1);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.DynamicDraw);

            shader.Use();

            texture = GetTextureFromPiece(PieceType);
            texture.Use();

            texture.Unbind();
        }

        public override void Render()
        {
            shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.DynamicDraw);

            shader.Use();
            texture.Use();

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            texture.Unbind();
        }

        public override void UpdatePosition()
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

        Texture GetTextureFromPiece(int piece)
        {
            int x;
            int y;

            y = (piece | Piece.White) == piece ? 0 : 1;

            x = PieceTypeNoColour - 1;

            return new Texture(atlasPath, x, y, 6, 2);
        }
    }
}
