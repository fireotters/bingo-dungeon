using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Tokens;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Entities
{
    public class Player : AbstractEntity
    {
        public LineRenderer lineRenderer;
        [SerializeField] private Animator animator;
        private Vector3 previousPos;
		private List<Token> nearbyTokens = new List<Token>();
		[SerializeField] TextMeshPro text;

        private void Update()
        {
            var currentPos = transform.position;
            spriteRenderer.flipX = (currentPos - previousPos).x <= 0;
            previousPos = currentPos;
        }

        private void OnDestroy()
        {
            SignalBus<SignalGameEnded>.Fire(new SignalGameEnded { winCondition = false });
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.transform.parent.TryGetComponent<Token>(out var foundToken))
            {
                print("next to a token");
                nearbyTokens.Add(foundToken);
                print(nearbyTokens.Count);
            }
        }
        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.gameObject.transform.parent.TryGetComponent<Token>(out var foundToken))
            {
                print("moved away from a token");
                nearbyTokens.Remove(foundToken);
            }
        }

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
            if (extraTurns == range)
                text.gameObject.SetActive(true);
            text.text = extraTurns.ToString();
            spriteRenderer.sortingOrder += 20;

            while (true)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;

                if (nearbyTokens.Count > 0) // can move a token
                {
                    Token tokenSelected = MouseHoveringOverToken(mousePos, nearbyTokens);
                    if (tokenSelected)
                        if (Input.GetMouseButton(1))
                        {
                            Vector3 directionToMove = tokenSelected.transform.position - transform.position;
                            Vector3 intendedDestination = tokenSelected.transform.position + directionToMove;

                            Vector3Int destinationTile = tilemap.WorldToCell(intendedDestination);
                            if (!tilemap.HasTile(destinationTile))
                            {
                                tokenSelected.transform.DOMove(intendedDestination, 0.6f);
                                nearbyTokens.Remove(tokenSelected);

                                extraTurns -= 1;
                                text.text = extraTurns.ToString();
                                finished?.Invoke();
                                yield break;
                            }
                        }
                }

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
                        if (Math.Round(totalMoveCost) <= extraTurns)
                        {
                            lineRenderer.positionCount = previewLinePoints.Count;
                            lineRenderer.SetPositions(previewLinePoints.ToArray());

                            // Can only move if preview points exist
                            if (Input.GetMouseButton(0))
                            {
                                float xTo = mousePos.x;
                                float xFrom = gameObject.transform.position.x;
                                float xDiff = xTo - xFrom;

                                float yTo = mousePos.y;
                                float yFrom = gameObject.transform.position.y;
                                float yDiff = yTo - yFrom;

                                int dir = 0;

                                if (xDiff == 0 && yDiff > 0)
                                    dir = 2;
                                else if (xDiff < 0 && yDiff == 0)
                                    dir = 3;
                                else if (xDiff == 0 && yDiff < 0)
                                    dir = 0;
                                else if (xDiff > 0 && yDiff == 0)
                                    dir = 1;
                                else if (xDiff > 0 && yDiff > 0)
                                    dir = 3;
                                else if (xDiff > 0 && yDiff < 0)
                                    dir = 3;
                                else if (xDiff < 0 && yDiff > 0)
                                    dir = 1;
                                else if (xDiff < 0 && yDiff < 0)
                                    dir = 1;

                                animator.SetBool("Moving", true);
                                animator.SetInteger("Dir", dir);
                                animator.SetBool("Push", false);
                                if (TryMove(mousePos, () =>
                                {
                                    extraTurns -= Mathf.FloorToInt(totalMoveCost);
                                    text.text = extraTurns.ToString();

                                    if (extraTurns == 0)
                                        text.gameObject.SetActive(false);

                                    animator.SetBool("Moving", false);
                                    animator.SetInteger("Dir", 0);
                                    animator.SetBool("Push", false);
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

        private Token MouseHoveringOverToken(Vector3 mousePos, List<Token> listOfNearbyTokens)
        {
            foreach (Token token in listOfNearbyTokens)
            {
                if (Vector3.Distance(transform.position, token.transform.position) < 0.1f)
                {
                    continue; // Don't attempt to push tokens Player is standing on
                }
                if (Vector3.Distance(mousePos, token.transform.position) < 0.5f) {
                    return token;
                }
            }
            return null;
        }

        //private bool TryMoveToken(Vector3 mousePos)
        //{
        //    Vector3[] possibleDirections =
        //    {
        //        new(0, 1), new(0, -1), new(1, 0), new(-1, 0),
        //    };

        //    var validPositionsToMove = FigureOutValidPositions(possibleDirections);
        //    float previousDistance = 0;
        //    var nearestPointToMousePos = Vector3.zero;

        //    foreach (var validPosition in validPositionsToMove)
        //    {
        //        var distance = Vector3.Distance(mousePos, validPosition);
        //        if (nearestPointToMousePos == Vector3.zero || distance < previousDistance)
        //            nearestPointToMousePos = validPosition;

        //        previousDistance = distance;
        //    }

        //    var lineToDraw = new[] { nearbyToken.transform.position, nearestPointToMousePos };
        //    lineRenderer.positionCount = lineToDraw.Length;
        //    lineRenderer.SetPositions(lineToDraw);

        //    if (Input.GetMouseButton(1))
        //    {
        //        nearbyToken.MoveTo(nearestPointToMousePos);
        //        nearbyToken = null;
        //        return true;
        //    }

        //    return false;
        //}

        //private List<Vector3> FigureOutValidPositions(Vector3[] possibleDirections)
        //{
        //    var validPositions = new List<Vector3>();

        //    foreach (var possibleDirection in possibleDirections)
        //    {
        //        var supposedFinalTokenPosition = nearbyToken.transform.position + possibleDirection;
        //        if (supposedFinalTokenPosition != transform.position &&
        //            !IsPossibleDirectionAWall(supposedFinalTokenPosition))
        //        {
        //            validPositions.Add(supposedFinalTokenPosition);
        //        }
        //    }

        //    return validPositions;
        //}

        private bool IsPossibleDirectionAWall(Vector3 supposedFinalTokenPosition)
        {
            var positionInTilemap = tilemap.WorldToCell(supposedFinalTokenPosition);
            return tilemap.HasTile(positionInTilemap);
        }
    }
}
