namespace GameEngine.BitboardUtil
{

    public class BitboardManager {
        const int WIDTH = Board.WIDTH;
        const int HEIGHT = Board.HEIGHT;

        ulong allPiecesBitBoard;
        ulong whitePiecesBitBoard;
        ulong blackPiecesBitBoard;

        Square[] squares;

        public BitboardManager (Square[] squares) {
            this.squares = squares;
        }

        public void UpdateSquares (Square[] squares) {
            this.squares = squares;
        }

        static readonly Coord[] OrthogonalOffsets = { 
            new Coord(1, 0), new Coord(-1, 0),
            new Coord(0, 1), new Coord(0, -1)
        };
        static readonly Coord[] DiagonalOffsets = { 
            new Coord(1, 1), new Coord(-1, -1),
            new Coord(1, -1), new Coord(-1, 1)
        };
        static readonly Coord[] KnightOffsets = {
            new Coord(1, 2), new Coord(-1, 2),
            new Coord(2, 1), new Coord(2, -1),
            new Coord(1, -2), new Coord(-1, -2),
            new Coord(-2, 1), new Coord(-2, -1)
        };

        static readonly Coord singlePawnOffset = new Coord(0, 1);
        static readonly Coord doublePawnOffset = new Coord(0, 2);

        static readonly Coord captureLeftOffset = new Coord(-1, 1);
        static readonly Coord captureRightOffset = new Coord(1, 1);

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

        public ulong GetPiecesBitboard () {
            return GetWhitePiecesBitboard() | GetBlackPiecesBitboard(); 
        }

        public ulong GetWhitePiecesBitboard () {
            ulong bitboard = 0;
            for (int i = 0; i < WIDTH * HEIGHT; i++) {
                bitboard |= (squares[i].piece.IsWhite && !squares[i].IsEmpty()) ? ((ulong)1 << i) : 0;
            }
            return bitboard;
        }

        public ulong GetBlackPiecesBitboard () {
            ulong bitboard = 0;
            for (int i = 0; i < WIDTH * HEIGHT; i++) {
                bitboard |= (!squares[i].piece.IsWhite && !squares[i].IsEmpty()) ? ((ulong)1 << i) : 0;
            }
            return bitboard;
        }

        ulong GetOrthogonalMoves(Square square)
        {
            ulong validSquaresMask = 0;

            foreach (Coord dir in OrthogonalOffsets)
            {
                Coord coord = square.coord;
                
                for (int i = 0; i < 8; i++)
                {
                    coord += dir;
                    if (!coord.IsValidSquare())
                    {
                        break;
                    }

                    Square targetSquare = squares[coord.file + coord.rank * WIDTH];

                    bool canContinue = targetSquare.IsEmpty() || targetSquare == square;
                    bool isCapture = (targetSquare.piece.IsWhite == !square.piece.IsWhite) && !targetSquare.IsEmpty();

                    if (isCapture)
                    {
                        validSquaresMask |= coord.ToBitBoard();
                        break;
                    }

                    if (!canContinue)
                    {
                        break;
                    }

                    validSquaresMask |= coord.ToBitBoard();
                }
            }

            return validSquaresMask | square.coord.ToBitBoard();
        }

        ulong GetDiagonalMoves(Square square)
        {
            ulong validSquaresMask = 0;


            foreach (Coord dir in DiagonalOffsets)
            {
                Coord coord = square.coord;

                for (int i = 0; i < 8; i++)
                {
                    coord += dir;
                    if (!coord.IsValidSquare())
                    {
                        break;
                    }

                    Square targetSquare = squares[coord.file + coord.rank * WIDTH];

                    bool canContinue = targetSquare.IsEmpty() || targetSquare == square;
                    bool isCapture = (targetSquare.piece.IsWhite == !square.piece.IsWhite) && !targetSquare.IsEmpty();

                    if (isCapture)
                    {
                        validSquaresMask |= coord.ToBitBoard();
                        break;
                    }

                    if (!canContinue)
                    {
                        break;
                    }

                    validSquaresMask |= coord.ToBitBoard();
                }
            }

            return validSquaresMask | square.coord.ToBitBoard();
        }
        
        ulong GetKnightMoves(Square square)
        {
            ulong validSquaresMask = 0;

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

        //TODO: Definitely needs refactoring.
        //Perhaps compare to a bitboard of all pieces
        //instead of comparing to the squares directly?
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
                    if (squares[singlePawnMove.ToIndex()].IsEmpty())
                    {
                        if ((square.coord.rank == HEIGHT - 7) && squares[doublePawnMove.ToIndex()].IsEmpty())
                        {
                            validSquaresMask |= doublePawnMove.ToBitBoard();
                        }
                        validSquaresMask |= singlePawnMove.ToBitBoard();
                    }

                    if (!squares[leftCaptureMove.ToIndex()].IsEmpty())
                    {
                        validSquaresMask |= leftCaptureMove.ToBitBoard();
                    }
                    if (!squares[rightCaptureMove.ToIndex()].IsEmpty())
                    {
                        validSquaresMask |= rightCaptureMove.ToBitBoard();
                    }

                }
            } else
            {
                Coord singlePawnMove = square.coord - singlePawnOffset;
                Coord doublePawnMove = square.coord - doublePawnOffset;

                Coord leftCaptureMove = square.coord - captureRightOffset;
                Coord rightCaptureMove = square.coord - captureLeftOffset;

                if (singlePawnMove.IsValidSquare())
                {
                    if (squares[singlePawnMove.ToIndex()].IsEmpty())
                    {
                        if ((square.coord.rank == HEIGHT - 2) && squares[doublePawnMove.ToIndex()].IsEmpty())
                        {
                            validSquaresMask |= doublePawnMove.ToBitBoard();
                        }
                        validSquaresMask |= singlePawnMove.ToBitBoard();
                    }

                    if (!squares[leftCaptureMove.ToIndex()].IsEmpty())
                    {
                        validSquaresMask |= leftCaptureMove.ToBitBoard();
                    }
                    if (!squares[rightCaptureMove.ToIndex()].IsEmpty())
                    {
                        validSquaresMask |= rightCaptureMove.ToBitBoard();
                    }
                }
            }

            return validSquaresMask;
        }

        ulong GetKingMoves(Square square)
        {
            ulong validSquaresMask = 0;

            //King logic



            return validSquaresMask | square.coord.ToBitBoard();
        }

        public static string PrintBitboard(ulong bitboard) {
            string bitboardString = "";

            for (int i = 0; i < 64; i++) {
                ulong step = (ulong)1 << i;

                bitboardString += (step | bitboard) != bitboard ? "0 " : "1 ";
                if ((i + 1) % 8 == 0) {
                    bitboardString += "\n";
                } 
            }
            return bitboardString;
        }
    }
}

