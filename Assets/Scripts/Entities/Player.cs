using System;
using System.Collections;
using UnityEngine;

namespace Entities
{
    public class Player : AbstractEntity
    {
        public LineRenderer lineRenderer;

        [SerializeField] Animator animator;

        public override void DoTurn(Action finished)
        {
            if (CanDoTurn())
                StartCoroutine(PlayerTurn(finished));
            else
                StartCoroutine(LostTurn(finished));

            ConsumeTurn();
        }

        IEnumerator LostTurn(Action finished)
        {
            yield return new WaitForSeconds(1);
            finished?.Invoke();
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
                        animator.SetBool("Moving", true);
                        if (TryMove(mousePos, () =>
                        {
                            animator.SetBool("Moving", false);
                            lineRenderer.positionCount = 0;
                            Damage();
                            finished?.Invoke();
                        }))
                            yield break;
                    }
                }
                yield return null;
            }
        }
    }
}