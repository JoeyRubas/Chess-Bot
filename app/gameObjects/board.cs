
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
    private ulong[] bitboard;
    private bool WhiteCastleValid = false;
    private bool BlackCastleValid = false;




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

    public ushort makeMove(String move)
    {
        return makeMove(utility.StringMoveToShort(move));
    }

    public ushort makeMove(ushort move)
    {

        ushort startSquareIdx = (ushort)((move >>> 10) & 0x3f);
        Console.WriteLine(Convert.ToString(startSquareIdx, 2));
        ushort endSquareIdx = (ushort)((move >>> 4) & 0x3f);
        ulong startSquareMask = 1UL << (startSquareIdx);
        Console.WriteLine(Convert.ToString((long)startSquareMask, 2));
        ulong endSqareMask = 1UL << (endSquareIdx);
        char outchar = '-';
        for (int k = 0; k < 12; k++)
        {
            if ((startSquareMask & bitboard[k]) > 0)
            {
                bitboard[k] = bitboard[k] ^ startSquareMask ^ endSqareMask;

            }
        }

        return move;
    }

    public ushort unmakeMove(ushort move)
    {
        int startIndex = (move >> 10) & 0x3F; // Extract first 6 bits
        int endIndex = (move >> 4) & 0x3F; // Extract next 6 bits
        return makeMove(utility.IndexsToShortMove(endIndex, startIndex, 0));

    }

    private void addRookMoves(HashSet<ushort> legalMoves, int boardIndex, ulong otherColorPieces, ulong allPieces)
    {
        foreach (int direction in RookDirections)
        {
            int currentPos = boardIndex;
            while (true)
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

                // Prevent wrapping from one row or column to another
                if (Math.Abs(currentRow - nextRow) > 0 & Math.Abs(currentCol - nextCol) > 0)
                    break;


                // Collision: stop if there is a piece
                if ((allPieces & (1UL << nextPos)) != 0)
                {
                    // If it's a different colored piece, it can be captured
                    if ((otherColorPieces & (1UL << nextPos)) != 0)
                    {
                        legalMoves.Add(utility.IndexsToShortMove(boardIndex, nextPos, 0));
                    }
                    break;
                }

                // Add move if no collision
                legalMoves.Add(utility.IndexsToShortMove(boardIndex, nextPos, 0));
                currentPos = nextPos; // Continue in the same direction
            }
        }

    }

    private void addBishopMoves(HashSet<ushort> legalMoves, int boardIndex, ulong otherColorPieces, ulong allPieces)
    {
        foreach (int direction in BishopDirections)
        {
            int currentPos = boardIndex;
            while (true)
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

                // Prevent wrapping from one row or column to another
                if (Math.Abs(currentRow - nextRow) > 1 || Math.Abs(currentCol - nextCol) > 1)
                    break;


                // Collision: stop if there is a piece
                if ((allPieces & (1UL << nextPos)) != 0)
                {
                    // If it's an other colored piece, it can be captured
                    if ((otherColorPieces & (1UL << nextPos)) != 0)
                    {
                        legalMoves.Add(utility.IndexsToShortMove(boardIndex, nextPos, 0));
                    }
                    break;
                }

                // Add move if no collision
                legalMoves.Add(utility.IndexsToShortMove(boardIndex, nextPos, 0));
                currentPos = nextPos; // Continue in the same direction
            }
        }
    }

    private void addKingMoves(HashSet<ushort> legalMoves, int boardIndex, ulong otherColorPieces, ulong allPieces)
    {
        foreach (int direction in KingDirections)
        {
            int currentPos = boardIndex;

            int nextPos = currentPos + direction;

            // Check if the position is out of board bounds
            if (nextPos > 63 || nextPos < 0)
                break;

            // Prevent wrapping
            int currentRow = currentPos / 8;
            int currentCol = currentPos % 8;
            int nextRow = nextPos / 8;
            int nextCol = nextPos % 8;

            // Prevent wrapping from one row or column to another
            if (Math.Abs(currentRow - nextRow) > 1 || Math.Abs(currentCol - nextCol) > 1)
                break;
            // Collision: stop if there is a piece
            if ((allPieces & (1UL << nextPos)) != 0)
            {
                // If it's an other colored piece, it can be captured
                if ((otherColorPieces & (1UL << nextPos)) != 0)
                {
                    legalMoves.Add(utility.IndexsToShortMove(boardIndex, nextPos, 0));
                }
                break;
            }
            // Add move if no collision
            legalMoves.Add(utility.IndexsToShortMove(boardIndex, nextPos, 0));
            currentPos = nextPos; // Continue in the same direction
        }
    }

    private void addKnightMoves(HashSet<ushort> legalMoves, int boardIndex, ulong otherColorPieces, ulong allPieces)
    {
        foreach (int direction in KnightDirections)
        {
            int currentPos = boardIndex;

            int nextPos = currentPos + direction;

            // Check if the position is out of board bounds
            if (nextPos > 63 || nextPos < 0)
                break;

            // Prevent wrapping
            int currentRow = currentPos / 8;
            int currentCol = currentPos % 8;
            int nextRow = nextPos / 8;
            int nextCol = nextPos % 8;

            // Prevent wrapping from one row or column to another
            if (Math.Abs(currentRow - nextRow) > 2 || Math.Abs(currentCol - nextCol) > 2)
                break;
            // Collision: stop if there is a piece
            if ((allPieces & (1UL << nextPos)) != 0)
            {
                // If it's an other colored piece, it can be captured
                if ((otherColorPieces & (1UL << nextPos)) != 0)
                {
                    legalMoves.Add(utility.IndexsToShortMove(boardIndex, nextPos, 0));
                }
                break;
            }
            // Add move if no collision
            legalMoves.Add(utility.IndexsToShortMove(boardIndex, nextPos, 0));
            currentPos = nextPos; // Continue in the same direction
        }
    }

    public HashSet<ushort> GetLegalMoves(bool turn)
    {

        ulong whitePieces = 0UL;
        for (int i = 0; i < 6; i++)
        {
            whitePieces = whitePieces | bitboard[i];
        }


        ulong blackPieces = 0UL;
        for (int i = 6; i < 11; i++)
        {
            blackPieces = blackPieces | bitboard[i];
        }

        ulong allPieces = whitePieces | blackPieces;

        HashSet<ushort> legalMoves = new HashSet<ushort>(218);
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
                        if (((mask << 8) & allPieces) == 0) legalMoves.Add(utility.IndexsToShortMove(boardIndex, boardIndex + 8, 0));
                        if (((mask << 7) & blackPieces) > 0) legalMoves.Add(utility.IndexsToShortMove(boardIndex, boardIndex + 7, 0));
                        if (((mask << 9) & blackPieces) > 0) legalMoves.Add(utility.IndexsToShortMove(boardIndex, boardIndex + 9, 0));
                        if ((7 < boardIndex) & (boardIndex < 16) & (((mask << 16) & allPieces) == 0)) legalMoves.Add(utility.IndexsToShortMove(boardIndex, boardIndex + 16, 0));
                        break;

                    case WBishop:
                        addBishopMoves(legalMoves, boardIndex, blackPieces, allPieces);
                        break;
                    case WKnight:
                        addKnightMoves(legalMoves, boardIndex, blackPieces, allPieces);
                        break;
                    case WRook:
                        addRookMoves(legalMoves, boardIndex, blackPieces, allPieces);
                        break;
                    case WQueen:
                        addBishopMoves(legalMoves, boardIndex, blackPieces, allPieces);
                        addRookMoves(legalMoves, boardIndex, blackPieces, allPieces);
                        break;
                    case WKing:
                        addKingMoves(legalMoves, boardIndex, blackPieces, allPieces);
                        break;
                }
            }

            if (turn == blackTurn)
            {
                switch (pieceType)
                {
                    case BPawn:
                        Console.WriteLine("here!");
                        if (((mask >> 8) & allPieces) == 0) legalMoves.Add(utility.IndexsToShortMove(boardIndex, boardIndex - 8, 0));
                        if (((mask >> 7) & whitePieces) > 0) legalMoves.Add(utility.IndexsToShortMove(boardIndex, boardIndex - 7, 0));
                        if (((mask >> 9) & whitePieces) > 0) legalMoves.Add(utility.IndexsToShortMove(boardIndex, boardIndex - 9, 0));
                        if ((47 < boardIndex) & (boardIndex < 56) & (((mask >> 16) & allPieces) == 0)) legalMoves.Add(utility.IndexsToShortMove(boardIndex, boardIndex - 16, 0));
                        break;

                    case BBishop:
                        addBishopMoves(legalMoves, boardIndex, whitePieces, allPieces);
                        break;
                    case BKnight:
                        addKnightMoves(legalMoves, boardIndex, blackPieces, allPieces);
                        break;
                    case BRook:
                        addRookMoves(legalMoves, boardIndex, whitePieces, allPieces);
                        break;
                    case BQueen:
                        addBishopMoves(legalMoves, boardIndex, whitePieces, allPieces);
                        addRookMoves(legalMoves, boardIndex, whitePieces, allPieces);
                        break;
                    case BKing:
                        addKingMoves(legalMoves, boardIndex, whitePieces, allPieces);
                        break;
                }
            }
            mask = mask << 1;
        }
        return legalMoves;
    }

}