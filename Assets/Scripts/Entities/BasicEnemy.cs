using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FMODUnity;
using Signals;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities
{
    public abstract class BasicEnemy : AbstractEntity
    {
        [Tooltip("Time to move in seconds")] private float timeToMove = .5f;
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
        [SerializeField] private StudioEventEmitter enemyMovement, enemyDeath;

        private void Start()
        {
            playerObj = GameObject.Find("Player").transform;
            //boardManager = GameObject.Find("Board").GetComponent<Board>();
            BlackPiece = Convert.ToBoolean(Random.Range(0, 2));
            enemyAnimator = gameObject.GetComponent<Animator>();
            enemyAnimator.SetBool("Black", BlackPiece);
            SignalBus<SignalPieceAdded>.Fire(default);
        }

        private void OnDestroy()
        {
            SignalBus<SignalEnemyDied>.Fire();
        }

        [ContextMenu("Snap")]
        public void SnapToClosesTile()
        {
            transform.position = tilemap.WorldToCell(transform.position) + tilemap.cellSize/2;
        }

        public virtual Vector3 ChoosePosition()
        {
            // Movement constraints are set in each Enemy's script
            // Spawn movement reticules to illustrate piece range
            foreach (var validMove in validMoves)
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
                var playerPos = playerObj.transform.position;
                var movesCloserToPlayer = validMoves.Where(v3 =>
                {
                    var playerToDestinationDist = Vector3.Distance(playerPos, v3);
                    var playerToEnemyDist = Vector3.Distance(transform.position, playerPos);
                    return playerToDestinationDist < playerToEnemyDist;
                }).ToList();
                var movesAwayFromPlayer = validMoves.Where(v3 =>
                {
                    var playerToDestinationDist = Vector3.Distance(playerPos, v3);
                    var playerToEnemyDist = Vector3.Distance(transform.position, playerPos);
                    return playerToDestinationDist > playerToEnemyDist;
                }).ToList();
                print($"piece {gameObject.name} can move closer to player {movesCloserToPlayer.Count}");
                print($"piece {gameObject.name} can move away from player {movesAwayFromPlayer.Count}");

                var chanceToRetreat = Random.Range(1, 8);

                print($"piece {gameObject.name} will go closer to player: {chanceToRetreat > 2}");

                if (chanceToRetreat > 2)
                {
                    return movesCloserToPlayer.Count > 0
                        ? movesCloserToPlayer[Random.Range(0, movesCloserToPlayer.Count)]
                        : transform.position;
                }
                else
                {
                    return movesAwayFromPlayer.Count > 0 
                        ? movesAwayFromPlayer[Random.Range(0, movesAwayFromPlayer.Count)]
                        : transform.position;
                }
            }
        }

        public bool ValidateDestination(Vector3 modifier)
        {
            Vector3 destination = transform.position + modifier;
            Vector3Int destinationTile = tilemap.WorldToCell(destination);
            if (tilemap.HasTile(destinationTile) || IsCellOccupiedByEnemy(destination))
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

        private bool IsCellOccupiedByEnemy(Vector3 destination)
        {
            var searchLayer = LayerMask.NameToLayer("Pieces");
            var colliders = Physics2D.OverlapCircleAll(destination, 0.5f, searchLayer);

            foreach (var colliderFound in colliders)
            {
                if (colliderFound.gameObject != gameObject)
                {
                    if (colliderFound.gameObject.CompareTag("Enemy"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void DoMove(Action onFinished = null)
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
                    Debug.LogError("BasicEnemy Object '" + transform.name +
                                   "' has no moves to try! (Saved Unity from endless loop)");
                }
            } while (!moved);
        }

        public override bool TryMove(Vector3 destination, Action onFinish = null)
        {
            spriteRenderer.sortingOrder += 20;
            enemyMovement.Play();
            if (fatalMove != Vector3.zero)
            {
                SignalBus<SignalToggleFfw>.Fire(new SignalToggleFfw { Enabled = false });
                enemyMovement.SetParameter("Deadly", 1);
            }
            transform.DOMove(destination, timeToMove).OnComplete(
                () =>
                {
                    extraTurns = 0;
                    Damage();
                    onFinish?.Invoke();
                    spriteRenderer.sortingOrder -= 20;
                    spriteRenderer.sortingOrder = -(int)transform.position.y;
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
            enemyDeath.Play();
            if (corpsePrefab != null)
            {
                corpsePrefab.GetComponent<Corpse>().BlackPiece = BlackPiece;
                corpsePrefab.GetComponent<Corpse>().Pissed = Pissed;
                Instantiate(corpsePrefab, transform.position, Quaternion.identity);
            }
        }
    }
}