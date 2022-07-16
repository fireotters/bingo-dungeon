using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class PawnEnemy : BasicEnemy
    {
        public override Vector3 ChoosePosition()
        {
            // TODO Pawn is not functional yet
            // Pawns may move toward the other side. If they hit a wall, then move to either side.
            Vector3[] directionsToMove = {
                new Vector3(0, 1), new Vector3(0, -1), new Vector3(1, 0), new Vector3(-1, 0)
            };

            foreach (Vector3 direction in directionsToMove)
            {
                for (int i = 1; i < range; i++)
                {
                    validMoves.Add(transform.position + (direction * i));
                }
            }

            throw new NotImplementedException();
        }
    }

}