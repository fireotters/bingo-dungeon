using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Tokens;
using UnityEngine;
using TMPro;
using DG.Tweening;
using FMODUnity;
using Signals;
using UI;

namespace Entities
{
    public class Player : AbstractEntity
    {
        private Animator _animator;
        private Vector3 previousPos;
        private List<Token> nearbyTokens = new List<Token>();
        [SerializeField] private StudioEventEmitter playerTurn, playerMove, playerDeath;
        private GameUi _gameUi;

        // Movement Cursor Related Variables
        private LineRenderer _lineRenderer;
        private Turn_System.TurnManager _turnManager;
        private TextMeshPro _textTurnsRemaining, _textMovementCost;
        private GameObject _movementCursor, _cursorOptionAttack, _cursorOptionDestination, _textTrHalfSign, _textMcHalfSign;
        [SerializeField] GameObject textSkipUi;
        private List<Transform> _currentEnemyTransforms = new List<Transform>();
        private Vector3 _lastFrameCursorPos = Vector3.zero;

        public override void Awake()
        {
            _gameUi = FindObjectOfType<Canvas>().GetComponent<GameUi>();
            _turnManager = FindObjectOfType<Turn_System.TurnManager>().GetComponent<Turn_System.TurnManager>();
            _lineRenderer = GetComponent<LineRenderer>();

            _movementCursor = transform.Find("MovementCursor").gameObject;
            _cursorOptionAttack = _movementCursor.transform.Find("AttackCursor").gameObject;
            _cursorOptionDestination = _movementCursor.transform.Find("DestinationCursor").gameObject;

            _textTurnsRemaining = transform.Find("TextTurnsRemaining").GetComponent<TextMeshPro>();
            _textMovementCost = _movementCursor.transform.Find("TextMovementCost").GetComponent<TextMeshPro>();
            _textTrHalfSign = _textTurnsRemaining.transform.Find("Half Sign").gameObject;
            _textMcHalfSign = _textMovementCost.transform.Find("Half Sign").gameObject;

            _animator = transform.Find("Sprite").GetComponent<Animator>();
            spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = -(int)transform.position.y;
        }

        private void Update()
        {
            var currentPos = transform.position;
            spriteRenderer.flipX = (currentPos - previousPos).x <= 0;
            previousPos = currentPos;
        }

        private void OnDestroy()
        {
            SignalBus<SignalGameEnded>.Fire(new SignalGameEnded { WinCondition = false });
        }

