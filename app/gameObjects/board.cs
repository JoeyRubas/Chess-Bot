
using System.Diagnostics;

namespace gameObjects;

public class Board

{
    public const int WPawn = 0;
    public const int WBishop = 1;
    public const int WKnight = 2;
    public const int WRook = 3;
    public const int WQueen = 4;
    public const int WKing = 5;
    public const int BPawn = 6;
    public const int BBishop = 7;
    public const int BKnight = 8;
    public const int BRook = 9;
    public const int BQueen = 10;
    public const int BKing = 11;
    public const int enPassant = 12;
    public const bool whiteTurn = false;
    public const bool blackTurn = true;
    private int[] BishopDirections = { -9, -7, 7, 9 };
    private int[] RookDirections = { -8, 8, -1, 1 };
    private int[] KingDirections = { -9, -7, 7, 9, -8, 8, -1, 1 };
    private int[] KnightDirections = { -6, 6, -10, 10, -15, 15, -17, 17 };
    private int[] WhitePromotionOptions = { WBishop, WKnight, WRook, WQueen };
    private int[] BlackPromotionOptions = { BBishop, BKnight, BRook, BQueen };
    public ulong[] bitboard;
    public bool turn;
    private byte castling;
    //First 4 bits reserved, 1 by default
    //bit 5 - white has long castle rights
    //bit 6 - white has short castle rights
    //bit 7 - black has long castle rights
    //bit 8 - black has short castle rights





    public Board()
    {
        bitboard = new ulong[13];
        bitboard[WPawn] = 0xff00;
        bitboard[WBishop] = 0x24;
        bitboard[WKnight] = 0x42;
        bitboard[WRook] = 0x81;
        bitboard[WQueen] = 0x8;
        bitboard[WKing] = 0x10;
        bitboard[BPawn] = 0xff000000000000;
        bitboard[BBishop] = 0x2400000000000000;
        bitboard[BKnight] = 0x4200000000000000;
        bitboard[BRook] = 0x8100000000000000;
        bitboard[BQueen] = 0x800000000000000;
        bitboard[BKing] = 0x1000000000000000;
        bitboard[enPassant] = 0;
        castling = 0xff;
        turn = whiteTurn;

    }

    public void renderBoard()
    {
        string pieceMappings = "♙♗♘♖♕♔♟♝♞♜♛♚";

        for (int i = 7; i >= 0; i--)
        {
            Console.Write(i + 1);
            for (int j = 0; j < 8; j++)
            {
                ulong mask = 1UL << (j + 8 * i);
                char outchar = '-';
                for (int k = 0; k < 12; k++)
                {
                    if ((mask & bitboard[k]) > 0)
                    {
                        outchar = pieceMappings[k];
                        break;
                    }
                }
                if (outchar == '-') Console.Write("□ ");
                else Console.Write(outchar);
            }
            Console.Write("\n");
        }
    }



    public void MakeMove(Move move)
    {
        //Console.WriteLine("MakeMove calledfor move " + move.getString());
        // bool moveIsLegal = false;
        bool moveIsLegal = true;
        // HashSet<Move> LegalMoves = GetLegalMoves();
        // foreach (Move legalMove in LegalMoves)
        // {
        //     if (legalMove.startSquare == move.startSquare && legalMove.endSquare == move.endSquare)
        //     {
        //         moveIsLegal = true;
        //         break;
        //     }
        // }
        if (!moveIsLegal)
        {
            Console.WriteLine("Illegal move attempted");
            Console.WriteLine("Current board:");
            renderBoard();
            Console.WriteLine("Attempted move: " + move.getString() + "From: " + move.startSquare + "To: " + move.endSquare);
            throw new Exception("Illegal move");
        }

        if ((move.castling & 0xf) == 0)
        {
            int currentPeiceType = move.promotionPieceType;
            if (currentPeiceType == -1)
            {
                currentPeiceType = move.pieceType;
            }

            // Remove piece from start square
            ulong startMask = ~(1UL << move.startSquare);
            for (int i = 0; i < bitboard.Length; i++)
            {
                bitboard[i] &= startMask;
            }

            // Handle capture
            if (move.captureIndex != -1)
            {
                ulong captureMask = ~(1UL << move.captureIndex);
                for (int i = 0; i < bitboard.Length; i++)
                {
                    bitboard[i] &= captureMask;
                }
            }

            // Place piece on end square
            ulong endMask = 1UL << move.endSquare;


            bitboard[currentPeiceType] |= endMask;


        }
        else if ((move.castling & 0x1) > 0)
        {
            //Console.WriteLine("White long castle hit");
            bitboard[WKing] = 0x4;
            bitboard[WRook] = (bitboard[WRook] & 0xfffffffffffffffe) | 0x8;
        }
        else if ((move.castling & 0x2) > 0)
        {
            //Console.WriteLine("White short castle hit");
            bitboard[WKing] = 0x40;
            bitboard[WRook] = (bitboard[WRook] & 0xffffffffffffff7f) | 0x20;
        }
        else if ((move.castling & 0x4) > 0)
        {
            //Console.WriteLine("Black long castle hit");
            bitboard[BKing] = 0x400000000000000;
            bitboard[BRook] = (bitboard[WRook] & 0xfeffffffffffffff) | 0x800000000000000;
        }
        else if ((move.castling & 0x8) > 0)
        {
            //Console.WriteLine("Black short castle hit");
            bitboard[WKing] = 0x4000000000000000;
            bitboard[WRook] = (bitboard[WRook] & 0x7fffffffffffffff) | 0x2000000000000000;
        }


        // Update en passant
        bitboard[enPassant] = move.enPassantGained;

        // Update castling rights
        castling = (byte)(castling ^ move.castling);
        castling = (byte)(castling | 0xf);

        // Switch turn
        turn = !turn;


    }

