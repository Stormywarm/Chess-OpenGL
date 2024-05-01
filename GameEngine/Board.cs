using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using GameEngine.BitboardUtil;
namespace GameEngine
{
    public class Board
    {
        public const int WIDTH = 8;
        public const int HEIGHT = 8;

        public Square[] squares;

        static readonly Vector4 blackColour = new Vector4(0.44f, 0.5f, 0.56f, 1.0f);
        static readonly Vector4 whiteColour = new Vector4(0.78f, 0.78f, 0.78f, 1.0f);

        public readonly static Dictionary<char, int> charToPiece = new Dictionary<char, int>()
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

        public Board()
        {
            squares = new Square[WIDTH * HEIGHT];

            Piece[,] pieces = LoadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");


            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    int i = x + y * WIDTH;

                    Vector4 colour = (x + y) % 2 == 0 ? blackColour : whiteColour;

                    Shader shader = new Shader("Resources/Shaders/shader.vert", "Resources/Shaders/shader.frag");
                    shader.SetVector4("colour", colour);

                    Square square = new Square(shader, new Vector3(x, y, 0));
                    square.piece = pieces[x, y];

                    squares[i] = square;
                }
            }
        }

        public ref Square GetSquare(Vector2 coord)
        {
            int i = (int)coord.X + (int)coord.Y * WIDTH;
            return ref squares[i];
        }

        public ref Square GetSquare(int x, int y)
        {
            int i = x + y * WIDTH;
            return ref squares[i];
        }

        public ref Square GetSquare(Coord coord)
        {
            int i = coord.file + coord.rank * WIDTH;
            return ref squares[i];
        }

        public Piece[,] LoadFEN(string fen)
        {
            Piece[,] pieceArray = new Piece[WIDTH, HEIGHT];

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    pieceArray[x, y] = new Piece(Piece.None);
                }
            }

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
                        pieceArray[x, HEIGHT - 1 - rank] = new Piece(charToPiece[pieceKeys[file]]);
                    }
                    x++;
                }
            }
            return pieceArray;
        }

        public void Setup()
        {
            float zPosition = -1;
            for (int i = 0; i < squares.Length; i++)
            {
                squares[i].Setup();

                Piece piece = squares[i].piece;
                if (piece.PieceType != Piece.None)
                {
                    piece.position.Z = zPosition;
                    piece.Setup();
                }
            }
        }

        public void RenderSquares()
        {
            for (int i = 0; i < squares.Length; i++)
            {
                squares[i].Render();
            }
        }

        public void RenderPieces()
        {
            for (int i = 0; i < squares.Length; i++)
            {
                Piece piece = squares[i].piece;
                if (piece.PieceType != Piece.None && piece.position.Z != -0.5f)
                {
                    piece.Render();
                }
            }
        }

        public bool TryMakeMove(Move move, ref bool isWhiteToMove, ref BitboardManager bitboard)
        {
            ulong validMovesBitboard = bitboard.GetMovesBitboard(squares[move.StartCoord.ToIndex()]);

            Piece movedPiece = squares[move.StartCoord.ToIndex()].piece;

            bool isSideToMove = isWhiteToMove == movedPiece.IsWhite;
            bool isValidMove = BitboardManager.Contains(validMovesBitboard, move.DestCoord.ToBitBoard());

            if (movedPiece.PieceTypeNoColour == Piece.Pawn)
            {
                bool isDoublePawnMove = MathF.Abs(move.DestCoord.rank - move.StartCoord.rank) == 2;
                movedPiece.madeDoublePawnMove = isDoublePawnMove;
            }

            if (!isSideToMove || !isValidMove)
            {
                Console.WriteLine("Invalid move");
                return false;
            }
            else
            {
                Square startSquare = GetSquare(move.StartCoord);
                Square destSquare = GetSquare(move.DestCoord);

                destSquare.piece = new Piece(startSquare.piece);

                destSquare.piece.position = new Vector3(destSquare.position.X, destSquare.position.Y, -1);
                destSquare.piece.UpdatePosition();

                startSquare.piece = new Piece(Piece.None);

                isWhiteToMove = !isWhiteToMove;

                bitboard.UpdateSquares(squares);

                return true;
            }
        }

        void CastleShort()
        {

        }

        void CastleLong()
        {

        }
    }
}