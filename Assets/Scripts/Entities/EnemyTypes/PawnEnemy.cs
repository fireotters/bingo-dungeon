using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class PawnEnemy : BasicEnemy
    {
        public override Vector3 ChoosePosition()
        {
            List<Vector3> validMoves = new List<Vector3>();
            Vector3[] directionsToMove = {
                new Vector3(0, 1), new Vector3(0, -1), new Vector3(1, 0), new Vector3(-1, 0)
            };

            foreach (Vector3 direction in directionsToMove)
            {
                // Pawns may move toward the other side. If they hit a wall, then move to either side.
                for (int i = 1; i < range; i++)
                {
                    validMoves.Add(transform.position + (direction * i));
                }
                // TODO Pawn is not functional yet
            }

            print(validMoves.Count);
            int randomNum = Random.Range(0, validMoves.Count);

            return validMoves[randomNum];
        }
    }

}