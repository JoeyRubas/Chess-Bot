using System;
using System.Numerics;
using System.Collections.Generic;
using gameObjects;

namespace chessBotV1
{
    public static class chessBotV1
    {
        private static int Evaluation(ulong[] bitboard)
        {
            int eval = 0;
            int[] value = { 1, 3, 3, 5, 9, 10000, -1, -3, -3, -5, -9, 10000 };
            for (int i = 0; i < 12; i++)
            {
                eval += value[i] * BitOperations.PopCount(bitboard[i]);
            }
            return eval;
        }

        public static int MinimaxWithAlphaBeta(Board board, int depth, bool maximizingPlayer, int alpha, int beta, out Move bestMove)
        {
            bestMove = null;
            if (depth == 0)
            {
                return Evaluation(board.bitboard);
            }

            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (Move move in board.GetLegalMoves())
                {
                    if (depth == 2) Console.WriteLine("Depth: " + depth + " attempting move: " + move.getString());
                    //board.renderBoard();
                    board.MakeMove(move);
                    int eval = MinimaxWithAlphaBeta(board, depth - 1, false, alpha, beta, out _);
                    //Console.WriteLine("Depth: " + depth + " reversing move: " + move.getString());
                    board.UnmakeMove(move);
                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        bestMove = move;
                    }
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                        break;
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (Move move in board.GetLegalMoves())
                {
                    if (depth == 2) Console.WriteLine("Depth: " + depth + " attempting move: " + move.getString());
                    //board.renderBoard();
                    board.MakeMove(move);
                    int eval = MinimaxWithAlphaBeta(board, depth - 1, true, alpha, beta, out _);
                    //Console.WriteLine("Depth: " + depth + " reversing move: " + move.getString());
                    board.UnmakeMove(move);

                    if (eval < minEval)
                    {
                        minEval = eval;
                        bestMove = move;
                    }
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                        break;
                }
                return minEval;
            }
        }
    }
}
