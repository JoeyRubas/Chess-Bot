using System;
using System.Drawing;
using System.Formats.Tar;
using System.Windows.Forms;

namespace gameObjects;

public class ChessBoardForm : Form
{
    Board board;
    string[] ImageNames = { "Images/WPawn.svg",
                            "Images/WBishop.svg",
                            "Images/WKnight.svg",
                            "Images/WRook.svg",
                            "Images/WQueen.svg",
                            "Images/WKing.svg",
                            "Images/BPawn.svg",
                            "Images/BBishop.svg",
                            "Images/BKnight.svg",
                            "Images/BRook.svg",
                            "Images/BQueen.svg",
                            "Images/BKing.svg",};
    public ChessBoardForm(Board inBoard)
    {
        this.ClientSize = new Size(800, 800);
        this.Text = "Chess Board";
        board = inBoard;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        DrawBoard(e.Graphics);
        DrawPieces(e.Graphics);
    }

    private void DrawBoard(Graphics graphics)
    {
        int tileSize = 100;
        bool white = true;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                using (Brush brush = new SolidBrush(white ? Color.White : Color.Gray))
                {
                    int x = j * tileSize;
                    int y = i * tileSize;
                    graphics.FillRectangle(brush, x, y, tileSize, tileSize);
                    white = !white;
                }
            }
            white = !white;  // switch the starting color every row
        }
    }

    private void DrawPieces(Graphics graphics)
    {
        ulong mask = 1UL;
        for (int i = 0; i < 64; i++)
        {
            for (int k = 0; k < 12; k++)
            {
                if ((mask & board.bitboard[k]) > 0)
                {
                    int y = (i / 8) * ClientSize.Height;
                    int x = i % 8 * ClientSize.Width;
                    Image img = Image.FromFile("C:/Users/kingc/OneDrive/Documents/Programming/Chess-Bot/app/" + ImageNames[k]);
                    Point pt = new Point(x, y);
                    graphics.DrawImage(img, pt);
                    break;
                }
            }
        }
    }
}
