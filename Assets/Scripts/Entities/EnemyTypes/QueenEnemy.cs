using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class QueenEnemy : BasicEnemy
    {
        public override Vector3 ChoosePosition()
        {
            List<Vector3> validMoves = new List<Vector3>();
            Vector3[] directionsToMove = {
                new Vector3(0, 1), new Vector3(0, -1), new Vector3(1, 0), new Vector3(-1, 0),
                new Vector3(1, 1), new Vector3(1, -1), new Vector3(-1, 1), new Vector3(-1, -1)
            };

            foreach (Vector3 direction in directionsToMove)
            {
                // Queens may move along any X, Y, or diagonal axis. Unlike usual chess, these Queens have a range.
                for (int i = 1; i < range; i++)
                {
                    Vector3 destination = transform.position + (direction * i);
                    Vector3Int destinationTile = Vector3Int.FloorToInt(destination);
                    if (tilemap.HasTile(destinationTile))
                    {
                        continue;
                    }
                    else
                    {
                        if (Vector3.Distance(destination, playerObj.position) < 0.1f)
                        {
                            print("Queen takes Player");
                            return destination;
                        }
                        validMoves.Add(destination);
                    }
                }
            }

            print(validMoves.Count);
            int randomNum = Random.Range(0, validMoves.Count);

            return validMoves[randomNum];
        }
    }

}