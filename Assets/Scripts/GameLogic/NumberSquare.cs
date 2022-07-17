using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberSquare : MonoBehaviour
{
    private Collider2D _collider;
    public List<Transform> currentTouchingObjects = new List<Transform>();

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print(transform.name + ": Hi");
        currentTouchingObjects.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //print(transform.name + ": Bye");
        currentTouchingObjects.Remove(collision.transform);
    }

    public bool IsSomethingStandingOn()
    {
        return currentTouchingObjects.Count > 0;
    }
}
