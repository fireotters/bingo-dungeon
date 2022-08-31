using System;
using System.Collections.Generic;
using DG.Tweening;
using Toolbox;
using UnityEngine;
using UnityEngine.Tilemaps;
using Signals;

namespace Entities
{
    public abstract class AbstractEntity : MonoBehaviour, ITurnEntity
    {
        protected Tilemap _tilemap;
        public int range;
        public bool fourDir;
        private int lostTurns;
        protected int hitPoints = 1;
        protected float extraTurns;
        private const int MAX_HEALTH = 2;
        protected Action currentFinishAction;
        protected SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            foreach (Tilemap tm in FindObjectsOfType<Tilemap>())
            {
                if (tm.transform.name == "Obstacles")
                {
                    _tilemap = tm;
                    break;
                }
            }

            if (transform.position.x * 10 % 5 != 0 || transform.position.y * 10 % 5 != 0)
                Debug.LogError(transform.name + ": transform.pos.x & y must be set to a coord ending with .5! ");

            spriteRenderer.sortingOrder = -(int)transform.position.y;
        }

        public void AddLostTurn() => lostTurns++;

        protected bool CanDoTurn()
        {
            return lostTurns <= 0;
        }

        protected void ConsumeTurn() => lostTurns = Mathf.Max(lostTurns-1, 0);
        
        protected List<Vector3> PreviewPath(Vector3 endPos)
        {
            return fourDir
                ? AStar.FindFourDirectionPath(_tilemap, transform.position, endPos)
                : AStar.FindPath(_tilemap, transform.position, endPos);
        }

        public virtual bool TryMove(Vector3 destination, Action onFinish = null)
        {
            currentFinishAction = onFinish;
            if (IsInRange(destination))
            {
                var linePath = PreviewPath(destination);
                if (linePath != null)
                {
                    transform.DOPath(linePath.ToArray(), 1f).OnComplete(() =>
                    {
                        onFinish?.Invoke();
                        currentFinishAction = null;
                        });
                    return true;
                }
            }

            return false;
        }

        public void ChangePath(Vector3 newDestination)
        {
            if(currentFinishAction != null)
            {
                transform.DOKill();
                TryMove(newDestination, currentFinishAction);
            }
        }

        protected bool IsInRange(Vector3 endPos)
        {
            Vector3Int distance = _tilemap.WorldToCell(endPos) - _tilemap.WorldToCell(transform.position);
            bool isWithinRange = distance.magnitude < (range + 1);
            bool isNotSquarePlayerIsOn = distance.magnitude > 0.1f;
            return isWithinRange && isNotSquarePlayerIsOn;
        }

        protected void Damage()
        {
            var searchLayer = LayerMask.NameToLayer("Pieces");
            var colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, searchLayer);

            foreach (var colliderFound in colliders)
            {
                if (colliderFound.gameObject != gameObject)
                {
                    if (colliderFound.TryGetComponent<AbstractEntity>(out var otherEntity))
                    {
                        otherEntity.TakeDamage();
                        // If the player destroys an enemy, skip the player's turn
                        if (transform.name == "Player")
                        {
                            GetComponent<Player>().WaitAfterKillingThenEndTurn();
                        }
                    }
                }
            }
        }

        public void IncreaseHealth()
        {
            if (hitPoints < MAX_HEALTH)
            {
                hitPoints++;
                print($"increased health to {hitPoints}");
            }
        }
        
        protected virtual void TakeDamage()
        {
            if (hitPoints > 0)
            {
                print($"{transform.name}: AAAAAAAARGGGGGGGHHHHHHHHHHHHH MY BOOOONES (health is now {hitPoints})");
                hitPoints--;

                if (hitPoints == 0)
                {
                    Destroy(gameObject);

                    if (transform.name == "Player")
                        SignalBus<SignalGameEnded>.Fire(new SignalGameEnded { WinCondition = false });
                    else
                        SignalBus<SignalEnemyDied>.Fire();
                    OnDeath();
                }
            }
        }

        public abstract void DoTurn(Action finished);
        
        public virtual void OnDeath() { }

        public void InitTurn()
        {
            extraTurns = range;
        }

        public float GetTurns()
        {
            return extraTurns;
        }
    }
}