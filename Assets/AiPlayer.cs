using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiPlayer : Player
{
    public float DecisionDelay;
    public StateActionValues stateActionValues;
    public float learningRate;
    public float epsilon;

    int[,] percievedSquares;
    List<Square> emptySquares;
    float defaultActionValue;
    System.Random random;
    Tuple<int[,], Tuple<int, int>> previousStateAction;

    void Start()
    {
        random = new System.Random();
        defaultActionValue = 0.5f;
        previousStateAction = null;
    }

    private void Reset()
    {
        epsilon = 0.2f;
        learningRate = 0.05f;
    }

    public override IEnumerator SelectingSquare(State state)
    {
        // Perceiving the state will set the percieved squares and empty squares
        PercieveState(state);

        // Game not done, select a square
        if (!state.done)
        {
            yield return new WaitUntil(SquareSelected);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.0f, 0.5f)); // here delay some time before next line, optional for computer player
        }

        // Game over, reflex upon last state-action 
        else
        {
            UpdatePreviousStateActionValue(state.reward);

            previousStateAction = null;

            yield return null;
        }
    }

    /// <summary>
    /// Extract important information from the state
    /// </summary>
    /// <param name="state"></param>
    protected void PercieveState(State state)
    {
        percievedSquares = new int[3,3];
        emptySquares = new List<Square>();

        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                Square square = state.squares[row, column];

                // Square occupied by opponent player, assign 2
                if (square.player != null && square.player.id != id)
                {
                    percievedSquares[row, column] = 2;
                }

                // Square occupied by self, assign 1
                else if (square.player != null && square.player.id == id)
                {
                    percievedSquares[row, column] = 1;
                }

                // Empty square
                else
                {
                    emptySquares.Add(square);
                    percievedSquares[row, column] = 0;
                } 
            }
        }
    }

    public override Tuple<int, int> GetSelectedSquarePosition()
    {
        return selectedSquarePosition;
    }

    protected override bool SquareSelected()
    {
        // Let's retrieve the action values for this state, if it doesnt exist, initialize them using the empty squares available now
        Dictionary<Tuple<int, int>, float> actionValues = GetOrInitializeActionValues(percievedSquares);

        // Reduce exploration rate
        //epsilon *= 0.99f;

        // E-greedy (Select best position)
        double rand = random.NextDouble();
        if (rand > epsilon)
        {
            // Select the action(position) which gives the maximum value
            selectedSquarePosition = actionValues.Aggregate((x, y) => x.Value > y.Value ? x : y).Key; // x is accumulation, in this case it maintains the largest key

            // Update previous state action value (backup)
            float value = actionValues[selectedSquarePosition];
            UpdatePreviousStateActionValue(value);
        }

        // Explore
        else
        {
            selectedSquarePosition = SelectRandomAction(actionValues);

            //var best = actionValues.Aggregate((x, y) => x.Value > y.Value ? x : y).Key; // x is accumulation, in this case it maintains the largest key
            //print("exploring, best action:" + best + ", but gonna explore:" + selectedSquarePosition);
        }

        previousStateAction = new Tuple<int[,], Tuple<int, int>> (percievedSquares, selectedSquarePosition);

        return true;
    }

    protected void UpdatePreviousStateActionValue(float target)
    {
        if (previousStateAction != null)
        {
            int[,] previousState = previousStateAction.Item1;
            Tuple<int, int> previousAction = previousStateAction.Item2;

            Dictionary<Tuple<int, int>, float> actionValues = stateActionValues.Retrieve(previousState);
            float oldValue = actionValues[previousAction];
            float newValue = oldValue + learningRate * (target - oldValue);
            actionValues[previousAction] = newValue;

            //print("Updated previous action " + previousAction + " from " + oldValue + " to " + newValue);
        }
    }

    protected Dictionary<Tuple<int, int>, float> GetOrInitializeActionValues(int[,] percievedSquares)
    {
        // At this point, the squares are percieved, meaning both percievedSquares and emptySquares are not null 

        Dictionary<Tuple<int, int>, float> actionValues = stateActionValues.Retrieve(percievedSquares);

        if (actionValues != null)
        {
            return actionValues;
        }
        else
        {
            actionValues = new Dictionary<Tuple<int, int>, float>();

            foreach (Square emptySquare in emptySquares)
            {
                var position = emptySquare.Position;
                Tuple<int, int> action = new Tuple<int, int>(position.Item1, position.Item2);

                actionValues.Add(action, defaultActionValue);
            }

            // Add it to the state action values dictionary
            stateActionValues.Set(percievedSquares, actionValues);

            return actionValues;
        }
    }

    protected Tuple<int, int> SelectRandomAction(Dictionary<Tuple<int, int>, float> actionValues)
    {
        int randomIndex = UnityEngine.Random.Range(0, actionValues.Count);

        return actionValues.ElementAt(randomIndex).Key;
    }
}