        // Player only checks triggers for Tokens which come into range
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.transform.parent.TryGetComponent<Token>(out var foundToken))
                nearbyTokens.Add(foundToken);
        }
        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.gameObject.transform.parent.TryGetComponent<Token>(out var foundToken))
                nearbyTokens.Remove(foundToken);
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

        public void SkipTurn()
        {
            StopAllCoroutines();
            extraTurns = 0;
            _textTurnsRemaining.text = extraTurns.ToString();
            _textTurnsRemaining.gameObject.SetActive(false);
            textSkipUi.SetActive(false);
            currentFinishAction?.Invoke();
            currentFinishAction = null;
            SignalBus<SignalToggleFfw>.Fire(new SignalToggleFfw() { Enabled = true });
        }

        public void WaitAfterKillingThenSkipTurn()
        {
            // Killing an enemy and immediately skipping turns caused enemy movement sounds to be doubled
            extraTurns = 0;
            _textTurnsRemaining.text = extraTurns.ToString();
            _textTurnsRemaining.gameObject.SetActive(false);
            textSkipUi.SetActive(false);
            Invoke(nameof(SkipTurn), 0.4f);
        }

        IEnumerator PlayerTurn(Action finished)
        {
            SignalBus<SignalToggleFfw>.Fire(new SignalToggleFfw() { Enabled = false });
            currentFinishAction = finished;
            if (extraTurns == range)
            {
                playerTurn.Play();
                _textTurnsRemaining.gameObject.SetActive(true);
                textSkipUi.SetActive(true);

                // Fetch all current enemies from TurnManager
                _currentEnemyTransforms.Clear();
                foreach (GameObject x in _turnManager.turnEntitiesObjects)
                    _currentEnemyTransforms.Add(x.transform);
            }
            _textTurnsRemaining.text = Mathf.FloorToInt(extraTurns).ToString();
            _textTrHalfSign.SetActive(extraTurns % 1f == .5f);

            while (true)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                if (_gameUi.gamePausePanel.activeInHierarchy)
                {
                    mousePos = new Vector3(6000, 6000, 0);
                }

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
                                _textTurnsRemaining.text = extraTurns.ToString();
                                finished?.Invoke();
                                SignalBus<SignalToggleFfw>.Fire(new SignalToggleFfw() { Enabled = true });
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
                        float totalMoveCost = 0;
                        for (int i = 0; i < previewLinePoints.Count - 1; i++)
                        {
                            float x = Vector3.Distance(previewLinePoints[i], previewLinePoints[i + 1]);
                            if (x % 1f != 0f)
                                // Round up any diagonal lines to their closest multiple of 1.5, instead of 1.4142
                                x = 1.5f * Mathf.FloorToInt((x + .75f) / 1.5f);
                            totalMoveCost += x;
                        }

                        if (totalMoveCost <= extraTurns + 0.5f)
                        {
                            // Render a line & movement cost sign if a move is valid.
                            _lineRenderer.positionCount = previewLinePoints.Count;
                            _lineRenderer.SetPositions(previewLinePoints.ToArray());
                            _movementCursor.SetActive(true);
                            _movementCursor.transform.position = previewLinePoints[^1];
                            _textMovementCost.text = Mathf.FloorToInt(totalMoveCost).ToString();
                            _textMcHalfSign.SetActive(totalMoveCost % 1f == .5f);

                            // Show a 'damage' sprite if cursor is over an enemy
                            if (_lastFrameCursorPos != _movementCursor.transform.position)
                            {
                                bool showAttackCursor = IsMouseHoveringOverEnemy(_movementCursor.transform.position);
                                _cursorOptionAttack.SetActive(showAttackCursor);
                                _cursorOptionDestination.SetActive(!showAttackCursor);
                            }
                            _lastFrameCursorPos = _movementCursor.transform.position;


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

                                _animator.SetBool("Moving", true);
                                _animator.SetInteger("Dir", dir);
                                _animator.SetBool("Push", false);

                                // Spawn a fake destination cursor
                                GameObject fakeDestinationCursor = Instantiate(_cursorOptionDestination, new Vector3(6000,6000, 0), Quaternion.identity);
                                if (_cursorOptionDestination.activeInHierarchy)
                                {
                                    fakeDestinationCursor.transform.position = _movementCursor.transform.position;
                                    fakeDestinationCursor.GetComponent<SpriteRenderer>().sortingLayerName = "Pieces (incl. Player)";
                                    fakeDestinationCursor.GetComponent<SpriteRenderer>().sortingOrder = -19;
                                }

                                textSkipUi.SetActive(false);
                                _movementCursor.SetActive(false);
                                playerMove.Play();
                                
                                if (TryMove(mousePos, () =>
                                {
                                    extraTurns -= totalMoveCost;
                                    _textTurnsRemaining.text = extraTurns.ToString();

                                    if (extraTurns <= 0)
                                    {
                                        _textTurnsRemaining.gameObject.SetActive(false);
                                    }
                                    else
                                        textSkipUi.SetActive(true);

                                    _animator.SetBool("Moving", false);
                                    _animator.SetInteger("Dir", 0);
                                    _animator.SetBool("Push", false);
                                    _lineRenderer.positionCount = 0;
                                    spriteRenderer.sortingOrder = -(int)transform.position.y;
                                    Destroy(fakeDestinationCursor);
                                    Damage();
                                    SignalBus<SignalToggleFfw>.Fire(new SignalToggleFfw() { Enabled = true });
                                    finished?.Invoke();
                                }))
                                    yield break;
                            }
                        }
                        else
                        {
                            _lineRenderer.positionCount = 0;
                            _movementCursor.SetActive(false);
                        }
                    }
                }
                else
                {
                    _lineRenderer.positionCount = 0;
                    _movementCursor.SetActive(false);
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

        private bool IsMouseHoveringOverEnemy(Vector3 movementCursorPos)
        {
            foreach (Transform enemy in _currentEnemyTransforms)
            {
                if (Vector3.Distance(movementCursorPos, enemy.position) < 0.1f)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void TakeDamage()
        {
            var expectedHealthValue = hitPoints - 1;
            
            if (expectedHealthValue <= 0)
            {
                playerDeath.Play();
            }

            base.TakeDamage();
        }
    }
}
