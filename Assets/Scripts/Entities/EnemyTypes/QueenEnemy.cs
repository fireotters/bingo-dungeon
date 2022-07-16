using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class QueenEnemy : BasicEnemy
    {
        public override Vector3 ChoosePosition()
        {
            // Queens may move along any X, Y, or diagonal axis. Unlike usual chess, these Queens have a range.
            Vector3[] directionsToMove = {
                new Vector3(0, 1), new Vector3(0, -1), new Vector3(1, 0), new Vector3(-1, 0),
                new Vector3(1, 1), new Vector3(1, -1), new Vector3(-1, 1), new Vector3(-1, -1)
            };

            foreach (Vector3 direction in directionsToMove)
            {
                for (int i = 1; i < range; i++)
                {
                    Vector3 modifier = direction * i;
                    if (ValidateDestination(modifier) == false)
                    {
                        break;
                    }
                }
            }

            return base.ChoosePosition();
        }
    }

}