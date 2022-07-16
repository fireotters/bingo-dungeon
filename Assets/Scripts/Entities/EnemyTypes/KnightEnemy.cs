using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class KnightEnemy : BasicEnemy
    {
        public override Vector3 ChoosePosition()
        {
            List<Vector3> validMoves = new List<Vector3>();
            Vector3[] directionsToMove = {
                new Vector3(1, -2), new Vector3(1, 2), new Vector3(2, 1), new Vector3(2, -1),
                new Vector3(-1, -2), new Vector3(-1, 2), new Vector3(-2, 1), new Vector3(-2, -1)
            };

            foreach (Vector3 direction in directionsToMove)
            {
                // Knights may only move once, as in usual Chess
                Vector3 destination = transform.position + direction;
                Vector3Int destinationTile = Vector3Int.FloorToInt(destination);
                if (tilemap.HasTile(destinationTile))
                {
                    continue;
                }
                else
                {
                    if (Vector3.Distance(destination, playerObj.position) < 0.1f)
                    {
                        print("Knight takes Player");
                        return destination;
                    }
                    validMoves.Add(destination);
                }
            }

            //print(validMoves.Count);
            int randomNum = Random.Range(0, validMoves.Count); // TODO calculate the move most threatening to the player by using A* some way.
            return validMoves[randomNum];
        }
    }

}