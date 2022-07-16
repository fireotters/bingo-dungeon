using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities
{
    public abstract class BasicEnemy : AbstractEntity
    {

        [Tooltip("Time to move in seconds")]
        [SerializeField] float timeToMove;
        WaitForSeconds waitTimeToMove;
        [HideInInspector] public Transform playerObj;
        public Transform moveReticuleGameObject;
        public GameObject corpsePrefab;

        // Targetting
        public List<Vector3> validMoves = new List<Vector3>();
        public Vector3 fatalMove = Vector3.zero;


        void Start()
        {
            waitTimeToMove = new WaitForSeconds(timeToMove);
            playerObj = GameObject.Find("Player").transform;
            //StartCoroutine(EnemyPatrol());
        }

        public virtual Vector3 ChoosePosition()
        {
            // Movement constraints are set in each Enemy's script

            // Spawn movement reticules to illustrate piece range
            foreach (Vector3 validMove in validMoves)
                Instantiate(moveReticuleGameObject, validMove, Quaternion.identity);

            // If the player can be killed, do it. Else, pick a random valid destination.
            if (fatalMove != Vector3.zero)
            {
                return fatalMove;
            }
            else if (validMoves == null || validMoves.Count == 0)
                return transform.position;
            else
            {
                // TODO calculate the move most threatening to the player by using A* some way.
                int randomNum = Random.Range(0, validMoves.Count);
                return validMoves[randomNum];
            }
        }

        public bool ValidateDestination(Vector3 modifier)
        {
            Vector3 destination = transform.position + modifier;
            Vector3Int destinationTile = tilemap.WorldToCell(destination);
            if (tilemap.HasTile(destinationTile))
            {
                return false; // Tell the enemy to no longer search for valid tiles in this direction.
            }
            else
            {
                if (Vector3.Distance(destination, playerObj.position) < 0.1f)
                {
                    fatalMove = destination;
                }
                validMoves.Add(destination);
                return true;
            }
        }

        private void DoMove(System.Action onFinished = null)
        {
            // Reset targetting variables
            validMoves = new List<Vector3>();
            fatalMove = Vector3.zero;

            int attemptLimit = 100;
            int attempts = 0;
            bool moved;
            do
            {
                moved = TryMove(ChoosePosition(), onFinished);

                attempts += 1;
                if (attempts > attemptLimit)
                {
                    Debug.LogError("BasicEnemy Object '" + transform.name + "' has no moves to try! (Saved Unity from endless loop)");
                }
            } while (!moved);
        }

        public override bool TryMove(Vector3 destination, Action onFinish = null)
        {
            spriteRenderer.sortingOrder += 20;

            transform.DOMove(destination, 1f).OnComplete(
                () =>
                {
                    Damage();
                    onFinish?.Invoke();
                    spriteRenderer.sortingOrder -= 20;
                }
                );
            return true;
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

        public override void OnDeath()
        {
            Instantiate(corpsePrefab, transform.position, Quaternion.identity);
        }
    }
}
