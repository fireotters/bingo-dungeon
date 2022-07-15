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

        public void Move(Vector3 destination)
        {
            if (IsInRange(transform.position, destination))
            {
                print("In range");
                var linePath = PreviewPath(destination);
                if (linePath != null)
                {
                    transform.DOPath(linePath.ToArray(), 1f);
                }
            }
        }
        
        protected bool IsInRange(Vector3 startPos, Vector3 endPos)
        {
            Vector3Int distance = tilemap.WorldToCell(endPos) - tilemap.WorldToCell(startPos);
            return distance.magnitude < (range + 1);
        }
    }
}