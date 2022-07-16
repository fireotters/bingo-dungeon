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
            //StartCoroutine(EnemyPatrol());
        }

        Vector3 ChoosePosition()
        {
            return transform.position + new Vector3(Random.Range(-range, range + 1), Random.Range(-range, range + 1));
        }

        IEnumerator EnemyPatrol()
        {
            while (true)
            {
                DoMove();
                
                yield return waitTimeToMove;
            }
        }

        private void DoMove(System.Action onFinished = null)
        {
            bool moved;
            do
            {
                moved = TryMove(ChoosePosition(), onFinished);
            } while (!moved);
        }

        public override void DoTurn(System.Action finished)
        {
            StartCoroutine(EnemyTurn(finished));
        }

        IEnumerator EnemyTurn(System.Action finished)
        {
            yield return new WaitForSeconds(1f);
            DoMove(finished);
        }
    }
}
