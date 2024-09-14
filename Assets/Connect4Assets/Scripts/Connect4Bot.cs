using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect4Bot : MonoBehaviour
{

    public int DEPTH = 8;

    public int DEPTH_LIMIT = 15; // Limit the depth of the search (increments 1 every turn)
    private BoardState boardState;

    private char p1;
    private char p2;

    private bool firstTurn;

    void Start()
    {
        boardState = GetComponent<BoardState>();
        p1 = boardState.p1;
        p2 = boardState.p2;
        firstTurn = true;
    }
    // Return the board as a string for caching
    private string BoardToString(char[,] board)
    {
        string boardString = "";
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                boardString += board[i, j];
            }
        }
        return boardString;
    }

    // Calc score based off of count and emptyCount
    private int Score(int count, int emptyCount)
    {
        if (count == 4)
        {
            return 100;
        }
        else if (count == 3 && emptyCount == 1)
        {
            return 10;
        }
        else if (count == 2 && emptyCount == 2)
        {
            return 5;
        }
        else if (count == -4)
        {
            return -100;
        }
        else if (count == -3 && emptyCount == 1)
        {
            return -10;
        }
        else if (count == -2 && emptyCount == 2)
        {
            return -5;
        }
        return 0;
    }

    // Evaluate the board based on the current player
    private int EvaluateBoard()
    {
        char AI = boardState.currentPlayer;
        char player = AI == p1 ? p2 : p1;
        char[,] board = boardState.board;
        int score = 0;

        //Debug.Log($"AI: {AI}, Player: {player}");

        //boardState.DebugPrintBoard(board);
        
        int count = 0; 
        int emptyCount = 0;

        // Evaluate rows
        for (int row=0; row<6; row++){
            for (int col = 0; col<4; col++){
                count = 0;
                emptyCount = 0;
                for (int i=0; i<4; i++){
                    if (board[row, col+i] == AI){
                        count += 1;
                    } else if (board[row, col+i] == ' '){
                        emptyCount += 1;
                    } else {
                        count--;
                    }
                }
                score += Score(count, emptyCount);
            }
        }
        //Debug.Log($"score1: {score}");
        // Evaluate columns
        for (int col=0; col<7; col++){
            for (int row = 0; row<3; row++){
                count = 0;
                emptyCount = 0;
                for (int i=0; i<4; i++){
                    if (board[row+i, col] == AI){
                        count += 1;
                    } else if (board[row+i, col] == ' '){
                        emptyCount += 1;
                    } else {
                        count--;
                    }
                }
                score += Score(count, emptyCount);
            }
        }
        //Debug.Log($"score2: {score}");

        // Evaluate diagonals (negative slope)
        for (int row=0; row<3; row++){
            for (int col = 0; col<4; col++){
                count = 0;
                emptyCount = 0;
                for (int i=0; i<4; i++){
                    if (board[row+i, col+i] == AI){
                        count += 1;
                    } else if (board[row+i, col+i] == ' '){
                        emptyCount += 1;
                    } else {
                        count--;
                    }
                }
                score += Score(count, emptyCount);
            }
        }
        //Debug.Log($"score3: {score}");

        // Evaluate diagonals (positive slope)
        for (int row=0; row<3; row++){
            for (int col = 3; col<7; col++){
                count = 0;
                emptyCount = 0;
                for (int i=0; i<4; i++){
                    if (board[row+i, col-i] == AI){
                        count += 1;
                    } else if (board[row+i, col-i] == ' '){
                        emptyCount += 1;
                    } else {
                        count--;
                    }
                }
                score += Score(count, emptyCount);
            }
        }
        //Debug.Log($"score4: {score}");

        //Add score for playing in the middle section
        for (int row=0; row<6; row++){
            for (int col = 2; col<5; col++){
                if (board[row, col] == AI){
                    score += 2 - Mathf.Abs(3 - col);
                    //Debug.Log($"score middle hhhhhhhhhhhhhh: {score}");
                } else if (board[row, col] == player){
                    score -= 2 - Mathf.Abs(3 - col);
                }
            }
        }
        //Debug.Log($"score5: {score}");

        return score;
    }

    // Dictionary to store the cache
    private Dictionary<string, int> cache = new Dictionary<string, int>();

    // Minimax function
    // Returns the score of the board based off a certain depth
    int Minimax(char[,] board, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        char AI = boardState.currentPlayer;
        char player = AI == p1 ? p2 : p1;

        if (depth == 0 || boardState.CheckWin(maximizingPlayer) || boardState.IsFull())
        {
            return EvaluateBoard();
        }

        string cacheKey = $"{BoardToString(board)}";
        if (cache.ContainsKey(cacheKey))
        {
            //Debug.Log("Cache hit");
            return cache[cacheKey];
        }

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            int?[,] moves = boardState.AllCurrentPossibleMoves();
            for (int i = 0; i < 7; i++)
            {
                if (moves[i, 0] != null)
                {
                    board[(int)moves[i, 1], (int)moves[i, 0]] = AI;
                    int eval = Minimax(board ,depth - 1, alpha, beta, false);
                    board[(int)moves[i, 1], (int)moves[i, 0]] = ' ';
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }

            cache[cacheKey] = maxEval;
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            int?[,] moves = boardState.AllCurrentPossibleMoves();
            for (int i = 0; i < 7; i++)
            {
                if (moves[i, 0] != null)
                {
                    board[(int)moves[i, 1], (int)moves[i, 0]] = player;
                    int eval = Minimax(board, depth - 1, alpha, beta, true);
                    board[(int)moves[i, 1], (int)moves[i, 0]] = ' ';
                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }

            cache[cacheKey] = minEval;
            return minEval;
        }
    }


    // First function of the AI call chain
    // Drops a piece for the AI after calling the minmax function
    public int DropPieceAI(char player)
    {
        int bestMove = -1;
        int bestScore = int.MinValue; // Negative infinity
        int?[,] moves = boardState.AllCurrentPossibleMoves();
        char[,] board = boardState.board;
        int score;
        if (firstTurn && player == p1)
        {
            firstTurn = false;
            return 3; // Drop in the middle for the first turn
            // Middle is theoretically the best move for the first turn
        }
        for (int i = 0; i < 7; i++)
        {
            if (moves[i, 0] != null)
            {
                board[(int)moves[i, 1], (int)moves[i, 0]] = player;
                score = Minimax(board, DEPTH, int.MinValue, int.MaxValue, false);
                //Debug.Log($"Move: {i}, Score: {score}");
                board[(int)moves[i, 1], (int)moves[i, 0]] = ' ';
                if (score > bestScore)
                {
                    
                    bestScore = score;
                    bestMove = (int)moves[i, 0];
                }
            }
        }
        if (DEPTH <= DEPTH_LIMIT){
            DEPTH += 1; // Increase the depth of the search as game progresses
        }
        return bestMove;
    }
}
