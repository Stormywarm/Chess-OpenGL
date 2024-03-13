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
        public readonly static int Width = 8;
        public readonly static int Height = 8;

        public Square[] squares;

        static readonly Vector4 blackColour = new Vector4(0.44f, 0.5f, 0.56f, 1.0f);
        static readonly Vector4 whiteColour = new Vector4(0.78f, 0.78f, 0.78f, 1.0f);

        public static readonly Coord[] OrthogonalOffsets = { 
            new Coord(1, 0), new Coord(-1, 0),
            new Coord(0, 1), new Coord(0, -1)
        };
        public static readonly Coord[] DiagonalOffsets = { 
            new Coord(1, 1), new Coord(-1, -1),
            new Coord(1, -1), new Coord(-1, 1)
        };
        public static readonly Coord[] KnightOffsets = {
            new Coord(1, 2), new Coord(-1, 2),
            new Coord(2, 1), new Coord(2, -1),
            new Coord(1, -2), new Coord(-1, -2),
            new Coord(-2, 1), new Coord(-2, -1)
        };

        static readonly Coord singlePawnOffset = new Coord(0, 1);
        static readonly Coord doublePawnOffset = new Coord(0, 2);

        static readonly Coord captureLeftOffset = new Coord(-1, 1);
        static readonly Coord captureRightOffset = new Coord(1, 1);


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
            squares = new Square[Width * Height];

            Piece[,] pieces = LoadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");


            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int i = x + y * Width;

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
            int i = (int)coord.X + (int)coord.Y * Width;
            return ref squares[i];
        }

        public ref Square GetSquare(int x, int y)
        {
            int i = x + y * Width;
            return ref squares[i];
        }

        public ref Square GetSquare(Coord coord)
        {
            int i = coord.file + coord.rank * Width;
            return ref squares[i];
        }

        public Piece[,] LoadFEN(string fen)
        {
            Piece[,] pieceArray = new Piece[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
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
                        pieceArray[x, Height - 1 - rank] = new Piece(charToPiece[pieceKeys[file]]);
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
        
        public void MakeMove(Move move)
        {
            Square startSquare = GetSquare(move.StartCoord);
            Square destSquare = GetSquare(move.DestCoord);

            destSquare.piece = new Piece(startSquare.piece);

            destSquare.piece.position = new Vector3(destSquare.position.X, destSquare.position.Y, -1);
            destSquare.piece.UpdatePosition();

            startSquare.piece = new Piece(Piece.None);
        }
        
        public ulong GetMovesBitboard(Square square)
        {
            ulong moves = 0;

            int piece = square.piece.PieceTypeNoColour;

            return piece switch
            {
                Piece.Pawn => GetPawnMoves(square),
                Piece.Rook => GetOrthogonalMoves(square),
                Piece.Bishop => GetDiagonalMoves(square),
                Piece.Knight => GetKnightMoves(square),
                Piece.Queen => GetOrthogonalMoves(square) | GetDiagonalMoves(square),
                Piece.King => GetKingMoves(square),
                _ => moves
            };
        }

        static ulong GetOrthogonalMoves(Square square)
        {
            ulong validSquaresMask = 0;

            foreach (Coord dir in OrthogonalOffsets)
            {
                Coord coord = square.coord;
                
                for (int i = 0; i < 8; i++)
                {
                    if (!coord.IsValidSquare())
                    {
                        break;
                    }

                    coord += dir;
                    validSquaresMask |= coord.ToBitBoard();

                    if (square.piece.PieceTypeNoColour == Piece.King)
                    {
                        break;
                    }
                }
            }

            return validSquaresMask | square.coord.ToBitBoard();
        }

        static ulong GetDiagonalMoves(Square square)
        {
            ulong validSquaresMask = 0;

            foreach (Coord dir in DiagonalOffsets)
            {
                Coord coord = square.coord;

                for (int i = 0; i < 8; i++)
                {
                    if (!coord.IsValidSquare())
                    {
                        break;
                    }

                    coord += dir;
                    validSquaresMask |= coord.ToBitBoard();

                    if (square.piece.PieceTypeNoColour == Piece.King)
                    {
                        break;
                    }
                }
            }

            return validSquaresMask | square.coord.ToBitBoard();
        }
        
        static ulong GetKnightMoves(Square square)
        {
            ulong validSquaresMask = 0;

            if (square.piece.PieceType == Piece.None)
                return validSquaresMask;

            foreach (Coord dir in KnightOffsets)
            {
                Coord coord = square.coord + dir;

                if (!coord.IsValidSquare())
                {
                    continue;
                }

                validSquaresMask |= coord.ToBitBoard();
            }

            return validSquaresMask;
        }

        ulong GetPawnMoves(Square square)
        {
            ulong validSquaresMask = 0;

            Piece piece = square.piece;


            if (piece.PieceType == Piece.None)
                return validSquaresMask;

            if (piece.IsWhite)
            {   
                Coord singlePawnMove = square.coord + singlePawnOffset;
                Coord doublePawnMove = square.coord + doublePawnOffset;

                Coord leftCaptureMove = square.coord + captureLeftOffset;
                Coord rightCaptureMove = square.coord + captureRightOffset;

                if (singlePawnMove.IsValidSquare())
                {
                    if (square.coord.rank == Height - 7)
                    {
                        validSquaresMask |= doublePawnMove.ToBitBoard();
                    }

                    if (!squares[leftCaptureMove.ToIndex()].IsEmpty())
                    {
                        validSquaresMask |= leftCaptureMove.ToBitBoard();
                    }
                    if (!squares[rightCaptureMove.ToIndex()].IsEmpty())
                    {
                        validSquaresMask |= rightCaptureMove.ToBitBoard();
                    }
                    Console.WriteLine(squares[rightCaptureMove.ToIndex()].piece.PieceType);

                    validSquaresMask |= singlePawnMove.ToBitBoard();
                }
            } else
            {
                Coord singlePawnMove = square.coord - singlePawnOffset;
                Coord doublePawnMove = square.coord - doublePawnOffset;

                Coord leftCaptureMove = square.coord - captureRightOffset;
                Coord rightCaptureMove = square.coord - captureLeftOffset;

                if (singlePawnMove.IsValidSquare())
                {
                    if (square.coord.rank == Height - 2)
                    {
                        validSquaresMask |= doublePawnMove.ToBitBoard();
                    }

                    if (!squares[leftCaptureMove.ToIndex()].IsEmpty())
                    {
                        validSquaresMask |= leftCaptureMove.ToBitBoard();
                    }
                    if (!squares[rightCaptureMove.ToIndex()].IsEmpty())
                    {
                        validSquaresMask |= rightCaptureMove.ToBitBoard();
                    }
                    validSquaresMask |= singlePawnMove.ToBitBoard();
                }
            }

            return validSquaresMask;
        }

        ulong GetKingMoves(Square square)
        {
            ulong validSquaresMask = 0;

            //King logic


            validSquaresMask |= GetOrthogonalMoves(square);
            validSquaresMask |= GetDiagonalMoves(square);

            return validSquaresMask | square.coord.ToBitBoard();
        }

        void CastleShort()
        {

        }

        void CastleLong()
        {

        }
    }
}