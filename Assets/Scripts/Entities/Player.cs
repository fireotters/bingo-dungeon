using System;
using UnityEngine;

namespace Entities
{
    public class Player : AbstractEntity
    {
        public LineRenderer lineRenderer;

        private void Update()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            var previewLinePoints = PreviewPath(mousePos);
            
            if (previewLinePoints != null)
            {
                lineRenderer.positionCount = previewLinePoints.Count;
                lineRenderer.SetPositions(previewLinePoints.ToArray());
            }

            if (Input.GetMouseButton(0))
            {
                Move(mousePos);
            }
        }
    }
}