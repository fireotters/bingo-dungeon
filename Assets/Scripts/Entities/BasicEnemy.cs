using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public abstract class BasicEnemy : AbstractEntity
    {

        [Tooltip("Time to move in seconds")]
        [SerializeField] float timeToMove;
        WaitForSeconds waitTimeToMove;
        [HideInInspector] public Transform playerObj;

        void Start()
        {
            waitTimeToMove = new WaitForSeconds(timeToMove);
            playerObj = GameObject.Find("Player").transform;
            //StartCoroutine(EnemyPatrol());
        }

        public abstract Vector3 ChoosePosition();


        private void DoMove(System.Action onFinished = null)
        {
            int attemptLimit = 100;
            int attempts = 0;
            bool moved;
            do
            {
                moved = TryMove(ChoosePosition(), onFinished);
                attempts += 1;
                if (attempts > attemptLimit)
                {
                    Debug.LogError("BasicEnemy Object '" + transform.name + "' cannot try any moves! (Saved Unity from endless loop)");
                }
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
