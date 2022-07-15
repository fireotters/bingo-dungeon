using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using Toolbox;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entities
{
    public abstract class AbstractEntity : MonoBehaviour
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

        public bool TryMove(Vector3 destination)
        {
            if (IsInRange(destination))
            {
                print("In range");
                var linePath = PreviewPath(destination);
                if (linePath != null)
                {
                    transform.DOPath(linePath.ToArray(), 1f).OnComplete(OnReached);
                    return true;
                }
            }

            return false;
        }

        protected virtual void OnReached() 
        {
            // Notify turn ended
        }
        
        protected bool IsInRange(Vector3 endPos)
        {
            Vector3Int distance = tilemap.WorldToCell(endPos) - tilemap.WorldToCell(transform.position);
            return distance.magnitude < (range + 1);
        }
    }
}