    public void UnmakeMove(Move move)
    {
        // Switch turn back
        turn = !turn;

        // Reverse the update of castling rights
        castling = (byte)(castling | move.castling);

        // Reverse the update of en passant
        bitboard[enPassant] = move.enPassantLost;

        if ((move.castling & 0xf) == 0)
        {
            // Handle the basic move reversal
            ulong startMask = 1UL << move.startSquare;
            ulong endMask = ~(1UL << move.endSquare);

            bitboard[move.pieceType] = bitboard[move.pieceType] | startMask;
            if (move.promotionPieceType == -1)
            {
                bitboard[move.pieceType] &= endMask;
            }
            else
            {
                bitboard[move.promotionPieceType] &= endMask;
            }

            // If a capture occurred, restore the captured piece
            if (move.captureIndex != -1)
            {
                ulong captureMask = 1UL << move.captureIndex;
                bitboard[move.captureType] |= captureMask;
            }
        }
        else if ((move.castling & 0x1) > 0)
        {
            bitboard[WKing] = 0x10;
            bitboard[WRook] = (bitboard[WRook] & 0xfffffffffffffff7) | 0x1;
        }
        else if ((move.castling & 0x2) > 0)
        {
            bitboard[WKing] = 0x10;
            bitboard[WRook] = (bitboard[WRook] & 0xffffffffffffffdf) | 0x80;
        }
        else if ((move.castling & 0x4) > 0)
        {
            bitboard[BKing] = 0x1000000000000000;
            bitboard[BRook] = (bitboard[WRook] & 0xf7ffffffffffffff) | 0x100000000000000;
        }
        else if ((move.castling & 0x8) > 0)
        {
            bitboard[WKing] = 0x1000000000000000;
            bitboard[WRook] = (bitboard[WRook] & 0xdfffffffffffffff) | 0x8000000000000000;
        }


    }

    public bool IsGameOver()
    {
        return (bitboard[WKing] == 0 | bitboard[BKing] == 0);
    }

