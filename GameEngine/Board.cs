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
        public Square[] squares;

        public Piece[] pieces;

        static readonly Vector4 blackColour = new Vector4(0.44f, 0.5f, 0.56f, 1.0f);
        static readonly Vector4 whiteColour = new Vector4(0.78f, 0.78f, 0.78f, 1.0f);

        public Board(int width, int height)
        {
            squares = new Square[width * height];

            pieces = LoadFEN("r3k2r/p1pp1pb1/bn2Qnp1/2qPN3/1p2P3/2N5/PPPBBPPP/R3K2R b KQkq - 3 2");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = x + y * width;

                    Shader shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

                    Vector4 colour = (x + y) % 2 == 0 ? blackColour : whiteColour;
                    shader.SetVector4("colour", colour);

                    Square square = new Square(shader, new Vector3(x, y, 0));

                    squares[i] = square;
                }
            }
        }

        public Piece[] LoadFEN(string fen)
        {
            Dictionary<Char, int> charToPiece = new Dictionary<char, int>()
            {
                { 'p', Piece.Pawn   | Piece.Black },
                { 'k', Piece.King   | Piece.Black },
                { 'q', Piece.Queen  | Piece.Black },
                { 'r', Piece.Rook   | Piece.Black },
                { 'b', Piece.Bishop | Piece.Black },
                { 'n', Piece.Knight | Piece.Black },

                { 'P', Piece.Pawn   | Piece.White },
                { 'K', Piece.King   | Piece.White },
                { 'Q', Piece.Queen  | Piece.White },
                { 'R', Piece.Rook   | Piece.White },
                { 'B', Piece.Bishop | Piece.White },
                { 'N', Piece.Knight | Piece.White },
            };

            List<Piece> pieceList = new List<Piece>();

            string[] rankPieces = fen.Split(" ")[0].Split('/');

            for (int rank = 0; rank < rankPieces.Length; rank++)
            {
                rankPieces[rank].Replace("/", "");

                char[] pieceKeys = rankPieces[rank].ToCharArray();

                int x = 0;
                for (int file = 0; file < pieceKeys.Length; file++)
                {
                    if (char.IsNumber(pieceKeys[file]))
                    {
                        x += (pieceKeys[file] - '0') - 1;
                    }
                    else
                    {
                        pieceList.Add(new Piece(charToPiece[pieceKeys[file]], new Vector3(x, 7 - rank, 0)));
                    }
                    x++;
                }
            }
            return pieceList.ToArray();
        }
    }
}