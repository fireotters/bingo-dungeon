using System;
using System.Collections.Generic;
using DG.Tweening;
using Toolbox;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entities
{
    public abstract class AbstractEntity : MonoBehaviour, ITurnEntity
    {
        public Tilemap tilemap;
        public int range;
        public bool fourDir;
        private int lostTurns;
        private int hitPoints = 1;
        protected int extraTurns;
        private const int MAX_HEALTH = 2;
        protected Action currentFinishAction;

        // Re-orders the sprites on-screen as they move, so that pieces which are below others will render above them.
        // For example, a bishop on a space above a knight... rendering in above the knight. The whole knight should be visible, obscuring the bishop.
        // TODO Ask Rioni how to make this check only activate while a piece is moving.
        // Plan is:
        // - Lift the piece by 0.3 units, then temporarily increase the sorting order, so that it will fly over other pieces / cover
        // - Bring the sorting order back to normal after it's 'placed' back down.
        protected SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = -(int)transform.position.y;
        }

        public void AddLostTurn() => lostTurns++;

        protected bool CanDoTurn()
        {
            return lostTurns <= 0;
        }

        protected void ConsumeTurn() => lostTurns = Mathf.Max(lostTurns-1, 0);
        
        //private void Update()
        //{
        //    if (spriteRenderer != null)
        //    {
        //        spriteRenderer.sortingOrder = -Mathf.CeilToInt(transform.position.y);
        //    }
        //}

        protected List<Vector3> PreviewPath(Vector3 endPos)
        {
            return fourDir
                ? AStar.FindFourDirectionPath(tilemap, transform.position, endPos)
                : AStar.FindPath(tilemap, transform.position, endPos);
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
            Vector3Int distance = tilemap.WorldToCell(endPos) - tilemap.WorldToCell(transform.position);
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
        
        public void TakeDamage()
        {
            hitPoints--;
            print($"AAAAAAAAAAARGGGGGGGGGGGYHHHHHHHHHHHHHHHHHHHHHH MY BOOOONES (health is now {hitPoints})");

            if (hitPoints <= 0)
            {
                Destroy(gameObject);
                OnDeath();
            }
        }

        public abstract void DoTurn(Action finished);
        
        public virtual void OnDeath() { }

        public void InitTurn()
        {
            extraTurns = range;
        }

        public int GetTurns()
        {
            return extraTurns;
        }
    }
}