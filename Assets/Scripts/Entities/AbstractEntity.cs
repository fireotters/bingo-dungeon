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

        // Re-orders the sprites on-screen as they move, so that pieces which are below others will render above them.
        // For example, a bishop on a space above a knight... rendering in above the knight. The whole knight should be visible, obscuring the bishop.
        // TODO Ask Rioni how to make this check only activate while a piece is moving.
        // Plan is:
        // - Lift the piece by 0.3 units, then temporarily increase the sorting order, so that it will fly over other pieces / cover
        // - Bring the sorting order back to normal after it's 'placed' back down.
        SpriteRenderer spriteRenderer;
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        private void Update()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = -Mathf.CeilToInt(transform.position.y);
            }
        }

        protected List<Vector3> PreviewPath(Vector3 endPos)
        {
            return fourDir ?
                AStar.FindFourDirectionPath(tilemap, transform.position, endPos)
                : AStar.FindPath(tilemap, transform.position, endPos);
        }

        public virtual bool TryMove(Vector3 destination, System.Action onFinish = null)
        {
            if (IsInRange(destination))
            {
                var linePath = PreviewPath(destination);
                if (linePath != null)
                {
                    transform.DOPath(linePath.ToArray(), 1f).OnComplete(() => onFinish?.Invoke());
                    return true;
                }
            }

            return false;
        }

        protected bool IsInRange(Vector3 endPos)
        {
            Vector3Int distance = tilemap.WorldToCell(endPos) - tilemap.WorldToCell(transform.position);
            return distance.magnitude < (range + 1);
        }

        public abstract void DoTurn(Action finished);
    }
}