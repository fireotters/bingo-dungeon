using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class KnightEnemy : BasicEnemy
    {
        public override Vector3 ChoosePosition()
        {
            // Knights may only move once, as in usual Chess
            Vector3[] directionsToMove = {
                new Vector3(1, -2), new Vector3(1, 2), new Vector3(2, 1), new Vector3(2, -1),
                new Vector3(-1, -2), new Vector3(-1, 2), new Vector3(-2, 1), new Vector3(-2, -1)
            };

            foreach (Vector3 direction in directionsToMove)
            {
                ValidateDestination(direction);
            }

            return base.ChoosePosition();
        }

        private void OnDrawGizmosSelected()
        {
            Vector3[] directionsToMove = {
                new Vector3(1, -2), new Vector3(1, 2), new Vector3(2, 1), new Vector3(2, -1),
                new Vector3(-1, -2), new Vector3(-1, 2), new Vector3(-2, 1), new Vector3(-2, -1)
            };

            Gizmos.color = Color.green;
            foreach (Vector3 direction in directionsToMove)
                Gizmos.DrawWireSphere(transform.position + direction, 0.5f);
        }
    }

}