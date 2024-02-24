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
        public const int King = 1;
        public const int Queen = 2;
        public const int Bishop = 3;
        public const int Knight = 4;
        public const int Rook = 5;
        public const int Pawn = 6;

        public const int White = 8;
        public const int Black = 16;

        readonly int pieceType;

        const string atlasPath = "Textures/PieceAtlas.png";
        Texture texture;

        public Piece(int pieceType, Vector3 position)
        {
            this.pieceType = pieceType;

            vertices = new float[]
            {
              //Vertices        //Texture Coords:
                0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f, 1.0f
            };
            indices = new uint[]
            {
                0, 1, 2,
                1, 2, 3
            };

            this.position = position;

            shader = new Shader("Shaders/Piece/shader.vert", "Shaders/Piece/shader.frag");

            UpdatePosition();
        }

        public override void Setup()
        {
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int vertexLocation = shader.GetAttribLocation("vPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, 0);
            GL.EnableVertexAttribArray(vertexLocation);

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, sizeof(float) * 5, sizeof(float) * 3);
            GL.EnableVertexAttribArray(texCoordLocation);

            ebo = GL.GenBuffer();
            GL.BindVertexArray(ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

            shader.Use();

            texture = GetTextureFromPiece(pieceType);
            texture.Use();
        }

        public override void Render()
        {
            var projection = Matrix4.CreateOrthographicOffCenter(0, 8, 0, 8, 0, 100);
            shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(vao);

            shader.Use();
            texture.Use();

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, indices);
        }

        public override void UpdatePosition()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                if (i % 5 == 0)
                {
                    vertices[i] += position.X;
                    vertices[i + 1] += position.Y;
                }
            }
        }

        Texture GetTextureFromPiece(int piece)
        {
            int x;
            int y;

            if ((piece | Piece.White) == piece)
            {
                y = 0;
            }
            else
            {
                y = 1;
            }

            // Unbelievably ugly code, will find a better way later
            int pieceNoColour = (piece | Piece.White | Piece.Black) & ~(Piece.White | Piece.Black);
            x = pieceNoColour - 1;

            return new Texture(atlasPath, x, y, 6, 2);
        }
    }
}
