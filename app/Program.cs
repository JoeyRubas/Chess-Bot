using System;
using gameObjects;
using chessBotV1;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, welcome to Chess!");

        Board board = new Board();

        while (true)
        {
            // Render the board and ask for the user's move
            board.renderBoard();
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
                    board.renderBoard();
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

            }

            // Check again for game end conditions here
        }
    }
}
