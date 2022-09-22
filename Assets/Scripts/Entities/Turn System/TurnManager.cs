using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities.Tokens;
using Level;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Entities.Turn_System
{
    public class TurnManager : MonoBehaviour
    {
        public Transform currentTurnPointer;
        public Transform entitiesContainer;
        public List<ITurnEntity> turnEntities;
        public List<GameObject> turnEntitiesObjects;
        private BingoWheelUi _bingoWheelUi;
        private int currentTurn;
        public int totalTurns = 1;
        [SerializeField] private GridData gridData;
        [SerializeField] private Token blankToken;
        private List<TextMeshPro> _occupiedNumbers = new List<TextMeshPro>();
        bool rollTurn = true;
        CompositeDisposable disposables = new CompositeDisposable();

        private int turnsToNextTokenClear = 4;
        private UI.GameUi _gameUi;

        // Tracking token locations (so the player cannot push tokens on top of each other)
        public List<Token> tokenEntities;
        [HideInInspector] public List<Vector3> tokenLocations;

        // Pieces Aggressive mode
        private bool tokensCanSpawn = true;
        private bool piecesJustBecamePissed = false;

        private void Start()
        {
            SignalBus<SignalGameEnded>.Subscribe((_) => OnGameEnded()).AddTo(disposables);
            SignalBus<SignalEnemyDied>.Subscribe((x) =>
            {
                piecesJustBecamePissed = true;
                tokensCanSpawn = false;
                RemoveTokensOnSquares("white", false);
                RemoveTokensOnSquares("black", false);
            }).AddTo(disposables);

            CreateListITurnEntity();
            turnEntitiesObjects = turnEntities.Cast<Component>().Select(x => x.gameObject).ToList();
            _bingoWheelUi = FindObjectOfType<Canvas>().transform.Find("OverlayBingoUI").GetComponent<BingoWheelUi>();
            _gameUi = FindObjectOfType<Canvas>().GetComponent<UI.GameUi>();
            UpdatePointer();
            _gameUi.UpdateTokenClearCooldown(turnsToNextTokenClear);

            // Drop the first two tokens
            Invoke(nameof(DropInitialToken), 0.05f);
            Invoke(nameof(DropInitialToken), 0.1f);
            Invoke(nameof(StartPlayer), 0.2f);
        }

        private void DropInitialToken()
        {
            var selectedNumber = DecideTokenNumber();
            if (selectedNumber != null)
            {
                var chosenColor = DecideTokenColor();
                DropTokenOn(selectedNumber, chosenColor, playSound: false);
            }
        }

        private void StartPlayer()
        {
            turnEntities[0].InitTurn();
            turnEntities[0].DoTurn(NextTurn);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void OnGameEnded()
        {
            rollTurn = false;
            currentTurnPointer.gameObject.SetActive(false);
        }

        private void UpdatePointer()
        {
            var component = (turnEntities[currentTurn] as Component);
            if (component == null)
                return;
            var currentEntTransform = component.transform;
            currentTurnPointer.position = currentEntTransform.position + Vector3.up;
            currentTurnPointer.SetParent(currentEntTransform);
        }

        void CreateListITurnEntity()
        {
            turnEntities = entitiesContainer.GetComponentsInChildren<ITurnEntity>().ToList();
            ITurnEntity player = turnEntities.Find(x => (x as Component).gameObject.CompareTag("Player"));
            turnEntities.Remove(player);
            turnEntities.Insert(0, player);
        }

        IEnumerator DoNextTurn()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            if (piecesJustBecamePissed)
            {
                piecesJustBecamePissed = false;
                float tempTime = Time.timeScale;
                Time.timeScale = 1f;
                yield return new WaitForSeconds(2f);
                Time.timeScale = tempTime;
            }
            if (rollTurn)
            {
                // Unpredictably, currentTurn will be higher than the max. In this case, skip to next round.
                if (currentTurn < turnEntities.Count)
                {
                    if (turnEntities[currentTurn].GetTurns() > 0)
                    {
                        turnEntities[currentTurn].DoTurn(NextTurn);
                    }
                    else
                    {
                        currentTurn++;
                        bool finished = false;

                        int currentTurnTemp = currentTurn % turnEntitiesObjects.Count;
                        var currentEntity = turnEntitiesObjects[currentTurnTemp];
                        while (currentEntity == null || !currentEntity.activeInHierarchy)
                        {
                            turnEntities.RemoveAt(currentTurnTemp);
                            turnEntitiesObjects.RemoveAt(currentTurnTemp);

                            currentTurnTemp = currentTurn % turnEntitiesObjects.Count;
                            currentEntity = turnEntitiesObjects[currentTurnTemp];
                        }

                        if (currentTurn >= turnEntities.Count || finished)
                        {
                            // Update token clear cooldown
                            if (turnsToNextTokenClear > 0)
                                turnsToNextTokenClear -= 1;
                            _gameUi.UpdateTokenClearCooldown(turnsToNextTokenClear);

                            currentTurnPointer.gameObject.SetActive(false);
                            // Drop a token if less than 10 exist
                            if (tokenEntities.Count < 10 && tokensCanSpawn)
                            {
                                // Decide number & color
                                var chosenNumber = DecideTokenNumber();
                                if (chosenNumber != null)
                                {
                                    var chosenColor = DecideTokenColor();

                                    // Hide pointer, summon Bingo UI
                                    yield return new WaitForSeconds(.5f);
                                    _bingoWheelUi.RunBingoWheelUi(Int16.Parse(chosenNumber.transform.name), chosenColor);
                                    yield return new WaitForSeconds(2f);

                                    // Drop token, return pointer
                                    DropTokenOn(chosenNumber, chosenColor);
                                }
                            }
                            currentTurn = 0;
                            yield return new WaitForSeconds(.5f);
                            currentTurnPointer.gameObject.SetActive(true);
                            totalTurns++;
                        }

                        UpdatePointer();
                        turnEntities[currentTurn].InitTurn();
                        turnEntities[currentTurn].DoTurn(NextTurn);
                    }
                }
            }
        }

        private void NextTurn()
        {
            StartCoroutine(DoNextTurn());
        }

        private Color DecideTokenColor()
        {
            var colors = TokenColors.tokenColors;
            return (Color)colors.GetValue(Random.Range(0, colors.Length));
        }

        private void DropTokenOn(TextMeshPro number, Color chosenColor, bool playSound = true)
        {
            var newToken = Instantiate(blankToken, number.transform.position, Quaternion.identity, entitiesContainer);
            if (!playSound)
                Destroy(newToken.GetComponent<FMODUnity.StudioEventEmitter>());

            newToken.ChangeColor(chosenColor);
            tokenEntities.Add(newToken);
        }

        public void UpdateTokenLocations()
        {
            tokenLocations.Clear();
            foreach (Token token in tokenEntities)
                tokenLocations.Add(token.transform.position);
        }

        // Decide which tile to drop a token on by checking if a suggested tile is already occupied.
        private TextMeshPro DecideTokenNumber(bool firstAttempt = true, int iteration = 0)
        {
            if (iteration >= 20)
            {
                print("Token attempted to spawn 20 times, assume it is because of limited space & don't spawn a token.");
                return null;
            }
            if (firstAttempt)
            {
                _occupiedNumbers.Clear();
                foreach (TextMeshPro textObj in gridData.tileNumbers)
                    if (textObj.GetComponent<NumberSquare>().IsSomethingStandingOn())
                        _occupiedNumbers.Add(textObj);
            }

            var rolledNumber = gridData.tileNumbers[Random.Range(0, gridData.tileNumbers.Count)];
            return _occupiedNumbers.Contains(rolledNumber) ? DecideTokenNumber(false, iteration + 1) : rolledNumber;
        }


        // Remove tokens on a certain color of tile. (They're separate methods because values can't be passed through UnityActions)
        public void RemoveTokensOnBlackSquares()
        {
            RemoveTokensOnSquares("black", true);
        }
        public void RemoveTokensOnWhiteSquares()
        {
            RemoveTokensOnSquares("white", true);
        }

        public void RemoveTokensOnSquares(string color, bool playDestroySound)
        {
            List<Token> tokenEntitiesCopy = new List<Token>(tokenEntities);
            foreach (Token token in tokenEntitiesCopy)
            {
                if (token.colorOfCurrentSquare == color)
                {
                    tokenEntities.Remove(token);
                    StartCoroutine(token.DestroyToken());
                }
            }
            if (tokenEntities.Count != tokenEntitiesCopy.Count)
            {
                _gameUi.UpdateTokenClearCooldown(4, playDestroySound: playDestroySound);
                turnsToNextTokenClear = 4;
            }
        }
    }
}