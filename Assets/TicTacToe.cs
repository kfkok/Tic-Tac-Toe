using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Threading;

public class TicTacToe : MonoBehaviour
{
    State state;

    // Start is called before the first frame update
    void Start()
    {
        state = new State();
        InitializeSquares();
    }

    public State Reset()
    {
        state.done = false;
        state.winningSquares.Clear();
        ClearSquares();

        return state;
    }

    protected void InitializeSquares()
    {
        List<Square> squareList = new List<Square>();

        // Add the all the small squares
        foreach (Transform child in transform)
        {
            Square square = child.GetComponent<Square>();

            if (square != null)
            {
                squareList.Add(square);    
            }
        }

        squareList = (squareList.OrderBy(x => x.name)).ToList<Square>();

        int k = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                state.squares[i, j] = squareList[k];
                state.squares[i, j].Position = new Tuple<int, int>(i, j);
                k++;
            }
        }
    }

    protected void ClearSquares()
    {
        foreach (Square square in state.squares)
        {
            square.Clear();
        }
    }

    protected Player GetWinner()
    {
        // Check each row
        for (int row = 0; row < 3; row++)
        {
            if (((state.squares[row, 0].player != null && state.squares[row, 1].player != null) && state.squares[row, 2].player != null) &&
                (state.squares[row, 0].player.id == state.squares[row, 1].player.id) && (state.squares[row, 1].player.id == state.squares[row, 2].player.id))
            {
                state.winningSquares = new List<Square>(){ state.squares[row, 0], state.squares[row, 1], state.squares[row, 2] };
                return state.squares[row, 0].player;
            }
        }

        // Check each column
        for (int column = 0; column < 3; column++)
        {
            if (((state.squares[0, column].player != null && state.squares[1, column].player != null) && state.squares[2, column].player != null) &&
                (state.squares[0, column].player.id == state.squares[1, column].player.id) && (state.squares[1, column].player.id == state.squares[2, column].player.id))
            {
                state.winningSquares = new List<Square>() { state.squares[0, column], state.squares[1, column], state.squares[2, column] };
                return state.squares[0, column].player;
            }
        }

        // Check left to right diagonal
        if (((state.squares[0, 0].player != null && state.squares[1, 1].player != null) && state.squares[2, 2].player != null) &&
            (state.squares[0, 0].player.id == state.squares[1, 1].player.id) && (state.squares[1, 1].player.id == state.squares[2, 2].player.id))
        {
            state.winningSquares = new List<Square>() { state.squares[0, 0], state.squares[1, 1], state.squares[2, 2] };
            return state.squares[0, 0].player;
        }

        // Check right to left diagonal
        if (((state.squares[0, 2].player != null && state.squares[1, 1].player != null) && state.squares[2, 0].player != null) &&
            ((state.squares[0, 2].player.id == state.squares[1, 1].player.id) && state.squares[1, 1].player.id == state.squares[2, 0].player.id)) 
        {
            state.winningSquares = new List<Square>() { state.squares[0, 2], state.squares[1, 1], state.squares[2, 0] };
            return state.squares[0, 2].player;
        }

        return null;
    }

    public State Step(Player player, Tuple<int, int> position)
    {
        state.lastMoveIsValid = AssignPlayerToSquare(player, position);

        if (state.lastMoveIsValid)
        {
            // Game won by current player
            Player winner = GetWinner();
            if (winner != null)
            {
                state.reward = 1f;
                state.done = true;
            }
            // Game draw
            else if (Draw())
            {
                state.reward = 0f;
                state.done = true;
            }
            else
            {
                state.reward = 0f;
                state.done = false;
            }
        }

        return state;
    }

    protected bool AssignPlayerToSquare(Player player, Tuple<int, int> position)
    {
        // Item 1 = row, Item 2 = column
        Square square = state.squares[position.Item1, position.Item2];

        // The position is already occupied by a player
        if (square.player != null)
        {
            return false;
        }
       
        MarkSquare(square, player);

        return true;
    }

    protected void MarkSquare(Square square, Player player)
    {
        // Assign player to the square
        square.player = player;

        // Visual changes indicating the player has occupied the square, eg by marking or changing color
        GameObject symbol = Instantiate(player.symbol);
        symbol.transform.position = square.transform.position + new Vector3(0f, 0f, -0.1f);
        symbol.transform.parent = this.transform;
        symbol.GetComponent<TextMesh>().color = player.color;

        square.symbol = symbol;
    }

    protected bool Draw()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                if (state.squares[row, column].player == null)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

// The board state returned by Tic tac toe after each step
public class State
{
    public Square[,] squares;
    public float reward;
    public bool done;
    public bool lastMoveIsValid;
    public List<Square> winningSquares;

    public State(float reward = 0, bool done = false, bool lastMoveIsValid = false)
    {
        this.squares = new Square[3, 3];
        this.reward = reward;
        this.done = done;
        this.lastMoveIsValid = lastMoveIsValid;
        winningSquares = new List<Square>();
    }

}
