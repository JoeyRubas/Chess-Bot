namespace gameObjects;

public static class utility
{


    public static ushort StringMoveToShort(string move)
    {
        /*
            String Move:
            
            [peice starting square][peice ending square]
            Example:
            A2D2 - move the peice on A2 to D2
            
            Short move:
            
            First 6 bits: starting square in binary.
            Second 6 bits: ending square in binary.
            Last 4 bits (optional, not implimented, only used for undo-move in minimax): castle flag
                Each bit is set to 0 if castling rights are unchanged, set to 1 is castling rights are lost
                Bit 1 = white long castle, Bit 2 = white short castle, Bit 3 = black long castle, Bit 4 = black short castle
        */

        int SquareToIndex(string square)
        {
            int file = square[0] - 'A'; // 'A' becomes 0, 'B' becomes 1, ..., 'H' becomes 7
            int rank = square[1] - '1'; // '1' becomes 0, '2' becomes 1, ..., '8' becomes 7
            return rank * 8 + file;
        }

        // Parse the input move
        string startSquare = move.Substring(0, 2);
        string endSquare = move.Substring(2, 2);

        // Convert squares to indices
        int startIndex = SquareToIndex(startSquare);
        int endIndex = SquareToIndex(endSquare);
        Console.WriteLine(startIndex);
        Console.WriteLine(endIndex);

        // Pack the indices into a ushort
        // Left-shift the start index by 6 to make room for the end index
        ushort packedMove = (ushort)(((startIndex << 6) | endIndex) << 4);

        return packedMove;

    }

    public static string ShortToStringMove(ushort packedMove)
    {
        // Unpack the move
        // We first need to shift right by 4 bits to ignore the unused last 4 bits
        // Then extract the start index (first 6 bits) and the end index (next 6 bits)
        int startIndex = (packedMove >> 10) & 0x3F; // Extract first 6 bits
        int endIndex = (packedMove >> 4) & 0x3F; // Extract next 6 bits

        // Convert index to square
        string IndexToSquare(int index)
        {
            int file = index % 8; // Column index
            int rank = index / 8; // Row index
            char fileChar = (char)('A' + file); // Convert 0-7 to 'A'-'H'
            char rankChar = (char)('1' + rank); // Convert 0-7 to '1'-'8'
            return fileChar.ToString() + rankChar.ToString();
        }

        // Convert indices to squares
        string startSquare = IndexToSquare(startIndex);
        string endSquare = IndexToSquare(endIndex);

        return startSquare + endSquare; // Concatenate the start and end square to form the move string
    }


    public static ushort IndexsToShortMove(int startIndex, int endIndex, int castleFlag)
    {
        return (ushort)((((startIndex << 6) | endIndex) << 4) | castleFlag);
    }









}