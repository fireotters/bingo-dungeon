using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities.Tokens;
using Level;
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
        private BingoWheelUi _bingoWheelUi;
        public List<GameObject> turnEntitiesObjects;
        private int currentTurn;
        [SerializeField] private GridData gridData;
        [SerializeField] private Token blankToken;
        private List<TextMeshPro> _occupiedNumbers = new List<TextMeshPro>();
        [SerializeField] private TileBase meteorTile;
        [SerializeField] private Tilemap obstacleTilemap;
        bool rollTurn = true;
        CompositeDisposable disposables = new CompositeDisposable();

        // Tracking token locations (so the player cannot push tokens on top of each other)
        [HideInInspector] public List<Token> tokenEntities;
        [HideInInspector] public List<Vector3> tokenLocations;

        private void Start()
        {
            SignalBus<SignalGameEnded>.Subscribe((gameEnded) => OnGameEnded()).AddTo(disposables);
            CreateListITurnEntity();
            turnEntitiesObjects = turnEntities.Cast<Component>().Select(x => x.gameObject).ToList();
            turnEntities[0].InitTurn();
            turnEntities[0].DoTurn(NextTurn);
            _bingoWheelUi = FindObjectOfType<Canvas>().transform.Find("DialogBingoUI").GetComponent<BingoWheelUi>();
            UpdatePointer();

            // Drop the first two tokens
            Invoke(nameof(DropInitialTokens), 0.1f);
        }

        private void DropInitialTokens()
        {
            for (int i = 0; i < 2; i++)
            {
                var selectedNumber = DecideTokenNumber();
                var chosenColor = DecideTokenColor();
                DropTokenOn(selectedNumber, chosenColor, playSound:false);
            }
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
            if (rollTurn)
            {
                // Unpredictably, currentTurn will be higher than the max. In this case, skip to next round.
                if (currentTurn < turnEntities.Count)
                    if (turnEntities[currentTurn].GetTurns() > 0)
                        turnEntities[currentTurn].DoTurn(NextTurn);
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
                        // Decide number & color
                        var chosenNumber = DecideTokenNumber();
                        var chosenColor = DecideTokenColor();

                        // Hide pointer, summon Bingo UI
                        currentTurnPointer.gameObject.SetActive(false);
                        yield return new WaitForSeconds(.5f);
                        _bingoWheelUi.RunBingoWheelUi(Int16.Parse(chosenNumber.transform.name), chosenColor);
                        yield return new WaitForSeconds(2f);

                        // Drop token, return pointer
                        DropTokenOn(chosenNumber, chosenColor);
                        currentTurn = 0;
                        yield return new WaitForSeconds(.5f);
                        currentTurnPointer.gameObject.SetActive(true);
                    }

                    UpdatePointer();
                    turnEntities[currentTurn].InitTurn();
                    turnEntities[currentTurn].DoTurn(NextTurn);
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
            var newToken = Instantiate(blankToken, number.transform.position, Quaternion.identity);
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
        private TextMeshPro DecideTokenNumber(bool firstAttempt = true)
        {
            if (firstAttempt)
            {
                _occupiedNumbers.Clear();
                foreach (TextMeshPro textObj in gridData.tileNumbers)
                    if (textObj.GetComponent<NumberSquare>().IsSomethingStandingOn())
                        _occupiedNumbers.Add(textObj);
            }

            var rolledNumber = gridData.tileNumbers[Random.Range(0, gridData.tileNumbers.Count)];
            return _occupiedNumbers.Contains(rolledNumber) ? DecideTokenNumber(false) : rolledNumber;
        }
    }
}