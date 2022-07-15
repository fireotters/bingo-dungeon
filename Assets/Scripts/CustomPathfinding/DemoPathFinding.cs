using System.Collections.Generic;
using Toolbox;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class DemoPathFinding : MonoBehaviour
{
    public Transform endPos;
    public Transform startPos;
    public Tilemap tilemap;
    [FormerlySerializedAs("linePath")] public LineRenderer lineRenderer;

    private void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            endPos.position = mousePos;
            var linePath = AStar.FindFourDirectionLinePath(tilemap, startPos.position, endPos.position);

            if (linePath != null)
            {
                lineRenderer.positionCount = linePath.Length;
                lineRenderer.SetPositions(linePath.nodes);
            }
        // }
    }
}
