namespace gameObjects;

public static class utility
{

    public static int SquareToIndex(string square)
    {
        int file = square[0] - 'A'; // 'A' becomes 0, 'B' becomes 1, ..., 'H' becomes 7
        int rank = square[1] - '1'; // '1' becomes 0, '2' becomes 1, ..., '8' becomes 7
        return rank * 8 + file;
    }

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

        // Parse the input move
        string startSquare = move.Substring(0, 2);
        string endSquare = move.Substring(2, 2);

        // Convert squares to indices
        int startIndex = SquareToIndex(startSquare);
        int endIndex = SquareToIndex(endSquare);

        // Pack the indices into a ushort
        // Left-shift the start index by 6 to make room for the end index
        ushort packedMove = (ushort)((startIndex << 6) | endIndex);

        return packedMove;

    }

    public static string ShortMoveToString(ushort packedMove)
    {
        // Converts a 0-63 index to a board square in algebraic notation
        string IndexToSquare(int index)
        {
            int file = index % 8; // Remainder when divided by 8 gives file
            int rank = index / 8; // Division by 8 gives rank
            char fileChar = (char)('A' + file); // Convert file to character from 'A' to 'H'
            char rankChar = (char)('1' + rank); // Convert rank to character from '1' to '8'
            return fileChar.ToString() + rankChar.ToString();
        }

        // Extract indices from packed move
        int startIndex = packedMove >> 6; // Right-shift 6 bits to get the upper 6 bits
        int endIndex = packedMove & 0x3F; // Apply mask 0x3F (00111111) to get the lower 6 bits

        // Convert indices to algebraic notation
        string startSquare = IndexToSquare(startIndex);
        string endSquare = IndexToSquare(endIndex);

        // Combine start and end squares into move string
        return startSquare + endSquare;
    }



}