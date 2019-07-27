using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public TicTacToe env;
    public Text text;
    public int episodes;
    public enum Mode
    {
        Play,
        Train
    }
    public Mode mode;
    public StrikeLine strikeLine;
    public Player player1; // player 1 can be human or ai player
    public AiPlayer player2; // player 2 is ai player
    public StateActionValues stateActionValue;
    public Text humanWin;
    public Text aiWin;
    public Text draw;

    protected Player currentPlayer;
    protected Player previousPlayer;
    int humanWinCount;
    int aiWinCount;
    int drawCount;

    private Tuple<int, int> selectedSquarePosition;
    private State state;

    void Start()
    {
        humanWinCount = 0;
        aiWinCount = 0;
        drawCount = 0;

        StartCoroutine(Run());
    }


    IEnumerator Run()
    {
        // Wait the ai brain to finish loading, (file could be huge!)
        yield return new WaitUntil(stateActionValue.Ready);

        // Game can begin now
        for (int episode = 0; episode < episodes; episode++)
        {
            //print("episode:" + episode);
            state = env.Reset();

            while (!state.done)
            {
                SetCurrentPlayer();

                // The game is waiting for the player to select a square
                yield return currentPlayer.SelectingSquare(state);

                // The player has selected a square, get the selected square
                selectedSquarePosition = currentPlayer.GetSelectedSquarePosition();

                yield return Step(selectedSquarePosition);
            }

            // Save state action values to file upon certain episodes reached
            if (episode != 0 && (episode % 5 == 0 && mode == Mode.Play || episode == episodes - 1 || episode % 500 == 0 && mode == Mode.Train))
            {
                stateActionValue.SaveStateActionValues();
            }
        }

        SetText("Game ended");

        yield return null;
    }

    IEnumerator Step(Tuple<int, int> position)
    {
        state = env.Step(currentPlayer, position);

        if (state.done)
        {
            // game won by current player
            if (state.reward > 0)
            {
                // winning player, state reward = +1                
                yield return currentPlayer.SelectingSquare(state);

                // losing player, state reward = -1
                state.reward = -1;
                yield return previousPlayer.SelectingSquare(state);

                if (mode == Mode.Play)
                {
                    SetText(currentPlayer.playerName + " won");
                    var winningSquares = state.winningSquares;
                    yield return strikeLine.DrawLine(winningSquares.First().transform.position, winningSquares.Last().transform.position);
                    yield return new WaitForSeconds(0.3f);
                    strikeLine.ClearLine();

                    if (currentPlayer is HumanPlayer)
                        humanWin.text = (++humanWinCount).ToString();
                    else
                        aiWin.text = (++aiWinCount).ToString();
                }
            }
            else
            {
                if (mode == Mode.Play)
                {
                    SetText("draw");
                    yield return new WaitForSeconds(0.8f);
                    draw.text = (++drawCount).ToString();
                }
            }
        }
    }

    protected void SetCurrentPlayer()
    {
        if (currentPlayer == player1)
        {
            currentPlayer = player2;
            previousPlayer = player1;
        }
        else
        {
            currentPlayer = player1;
            previousPlayer = player2;
        }

        if (mode == Mode.Play)
            SetText(currentPlayer.playerName + " turn");
    }

    protected void SetText(string message)
    {
        text.text = message;
        print(message);
    }

}

