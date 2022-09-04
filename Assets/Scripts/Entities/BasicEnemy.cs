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
        // Enemy states
        private bool _pissed = false;
        private bool _colorIsBlack;

        // Move timings
        private float _timeToMove = .3f;
        private readonly float _timeToStartTurn = .1f;



        public Transform moveReticuleGameObject;
        private Animator enemyAnimator;
        [SerializeField] private GameObject corpsePrefab;


        // Targetting
        private Transform playerObj;
        private List<Vector3> validMoves = new List<Vector3>();
        private Vector3 fatalMove = Vector3.zero;
        [SerializeField] private StudioEventEmitter enemyMovement, enemyDeath;

        CompositeDisposable _disposables = new CompositeDisposable();

        private void Start()
        {
            // Find Components
            enemyAnimator = GetComponent<Animator>();
            playerObj = GameObject.Find("Player").transform;

            // Colour the piece
            _colorIsBlack = Convert.ToBoolean(Random.Range(0, 2));
            enemyAnimator.SetBool("Black", _colorIsBlack);

            // SignalBus Actions
            SignalBus<SignalPieceAdded>.Fire(default);
            SignalBus<SignalEnemyDied>.Subscribe((x) =>
            {
                if (!_pissed)
                {
                    _pissed = true;
                    enemyAnimator.SetBool("Pissed", true);
                }
            }).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        [ContextMenu("Snap")]
        public void SnapToClosesTile()
        {
            transform.position = _tilemap.WorldToCell(transform.position) + _tilemap.cellSize/2;
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
                // Slow down piece which finishes off Player, accentuates the loss.
                _timeToMove *= 3;
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

                // Retreat Chance:
                // 20% if piece is Pissed, 50% if piece is docile
                int roll = Random.Range(1, 101);
                int retreat_chance = _pissed ? 20 : 50;
                bool enemy_will_retreat = roll < retreat_chance;
                print($"Piece {gameObject.name} wants to {(enemy_will_retreat ? "APPROACH" : "RETREAT")} - (Approach Moves: {movesCloserToPlayer.Count}) (Retreat Moves: {movesAwayFromPlayer.Count})");

                if (enemy_will_retreat)
                {
                    return movesAwayFromPlayer.Count > 0 ? movesAwayFromPlayer[Random.Range(0, movesAwayFromPlayer.Count)]
                         : movesAwayFromPlayer.Count > 0 ? movesCloserToPlayer[Random.Range(0, movesCloserToPlayer.Count)] : transform.position;
                }
                else
                {
                    return movesCloserToPlayer.Count > 0 ? movesCloserToPlayer[Random.Range(0, movesCloserToPlayer.Count)]
                         : movesAwayFromPlayer.Count > 0 ? movesAwayFromPlayer[Random.Range(0, movesAwayFromPlayer.Count)] : transform.position;
                }
            }
        }

        public bool ValidateDestination(Vector3 modifier)
        {
            Vector3 destination = transform.position + modifier;
            Vector3Int destinationTile = _tilemap.WorldToCell(destination);
            if (_tilemap.HasTile(destinationTile) || IsCellOccupiedByEnemy(destination))
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
            transform.DOMove(destination, _timeToMove).OnComplete(
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
            yield return new WaitForSeconds(_timeToStartTurn);
            DoMove(finished);
        }

        public override void OnDeath()
        {
            enemyDeath.Play();
            if (corpsePrefab != null)
            {
                corpsePrefab.GetComponent<Corpse>().BlackPiece = _colorIsBlack;
                Instantiate(corpsePrefab, transform.position, Quaternion.identity);
            }
        }
    }
}