    private void addPieceMoves(HashSet<Move> legalMoves, int boardIndex, ulong otherColorPieces, ulong allPieces, int[] directions, int maxSteps, int peiceType)
    {
        foreach (int direction in directions)
        {
            int currentPos = boardIndex;
            int steps = 0;

            while (steps < maxSteps)
            {
                int nextPos = currentPos + direction;

                // Check if the position is out of board bounds
                if (nextPos > 63 || nextPos < 0)
                    break;

                // Prevent wrapping
                int currentRow = currentPos / 8;
                int currentCol = currentPos % 8;
                int nextRow = nextPos / 8;
                int nextCol = nextPos % 8;

                // Calculate the wrapping from one row or column to another based on piece type
                int rowDifference = Math.Abs(currentRow - nextRow);
                int colDifference = Math.Abs(currentCol - nextCol);

                ulong nextPosMask = 1UL << nextPos;


                if (peiceType == WKnight || peiceType == BKnight)
                {
                    if (!((rowDifference == 2 && colDifference == 1) || (rowDifference == 1 && colDifference == 2))) break;
                }
                else if ((direction % 8 == 0 && rowDifference > 1) || (direction % 8 != 0 && (colDifference > 1 || rowDifference > 1)))
                    break;

                Move move = new Move(boardIndex, nextPos, peiceType);
                move.enPassantLost = enPassant;
                if (peiceType == BRook)
                {
                    if (currentPos == 56) move.castling = (byte)(castling & 0x40);
                    else if (currentPos == 63) move.castling = (byte)(castling & 0x80);
                }
                if (peiceType == WRook)
                {
                    if (currentPos == 0) move.castling = (byte)(castling & 0x10);
                    else if (currentPos == 7) move.castling = (byte)(castling & 0x20);
                }

                // Collision: stop if there is a piece
                if ((allPieces & (nextPosMask)) != 0)
                {
                    // If it's an other colored piece, it can be captured
                    if ((otherColorPieces & (nextPosMask)) != 0)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            if ((bitboard[i] & nextPosMask) > 0)
                            {
                                move.captureType = i;
                                move.captureIndex = nextPos;
                                if (i == BRook)
                                {
                                    if (nextPos == 56) move.castling = (byte)(castling & 0x40);
                                    else if (nextPos == 63) move.castling = (byte)(castling & 0x80);
                                }
                                if (i == WRook)
                                {
                                    if (nextPos == 0) move.castling = (byte)(castling & 0x10);
                                    else if (nextPos == 7) move.castling = (byte)(castling & 0x20);
                                }
                            }
                        }
                        move.debugData = "Move added on line 309";
                        legalMoves.Add(move);
                    }
                    break;
                }

                // Add move if no collision
                legalMoves.Add(move);
                currentPos = nextPos; // Continue in the same direction

                steps++; // Increment steps based on piece capability
            }
        }
    }


    public HashSet<Move> GetLegalMoves()
    {

        ulong whitePieces = 0UL;
        for (int i = 0; i < 6; i++)
        {
            whitePieces = whitePieces | bitboard[i];
        }


        ulong blackPieces = 0UL;
        for (int i = 6; i < 12; i++)
        {
            blackPieces = blackPieces | bitboard[i];
        }

        ulong allPieces = whitePieces | blackPieces;

        HashSet<Move> legalMoves = new HashSet<Move>(218);
        ulong mask = 1UL;
        for (int boardIndex = 0; boardIndex < 64; boardIndex++)
        {
            int pieceType = -1;
            for (int i = 0; i < 12; i++)
            {
                if ((bitboard[i] & mask) > 0)
                {
                    pieceType = i;
                    break;
                }
            }
            if (turn == whiteTurn)
            {
                switch (pieceType)
                {
                    case WPawn:
                        if (((mask << 8) & allPieces) == 0)
                        {
                            if (boardIndex > 47)
                            {
                                foreach (int peice in WhitePromotionOptions)
                                {
                                    Move move = new Move(boardIndex, boardIndex + 8, pieceType);
                                    move.promotionPieceType = peice;
                                    move.enPassantLost = enPassant;
                                    legalMoves.Add(move);
                                }
                            }
                            else
                            {
                                Move move = new Move(boardIndex, boardIndex + 8, pieceType);
                                move.enPassantLost = enPassant;
                                legalMoves.Add(move);
                            }
                        }
                        if (((mask << 7) & blackPieces) > 0 & !(boardIndex % 8 == 0))
                        {
                            if (boardIndex > 47)
                            {
                                foreach (int peice in WhitePromotionOptions)
                                {
                                    Move move = new Move(boardIndex, boardIndex + 7, pieceType);
                                    move.promotionPieceType = peice;
                                    move.enPassantLost = enPassant;
                                    legalMoves.Add(move);
                                }
                            }
                            else
                            {
                                Move move = new Move(boardIndex, boardIndex + 7, pieceType);
                                move.enPassantLost = enPassant;
                                legalMoves.Add(move);
                            }
                        }
                        if (((mask << 9) & blackPieces) > 0 & !(boardIndex % 8 == 7))
                        {
                            if (boardIndex > 47)
                            {
                                foreach (int peice in WhitePromotionOptions)
                                {
                                    Move move = new Move(boardIndex, boardIndex + 9, pieceType);
                                    move.promotionPieceType = peice;
                                    move.enPassantLost = enPassant;
                                    legalMoves.Add(move);
                                }
                            }
                            else
                            {
                                Move move = new Move(boardIndex, boardIndex + 9, pieceType);
                                move.enPassantLost = enPassant;
                                legalMoves.Add(move);
                            }
                        }
                        if ((7 < boardIndex) & (boardIndex < 16) & (((mask << 16) & allPieces) == 0))
                        {
                            Move move = new Move(boardIndex, boardIndex + 16, pieceType);
                            move.enPassantLost = enPassant;
                            move.enPassantGained = mask;
                            legalMoves.Add(move);
                        }
                        break;

                    case WBishop:
                        addPieceMoves(legalMoves, boardIndex, blackPieces, allPieces, BishopDirections, 7, pieceType);
                        break;
                    case WKnight:
                        addPieceMoves(legalMoves, boardIndex, blackPieces, allPieces, KnightDirections, 1, pieceType);
                        break;
                    case WRook:
                        addPieceMoves(legalMoves, boardIndex, blackPieces, allPieces, RookDirections, 7, pieceType);
                        break;
                    case WQueen:
                        addPieceMoves(legalMoves, boardIndex, blackPieces, allPieces, RookDirections, 7, pieceType);
                        addPieceMoves(legalMoves, boardIndex, blackPieces, allPieces, BishopDirections, 7, pieceType);
                        break;
                    case WKing:
                        addPieceMoves(legalMoves, boardIndex, blackPieces, allPieces, KingDirections, 1, pieceType);
                        break;
                }
            }

            if (turn == blackTurn)
            {
                switch (pieceType)
                {
                    case BPawn:
                        if (((mask >> 8) & allPieces) == 0)
                        {
                            if (boardIndex < 16)
                            {
                                foreach (int peice in WhitePromotionOptions)
                                {
                                    Move move = new Move(boardIndex, boardIndex - 8, pieceType);
                                    move.promotionPieceType = peice;
                                    move.enPassantLost = enPassant;
                                    legalMoves.Add(move);
                                }
                            }
                            else
                            {
                                Move move = new Move(boardIndex, boardIndex - 8, pieceType);
                                move.enPassantLost = enPassant;
                                legalMoves.Add(move);
                            }
                        }
                        if (((mask >> 7) & whitePieces) > 0 & !(boardIndex % 8 == 7))
                        {
                            if (boardIndex < 16)
                            {
                                foreach (int peice in WhitePromotionOptions)
                                {
                                    Move move = new Move(boardIndex, boardIndex - 7, pieceType);
                                    move.promotionPieceType = peice;
                                    move.enPassantLost = enPassant;
                                    legalMoves.Add(move);
                                }
                            }
                            else
                            {
                                Move move = new Move(boardIndex, boardIndex - 7, pieceType);
                                move.enPassantLost = enPassant;
                                legalMoves.Add(move);
                            }
                        }
                        if (((mask >> 9) & whitePieces) > 0 & !(boardIndex % 8 == 0))
                        {
                            if (boardIndex < 16)
                            {
                                foreach (int peice in WhitePromotionOptions)
                                {
                                    Move move = new Move(boardIndex, boardIndex - 9, pieceType);
                                    move.promotionPieceType = peice;
                                    move.enPassantLost = enPassant;
                                    legalMoves.Add(move);
                                }
                            }
                            else
                            {
                                Move move = new Move(boardIndex, boardIndex - 9, pieceType);
                                move.enPassantLost = enPassant;
                                legalMoves.Add(move);
                            }
                        }
                        if ((47 < boardIndex) & (boardIndex < 56) & (((mask >> 16) & allPieces) == 0))
                        {
                            Move move = new Move(boardIndex, boardIndex - 16, pieceType);
                            move.enPassantLost = enPassant;
                            move.enPassantGained = mask;
                            legalMoves.Add(move);
                        }
                        break;
                    case BBishop:
                        addPieceMoves(legalMoves, boardIndex, whitePieces, allPieces, BishopDirections, 7, pieceType);
                        break;
                    case BKnight:
                        addPieceMoves(legalMoves, boardIndex, whitePieces, allPieces, KnightDirections, 1, pieceType);
                        break;
                    case BRook:
                        addPieceMoves(legalMoves, boardIndex, whitePieces, allPieces, RookDirections, 7, pieceType);
                        break;
                    case BQueen:
                        addPieceMoves(legalMoves, boardIndex, whitePieces, allPieces, RookDirections, 7, pieceType);
                        addPieceMoves(legalMoves, boardIndex, whitePieces, allPieces, BishopDirections, 7, pieceType);
                        break;
                    case BKing:
                        addPieceMoves(legalMoves, boardIndex, whitePieces, allPieces, KingDirections, 1, pieceType);
                        break;
                }
            }
            mask = mask << 1;
        }
        //Castling
        if (turn == whiteTurn)
        {
            if ((castling & 0x10) > 0 & (allPieces & 0xe) == 0)
            {
                legalMoves.Add(new Move((byte)(0x31 & castling)));
            }
            if ((castling & 0x20) > 0 & (allPieces & 0x60) == 0)
            {
                legalMoves.Add(new Move((byte)(0x32 & castling)));
            }
        }
        if (turn == blackTurn)
        {
            if ((castling & 0x40) > 0 & (allPieces & 0xe00000000000000) == 0)
            {
                legalMoves.Add(new Move((byte)(0xc4 & castling)));
            }
            if ((castling & 0x80) > 0 & (allPieces & 0x6000000000000000) == 0)
            {
                legalMoves.Add(new Move((byte)(0xc8 & castling)));
            }
        }
        return legalMoves;
    }

}