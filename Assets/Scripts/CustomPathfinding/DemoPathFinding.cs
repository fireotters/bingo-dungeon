using Toolbox;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class DemoPathFinding : MonoBehaviour
{
    public Transform endPos;
    public Transform startPos;
    public Tilemap tilemap;
    public int range;
    [FormerlySerializedAs("linePath")] public LineRenderer lineRenderer;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            endPos.position = mousePos;
            var linePath = AStar.FindFourDirectionLinePath(tilemap, startPos.position, endPos.position);

            if (linePath != null &&
                IsInRange(tilemap.WorldToCell(startPos.position), tilemap.WorldToCell(linePath.EndNode)))
            {
                lineRenderer.positionCount = linePath.Length;
                lineRenderer.SetPositions(linePath.nodes);
                startPos.DOPath(linePath.nodes, 1f);
            }
        }
    }

    bool IsInRange(Vector3Int startPos, Vector3Int endPos)
    {
        Vector3Int distance = endPos - startPos;
        return distance.magnitude < (range + 1);
    }
}