using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BoardState : MonoBehaviour
{
    public char[,] board = new char[6, 7]; 

    public char p1 = 'X';

    public char p2 = 'O';

    public char currentPlayer = 'X'; 

    void Start()
    {
        ClearBoard();
    }

    public void ClearBoard()
    {
        
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                board[i, j] = ' ';
            }
        }
    }

    // Check if anyone has won
    public bool CheckWin(bool maximizingPlayer)
    {
        char player = maximizingPlayer ? p2 : p1;
        // Check rows

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (board[i, j] == player && board[i, j + 1] == player && board[i, j + 2] == player && board[i, j + 3] == player)
                {
                    return true;
                }
            }
        }

        // Check columns
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[j, i] == player && board[j + 1, i] == player && board[j + 2, i] == player && board[j + 3, i] == player)
                {
                    return true;
                }
            }
        }

        // Check diagonals (negative slope)
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (board[i, j] == player && board[i + 1, j + 1] == player && board[i + 2, j + 2] == player && board[i + 3, j + 3] == player)
                {
                    return true;
                }
            }
        }

        // Check diagonals (positive slope)
        for (int i = 0; i < 3; i++)
        {
            for (int j = 3; j < 7; j++)
            {
                if (board[i, j] == player && board[i + 1, j - 1] == player && board[i + 2, j - 2] == player && board[i + 3, j - 3] == player)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Returns 2d array of all possible moves
    public int?[,] AllCurrentPossibleMoves()
    {
        // Make moves intialized with null
        int?[,] moves = new int?[7, 2];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 5; j > -1; j--)
            {
                if (board[j, i] == ' ')
                {
                    moves[i, 0] = i;
                    moves[i, 1] = j;
                    break;
                }
            }
        }
        
        return moves;
    }

    public void DropPiece(int col, char player)
    {
        for (int i = 5; i > -1; i--)
        {
            if (board[i, col] == ' ')
            {
                board[i, col] = player;
                break;
            }
        }
        currentPlayer = player == p1 ? p2 : p1;
    }

    public bool IsFull()
    {
        for (int i = 0; i < 7; i++)
        {
            if (board[0, i] == ' ')
            {
                return false;
            }
        }
        return true;
    }

    public void DebugPrintBoard(char[,] board2)
    {
        for (int j = 5; j >= 0; j--)
        {
            for (int i = 0; i < 7; i++)
            {
                if (board2[j, i] == ' ')
                {
                    Debug.Log("0");
                }
                else
                {
                    Debug.Log(board2[j, i]);
                }
            }
            Debug.Log("\n");
        }
    }

}
