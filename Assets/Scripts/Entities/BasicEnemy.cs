using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class BasicEnemy : AbstractEntity
    {

        [Tooltip("Time to move in seconds")]
        [SerializeField] float timeToMove;
        WaitForSeconds waitTimeToMove;

        void Start()
        {
            waitTimeToMove = new WaitForSeconds(timeToMove);
            StartCoroutine(EnemyPatrol());
        }

        Vector3 ChoosePosition()
        {
            return transform.position + new Vector3(Random.Range(-range, range+1), Random.Range(-range, range+1));
        }

        IEnumerator EnemyPatrol()
        {
            while (true)
            {
                TryMove(ChoosePosition());
                yield return waitTimeToMove;
            }
        }
    }
}
