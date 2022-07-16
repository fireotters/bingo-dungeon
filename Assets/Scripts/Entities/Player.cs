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
                            spriteRenderer.sortingOrder -= 20;
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