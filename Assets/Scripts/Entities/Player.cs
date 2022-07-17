using System;
using System.Collections;
using UnityEngine;

namespace Entities
{
    public class Player : AbstractEntity
    {
        public LineRenderer lineRenderer;

        [SerializeField] Animator animator;
        Vector3 previousPos;

        private void Update()
        {
            if((transform.position - previousPos).x <= 0)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;

             previousPos = transform.position;
        }

        public override void DoTurn(Action finished)
        {
            if (CanDoTurn())
                StartCoroutine(PlayerTurn(finished));
            else
                StartCoroutine(LostTurn(finished));

            ConsumeTurn();
        }

        private void OnDestroy()
        {
            SignalBus<SignalGameEnded>.Fire(new SignalGameEnded { winCondition = false });
        }
        IEnumerator LostTurn(Action finished)
        {
            yield return new WaitForSeconds(1);
            finished?.Invoke();
        }

        IEnumerator PlayerTurn(Action finished)
        {
            spriteRenderer.sortingOrder += 20;

            while (true)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;

                if (IsInRange(mousePos))
                {
                    var previewLinePoints = PreviewPath(mousePos);

                    if (previewLinePoints != null)
                    {
                        // Dodgy move cost setup, because our A* implementation cannot return move costs.
                        // This measures how long a line is, and from that determine the cost.
                        // This is so that Player can't go around thin walls to destinations which are considered 'In Range'
                        float totalMoveCost = 0;
                        for (int i = 0; i < previewLinePoints.Count - 1; i++)
                        {
                            totalMoveCost += Vector3.Distance(previewLinePoints[i], previewLinePoints[i + 1]);
                        }
                        // Round down to allow more forgiving movement, but also round up to rein in movement a bit.
                        if (Math.Round(totalMoveCost) <= range)
                        {
                            lineRenderer.positionCount = previewLinePoints.Count;
                            lineRenderer.SetPositions(previewLinePoints.ToArray());

                            // Can only move if preview points exist
                            if (Input.GetMouseButton(0))
                            {
                                animator.SetBool("Moving", true);
                                if (TryMove(mousePos, () =>
                                {
                                    animator.SetBool("Moving", false);
                                    lineRenderer.positionCount = 0;
                                    Damage();
                                    spriteRenderer.sortingOrder -= 20;
                                    finished?.Invoke();
                                }))
                                    yield break;
                            }
                        }
                        else
                        {
                            lineRenderer.positionCount = 0;
                        }
                    }

                }
                else
                {
                    lineRenderer.positionCount = 0;
                }
                yield return null;
            }
        }
    }
}