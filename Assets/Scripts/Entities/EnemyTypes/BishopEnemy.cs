using UnityEngine;

namespace Entities.EnemyTypes
{
    public class BishopEnemy : BasicEnemy
    {
        public override Vector3 ChoosePosition()
        {
            // Bishops may move along the diagonal axis. Unlike usual chess, these Bishops have a range.
            Vector3[] directionsToMove = {
                new Vector3(1, 1), new Vector3(1, -1), new Vector3(-1, 1), new Vector3(-1, -1)
            };

            foreach (Vector3 direction in directionsToMove)
            {
                for (int i = 1; i < range; i++)
                {
                    Vector3 modifier = direction * i;
                    if (!ValidateDestination(modifier))
                    {
                        break;
                    }
                }
            }

            return base.ChoosePosition();
        }
    }

}