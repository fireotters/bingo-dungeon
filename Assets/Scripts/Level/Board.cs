using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Board : MonoBehaviour
//{
//    [SerializeField] private Transform squareParent;
//    private List<Square> allSquares = new List<Square>();

//    private void Awake()
//    {
//        foreach (Transform row in squareParent)
//        {
//            foreach (Transform square in row)
//            {
//                allSquares.Add(new Square(square));
//            }
//        }
//    }

//    public void UpdatePieceProtections(List<Vector3> listOfProtectedSquares)
//    {
//        foreach (Vector3 protectedSquare in listOfProtectedSquares)
//        {
//            foreach (Square square in allSquares)
//            {
//                if (square.location == protectedSquare)
//                {
//                    square.debugSprite.color = Color.red;
//                }
//            }
//        }
//    }
//}

//public class Square
//{
//    public string notation;
//    public Vector3 location;
//    public bool protectedByEnemy = false;
//    public SpriteRenderer debugSprite;

//    public Square(Transform square)
//    {
//        notation = square.parent.name + square.name;
//        location = square.position;
//        debugSprite = square.GetComponent<SpriteRenderer>();
//        Debug.Log("Created a square with notation '" + notation + "'.");
//    }
//}