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
        private float timeToMove = .5f;
        private float timeToStartTurn = .5f;
        [HideInInspector] public Transform playerObj;
        //[HideInInspector] public Board boardManager;
        public Transform moveReticuleGameObject;
        bool BlackPiece;
        bool Pissed;
        Animator enemyAnimator;
        public GameObject corpsePrefab;

        // Targetting
        [HideInInspector] public List<Vector3> validMoves = new List<Vector3>();
        [HideInInspector] public Vector3 fatalMove = Vector3.zero;


        void Start()
        {
            playerObj = GameObject.Find("Player").transform;
            //boardManager = GameObject.Find("Board").GetComponent<Board>();
            BlackPiece = Convert.ToBoolean(Random.Range(0, 2));
            enemyAnimator = gameObject.GetComponent<Animator>();
            enemyAnimator.SetBool("Black", BlackPiece);
            SignalBus<SignalPieceAdded>.Fire(default);

            //StartCoroutine(EnemyPatrol());
        }

        private void OnDestroy()
        {
            SignalBus<SignalEnemyDied>.Fire();
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
                timeToMove *= 3; // Slow down piece which finishes off Player, accentuate the loss.
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

        //public abstract List<Vector3> FindProtectingSquares();

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
                Vector3 destination = ChoosePosition();
                moved = TryMove(destination, onFinished);

                attempts += 1;
                if (attempts > attemptLimit)
                {
                    Debug.LogError("BasicEnemy Object '" + transform.name + "' has no moves to try! (Saved Unity from endless loop)");
                }
            } while (!moved);
            //boardManager.UpdatePieceProtections(FindProtectingSquares());
        }

        public override bool TryMove(Vector3 destination, Action onFinish = null)
        {
            spriteRenderer.sortingOrder += 20;

            transform.DOMove(destination, timeToMove).OnComplete(
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
            yield return new WaitForSeconds(timeToStartTurn);
            DoMove(finished);
        }

        public override void OnDeath()
        {
            if (corpsePrefab != null)
            {
                corpsePrefab.GetComponent<Corpse>().BlackPiece = BlackPiece;
                corpsePrefab.GetComponent<Corpse>().Pissed = Pissed;
                Instantiate(corpsePrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
