using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Toolbox;

public class DemoPathFinding : MonoBehaviour
{
    public Transform endPos;
    public Transform startPos;
    public Tilemap tilemap;
    public LineRenderer linePath;
    private List<Vector3> wayPoints;

    private void Start()
    {
        wayPoints = new List<Vector3>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            endPos.position = mousePos;
            wayPoints = AStar.FindPath(tilemap, startPos.position, endPos.position);
            if (wayPoints != null)
            {
                linePath.positionCount = wayPoints.Count;
                linePath.SetPositions(wayPoints.ToArray());
            }
        }
    }
}
