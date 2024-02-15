using System;

namespace GameEngine
{
    public class Board
    {

        struct Square
        {
            public float[] vertices;
            public uint[] indices;

            public float x;
            public float y;
        }

        public VertexData GetVertexData()
        {
            int width = 1;
            int height = 1;

            Square[] squares = new Square[width * height]; 
            /*
            //Multiply by 3 to make room for x, y, z coordinates
            float[] vertices = new float[(width + 1) * (height + 1) * 3];
            float[] indices = new float[width * height * 6];
            */
            int triIndex = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = y * width + x;

                    squares[0].vertices = new float[]{
                        -0.5f, -0.5f, 0,
                        0.5f, -0.5f, 0,
                        0.5f, 0.5f, 0,
                        -0.5f, 0.5f, 0
                    };

                    squares[0].indices = new uint[]{
                        0, 1, 3,
                        1, 2, 3
                    };
                    /*

                    vertices[i * 3 + 0] = x;
                    vertices[i * 3 + 1] = y;
                    vertices[i * 3 + 2] = 0;

                    if (x < width && y < height)
                    {
                        indices[triIndex + 0] = i;
                        indices[triIndex + 1] = i + 1;
                        indices[triIndex + 2] = i + width;
                        indices[triIndex + 3] = i + 1;
                        indices[triIndex + 4] = i + width + 1;
                        indices[triIndex + 5] = i + width;
                        triIndex += 6;
                    }
                    */
                }
            }

            return new VertexData(squares[0].vertices, squares[0].indices);

        }

        public Board()
        {
        }
    }
}