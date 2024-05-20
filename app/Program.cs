using System;
using gameObjects;
using chessBotV1;
using System.Windows.Forms; // Required for Windows Forms application

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, welcome to Chess!");

        Board board = new Board();
        bool UserVBot = true;
        bool BotVBot = true;

        // Initialize and run the ChessBoardForm
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        ChessBoardForm chessBoard = new ChessBoardForm(board); // Pass the board instance to the form
        Application.Run(chessBoard);

        if (UserVBot)
        {
            while (true)
            {
                // Render the board (this now happens inside the ChessBoardForm)
                // board.renderBoard(); (This line is commented out because rendering is handled by the form)

                HashSet<Move> legalMoves = board.GetLegalMoves();
                Console.WriteLine("Available moves:");
                foreach (Move eachMove in legalMoves)
                {
                    Console.WriteLine(eachMove.getString());  // Assuming utility.ShortToStringMove exists to format the move display
                }

                Move userMove = null;
                bool validMove = false;
                while (!validMove)
                {
                    Console.Write("Enter your move: ");
                    string moveInput = Console.ReadLine().Substring(0, 4);

                    foreach (Move eachMove in legalMoves)
                    {
                        if (eachMove.getString() == moveInput)
                        {
                            userMove = eachMove;
                        }
                    }

                    if (legalMoves.Contains(userMove))
                    {
                        validMove = true;
                        board.MakeMove(userMove);
                        // board.renderBoard(); (This line is commented out because rendering is handled by the form)
                        chessBoard.Refresh(); // Refresh the form to update the board view
                    }
                    else
                    {
                        Console.WriteLine("Invalid move, please try again.");
                    }
                }

                // Check for game end conditions here (e.g., checkmate, stalemate)

                // Bot makes its move
                if (!board.IsGameOver())
                {
                    Console.WriteLine("Bot is thinking...");
                    Move botMove = null;
                    chessBotV1.chessBotV1.MinimaxWithAlphaBeta(board, 7, false, int.MinValue, int.MaxValue, out botMove); // Depth set to 3 for example
                    botMove.DebugPrint();
                    Console.WriteLine($"Bot played: {botMove.getString()}");
                    board.MakeMove(botMove);
                    chessBoard.Refresh(); // Refresh the form to update the board view
                }
            }
            // Check again for game end conditions here
        }
    }
}
