using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberSquare : MonoBehaviour
{
    public List<Transform> currentTouchingObjects = new List<Transform>();
    public int notation;
    public CheckForBingo checkForBingo;


    // Exiting tokens will immediately update their placement, but entering tokens are delayed, to prevent false wins.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentTouchingObjects.Add(collision.transform);
        if (collision.CompareTag("Token"))
            Invoke(nameof(DelayUpdateTokenPlacement), 1f);
    }

    private void DelayUpdateTokenPlacement()
    {
        checkForBingo.UpdateTokenPlacement(notation, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentTouchingObjects.Remove(collision.transform);
        if (collision.CompareTag("Token"))
            checkForBingo.UpdateTokenPlacement(notation, false);
    }

    public bool IsSomethingStandingOn()
    {
        return currentTouchingObjects.Count > 0;
    }
}
