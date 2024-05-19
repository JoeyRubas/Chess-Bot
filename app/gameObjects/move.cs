using System.Runtime.InteropServices;

namespace gameObjects;


public class Move
{
    public int pieceType;
    public int startSquare { get; }
    public int endSquare { get; }
    public int captureIndex;
    public int captureType;
    public int promotionPieceType;
    public ulong enPassantLost;
    public ulong enPassantGained;
    public byte castling;
    //Bit 1 - white long castle
    //bit 2 - white short castle
    //bit 3 - black long castle
    //bit 4 - black short castle
    //bit 5 - white loses long castle rights
    //bit 6 - white loses short castle rights
    //bit 7 - black loses long castle rights
    //bit 8 - black loses short castle rights
    public string debugData;
    public Move(int StartSquare, int EndSquare, int PieceType)
    {
        pieceType = PieceType;
        startSquare = StartSquare;
        endSquare = EndSquare;
        captureIndex = -1;
        captureType = -1;
        promotionPieceType = -1;
        enPassantLost = 0;
        castling = 0;
    }


    public Move(byte Castling)
    {
        startSquare = -1;
        endSquare = -1;
        captureIndex = -1;
        captureType = -1;
        promotionPieceType = -1;
        enPassantLost = 0;
        castling = Castling;
    }

    public String getString()
    {
        string IndexToSquare(int index)
        {
            int file = index % 8; // Column index
            int rank = index / 8; // Row index
            char fileChar = (char)('A' + file); // Convert 0-7 to 'A'-'H'
            char rankChar = (char)('1' + rank); // Convert 0-7 to '1'-'8'
            return fileChar.ToString() + rankChar.ToString();
        }

        // Convert indices to squares
        string startSquareString = IndexToSquare(startSquare);
        string endSquareString = IndexToSquare(endSquare);

        return startSquareString + endSquareString;
    }
    public void DebugPrint()
    {
        Console.WriteLine($"Piece Type: {pieceType}");
        Console.WriteLine($"Start Square: {startSquare}");
        Console.WriteLine($"End Square: {endSquare}");
        Console.WriteLine($"Capture Index: {captureIndex}");
        Console.WriteLine($"Capture Type: {captureType}");
        Console.WriteLine($"Promotion Piece Type: {promotionPieceType}");
        Console.WriteLine($"En Passant Lost: {enPassantLost}");
        Console.WriteLine($"En Passant Gained: {enPassantGained}");
        Console.WriteLine($"Castling: {Convert.ToString(castling, 2).PadLeft(8, '0')} (binary)");
        Console.WriteLine($"Debug Data: {debugData}");
    }

}