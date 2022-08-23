using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberSquare : MonoBehaviour
{
    public List<Transform> currentTouchingObjects = new List<Transform>();
    public int notation;
    public CheckForBingo checkForBingo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print(transform.name + ": Hi");
        currentTouchingObjects.Add(collision.transform);
        if (collision.CompareTag("Token"))
        {
            //print("Ayy that's a token right there");
            checkForBingo.UpdateTokenPlacement(notation, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //print(transform.name + ": Bye");
        currentTouchingObjects.Remove(collision.transform);
        if (collision.CompareTag("Token"))
        {
            //print("Ayy that's a token right there");
            checkForBingo.UpdateTokenPlacement(notation, false);
        }
    }

    public bool IsSomethingStandingOn()
    {
        return currentTouchingObjects.Count > 0;
    }
}
