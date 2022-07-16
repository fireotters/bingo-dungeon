using System;
using System.Collections;
using UnityEngine;

namespace Entities
{
    public class Player : AbstractEntity
    {
        public LineRenderer lineRenderer;

        public override void DoTurn(Action finished)
        {
            StartCoroutine(PlayerTurn(finished));
        }

        IEnumerator PlayerTurn(Action finished)
        {
            while (true)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;

                if (IsInRange(mousePos))
                {
                    var previewLinePoints = PreviewPath(mousePos);

                    if (previewLinePoints != null)
                    {
                        lineRenderer.positionCount = previewLinePoints.Count;
                        lineRenderer.SetPositions(previewLinePoints.ToArray());
                    }

                    if (Input.GetMouseButton(0))
                    {
                        if (TryMove(mousePos, () =>
                        {
                            lineRenderer.positionCount = 0;
                            finished?.Invoke();
                        }))
                            yield break;
                    }
                }
                yield return null;
            }
        }


        //private void Update()
        //{
        //    var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    mousePos.z = 0;

        //    if (!IsInRange(mousePos))
        //        return;

        //    var previewLinePoints = PreviewPath(mousePos);

        //    if (previewLinePoints != null)
        //    {
        //        lineRenderer.positionCount = previewLinePoints.Count;
        //        lineRenderer.SetPositions(previewLinePoints.ToArray());
        //    }

        //    if (Input.GetMouseButton(0))
        //    {
        //        TryMove(mousePos);
        //    }
        //}
    }
}