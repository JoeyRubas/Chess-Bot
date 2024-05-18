
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
                Console.Write(outchar);
            }
            Console.Write("\n");
        }
    }


}