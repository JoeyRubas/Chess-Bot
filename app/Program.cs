using gameObjects;

Console.WriteLine("Hello, World!");


Board board = new Board();


while (true)
{
    board.renderBoard();
    HashSet<ushort> legalMoves = board.GetLegalMoves(false);
    foreach (ushort eachMove in legalMoves)
    {
        Console.WriteLine(utility.ShortToStringMove(eachMove));
    }
    string move = Console.ReadLine();
    board.makeMove(move);

}
