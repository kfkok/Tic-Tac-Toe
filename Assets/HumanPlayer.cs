using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    public override IEnumerator SelectingSquare(State state)
    {
        // Game not done, select a square
        if (!state.done)
        {
            yield return new WaitUntil(SquareSelected);
            yield return new WaitForSeconds(0.001f); // here delay some time before next line, needed for human player
        }

        // Game over, no need select any square
        else
        {
            yield return null;
        }
    }

    public override Tuple<int, int> GetSelectedSquarePosition()
    {
        return selectedSquarePosition;
    }

    protected override bool SquareSelected()
    {
        selectedSquarePosition = null;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                GameObject gameObject = hit.collider.gameObject;
                //Debug.Log(gameObject.name);

                Square square = gameObject.GetComponent<Square>();
                selectedSquarePosition = square.Position;

                // Only proceed if the square is empty, e.g not selected before
                if (square.player == null)
                {
                    return true;
                }

                print("Occupied !!");
            }
        }

        return false;
    }
}
