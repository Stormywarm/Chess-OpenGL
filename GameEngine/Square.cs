using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameEngine
{
    public class Square : RenderObject
    {
        public Piece piece = new Piece(Piece.None);

        public Coord coord { get; set; }

        public Square(Shader shader, Vector3 position)
        {
            vertices = new float[shape.Length];
            Array.Copy(shape, vertices, vertices.Length);

            this.shader = shader;
            this.position = new Vector3(position.X, position.Y, 0);

            coord = new Coord(position.Xy);
            piece = new Piece(Piece.None);
        }

        public override void UpdatePosition()
        {
            
            for (int i = 0; i < vertices.Length; i++)
            {
                if (i % 5 == 0)
                {
                    
                    vertices[i] = position.X + shape[i];
                    vertices[i + 1] = position.Y + shape[i + 1];
                    vertices[i + 2] = -2;
                }
            }

            piece.position = new Vector3(position.X, position.Y, 0);
        }

        public bool IsEmpty()
        {
            return piece.PieceType == Piece.None;
        }
    }
}
