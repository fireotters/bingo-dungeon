using System;
using System.Collections.Generic;
using System.Threading;
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

        protected List<Vector3> PreviewPath(Vector3 endPos)
        {
            return fourDir ?
                AStar.FindFourDirectionPath(tilemap, transform.position, endPos)
                : AStar.FindPath(tilemap, transform.position, endPos);
        }

        public bool TryMove(Vector3 destination, System.Action onFinish = null)
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