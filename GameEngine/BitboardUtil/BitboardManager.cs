namespace GameEngine.BitboardUtil
{

    public class BitboardManager
    {
        const int WIDTH = Board.WIDTH;
        const int HEIGHT = Board.HEIGHT;

        Square[] squares;

        public BitboardManager(Square[] squares)
        {
            this.squares = squares;
        }

        public void UpdateSquares(Square[] squares)
        {
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

        public static readonly Coord singlePawnOffset = new Coord(0, 1);
        public static readonly Coord doublePawnOffset = new Coord(0, 2);

        public static readonly Coord captureLeftOffset = new Coord(-1, 1);
        public static readonly Coord captureRightOffset = new Coord(1, 1);

        public static readonly Coord leftOffset = new Coord(-1, 0);
        public static readonly Coord rightOffset = new Coord(1, 0);

        public ulong GetMovesBitboard(Square square)
        {
            ulong moves = 0;

            int piece = square.piece.PieceTypeNoColour;

            ulong blockingPiecesMask = square.piece.IsWhite ? ~GetWhitePiecesBitboard() : ~GetBlackPiecesBitboard();
            ulong captureablePiecesMask = square.piece.IsWhite ? GetBlackPiecesBitboard() : GetWhitePiecesBitboard();

            return piece switch
            {
                Piece.Pawn => (GetPawnMoves(square) & blockingPiecesMask) | (GetPawnAttacks(square) & captureablePiecesMask),
                Piece.Rook => GetOrthogonalMoves(square) & blockingPiecesMask,
                Piece.Bishop => GetDiagonalMoves(square) & blockingPiecesMask,
                Piece.Knight => GetKnightMoves(square) & blockingPiecesMask,
                Piece.Queen => (GetOrthogonalMoves(square) | GetDiagonalMoves(square)) & blockingPiecesMask,
                Piece.King => GetLegalKingMoves(square) & blockingPiecesMask,
                _ => moves
            };
        }

        public ulong GetPiecesBitboard() => GetWhitePiecesBitboard() | GetBlackPiecesBitboard();

        public ulong GetPiecesBitboard(bool isWhite) => isWhite ? GetWhitePiecesBitboard() : GetBlackPiecesBitboard();

        public ulong GetWhitePiecesBitboard()
        {
            ulong bitboard = 0;
            for (int i = 0; i < WIDTH * HEIGHT; i++)
            {
                bitboard |= (squares[i].piece.IsWhite && !squares[i].IsEmpty()) ? ((ulong)1 << i) : 0;
            }
            return bitboard;
        }

        public ulong GetBlackPiecesBitboard()
        {
            ulong bitboard = 0;
            for (int i = 0; i < WIDTH * HEIGHT; i++)
            {
                bitboard |= (!squares[i].piece.IsWhite && !squares[i].IsEmpty()) ? ((ulong)1 << i) : 0;
            }
            return bitboard;
        }

        public ulong GetAllAttacks(bool isWhite)
        {
            ulong attackedSquaresBitboard = 0;

            foreach (Square square in squares)
            {
                if (square.IsEmpty() || (square.piece.IsWhite != isWhite))
                {
                    continue;
                }
                else
                {
                    switch (square.piece.PieceTypeNoColour)
                    {
                        case Piece.King:
                            attackedSquaresBitboard |= GetAllKingMoves(square);
                            break;

                        case Piece.Pawn:
                            attackedSquaresBitboard |= GetPawnAttacks(square);
                            break;

                        case Piece.Queen:
                            attackedSquaresBitboard |= (GetOrthogonalMoves(square) | GetDiagonalMoves(square));
                            break;

                        case Piece.Rook:
                            attackedSquaresBitboard |= GetOrthogonalMoves(square);
                            break;

                        case Piece.Bishop:
                            attackedSquaresBitboard |= GetDiagonalMoves(square);
                            break;

                        case Piece.Knight:
                            attackedSquaresBitboard |= GetKnightMoves(square);
                            break;
                        default:
                            break;
                    }
                }
            }
            Console.WriteLine(PrintBitboard(attackedSquaresBitboard));
            return attackedSquaresBitboard;
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

                    bool canContinue = targetSquare.IsEmpty();

                    //Add the currently target move to the list of valid squares,
                    //even if the piece is the same colour as the piece.
                    //This is so that we can test if a piece is defended
                    //when calculating king moves
                    if (!canContinue)
                    {
                        validSquaresMask |= coord.ToBitBoard();
                        break;
                    }

                    validSquaresMask |= coord.ToBitBoard();
                }
            }

            return validSquaresMask;
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

                    //Continue adding moves through the opposite coloured king,
                    //so that the king cannot get out of check by simply
                    //taking a step back.
                    int oppositeColouredKing = targetSquare.piece.IsWhite ? Piece.King | Piece.Black : Piece.King | Piece.White;
                    bool canContinue = targetSquare.IsEmpty() || targetSquare.piece.PieceType == oppositeColouredKing;

                    //Add the currently target move to the list of valid squares,
                    //even if the piece is the same colour as the piece.
                    //This is so that we can test if a piece is defended
                    //when calculating king moves
                    if (!canContinue)
                    {
                        validSquaresMask |= coord.ToBitBoard();
                        break;
                    }

                    validSquaresMask |= coord.ToBitBoard();
                }
            }
            return validSquaresMask;
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

        ulong GetPawnMoves(Square square)
        {
            ulong validMoveMask = 0;

            Piece piece = square.piece;
            Coord coord = square.coord;

            if (piece.PieceType == Piece.None)
                return validMoveMask;

            if (piece.IsWhite)
            {
                Coord singlePawnMove = coord + singlePawnOffset;
                Coord doublePawnMove = coord + doublePawnOffset;

                if (singlePawnMove.IsValidSquare())
                {
                    if (coord.rank == HEIGHT - 7)
                    {
                        validMoveMask |= doublePawnMove.ToBitBoard();
                    }

                    validMoveMask |= singlePawnMove.ToBitBoard();
                }
            }
            else
            {
                Coord singlePawnMove = coord - singlePawnOffset;
                Coord doublePawnMove = coord - doublePawnOffset;

                if (singlePawnMove.IsValidSquare())
                {
                    if (coord.rank == HEIGHT - 2)
                    {
                        validMoveMask |= doublePawnMove.ToBitBoard();
                    }

                    validMoveMask |= singlePawnMove.ToBitBoard();
                }
            }

            return validMoveMask;
        }

        public ulong GetPawnAttacks(Square square)
        {
            ulong captureMask = 0;

            Piece piece = square.piece;
            Coord coord = square.coord;

            if (piece.PieceType == Piece.None)
                return captureMask;

            if (piece.IsWhite)
            {
                Coord leftCaptureMove = coord + captureLeftOffset;
                Coord rightCaptureMove = coord + captureRightOffset;

                captureMask |= leftCaptureMove.ToBitBoard();
                captureMask |= rightCaptureMove.ToBitBoard();
            }
            else
            {
                Coord leftCaptureMove = coord - captureRightOffset;
                Coord rightCaptureMove = coord - captureLeftOffset;

                captureMask |= leftCaptureMove.ToBitBoard();
                captureMask |= rightCaptureMove.ToBitBoard();
            }

            return captureMask;
        }

        ulong GetLegalKingMoves(Square square)
        {
            ulong validSquaresMask = GetAllKingMoves(square);
            ulong attackedSquaresMask = GetAllAttacks(!square.piece.IsWhite);

            return validSquaresMask & ~attackedSquaresMask;
        }

        ulong GetAllKingMoves(Square square)
        {
            ulong validSquaresMask = 0;

            foreach (Coord dir in OrthogonalOffsets)
            {
                Coord coord = square.coord + dir;
                if (coord.IsValidSquare())
                {
                    validSquaresMask |= coord.ToBitBoard();
                }
            }
            foreach (Coord dir in DiagonalOffsets)
            {
                Coord coord = square.coord + dir;
                if (coord.IsValidSquare())
                {
                    validSquaresMask |= coord.ToBitBoard();
                }
            }

            return validSquaresMask;
        }

        public bool IsInCheck(Square square)
        {
            ulong attackedSquaresMask = GetAllAttacks(!square.piece.IsWhite);
            return (square.coord.ToBitBoard() & attackedSquaresMask) > 0;
        }

        public static bool Contains(ulong x, ulong y)
        {
            return (x | y) == x;
        }

        public static string PrintBitboard(ulong bitboard)
        {
            string bitboardString = "";

            for (int i = 0; i < 64; i++)
            {
                ulong step = (ulong)1 << i;

                bitboardString += (step | bitboard) != bitboard ? "0 " : "1 ";
                if ((i + 1) % 8 == 0)
                {
                    bitboardString += "\n";
                }
            }
            return bitboardString;
        }
    }
}

