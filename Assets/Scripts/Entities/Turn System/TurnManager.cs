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
        public BingoWheelUi bingoWheelUi;
        public List<GameObject> turnEntitiesObjects;
        private int currentTurn;
        [SerializeField] private GridData gridData;
        [SerializeField] private Token[] tokens;
        private List<TextMeshPro> occupiedNumbers;
        [SerializeField] private TileBase meteorTile;
        [SerializeField] private Tilemap obstacleTilemap;
        bool rollTurn = true;
        CompositeDisposable disposables = new CompositeDisposable();

        private void Start()
        {
            SignalBus<SignalGameEnded>.Subscribe((gameEnded) => OnGameEnded()).AddTo(disposables);
            CreateListITurnEntity();
            turnEntitiesObjects = turnEntities.Cast<Component>().Select(x => x.gameObject).ToList();
            occupiedNumbers = new List<TextMeshPro>();
            turnEntities[0].InitTurn();
            turnEntities[0].DoTurn(NextTurn);
            UpdatePointer();
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
                        // Decide number
                        var selectedNumber = RollCage();
                        occupiedNumbers.Add(selectedNumber);
                        print($"Rolled number: {selectedNumber}");

                        // Decide effect
                        var values = Enum.GetValues(typeof(TokenType));
                        var type = (TokenType)values.GetValue(Random.Range(0, values.Length));

                        // Hide pointer, summon Bingo UI
                        currentTurnPointer.gameObject.SetActive(false);
                        yield return new WaitForSeconds(.5f);
                        bingoWheelUi.RunBingoWheelUi(Int16.Parse(selectedNumber.transform.name), type);
                        yield return new WaitForSeconds(3f);

                        // Drop token, return pointer
                        DropTokenOn(type, selectedNumber);
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

        private void DropTokenOn(TokenType type, TextMeshPro number)
        {
            switch (type)
            {
                case TokenType.Nothing:
                case TokenType.Shield:
                case TokenType.Water:
                case TokenType.Meteor:
                    var newToken = Instantiate(tokens[(int)type], number.transform.position, Quaternion.identity);
                    newToken.assignedNum = number;
                    newToken.turnManager = GetComponent<TurnManager>(); // Crossy: Oh help me, I don't understand this signalling library so I'm just passing turnmanager to every token
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //print(occupiedNumbers.Count);
        }

        private TextMeshPro RollCage()
        {
            UpdateOccupiedSquares();
            var rolledNumber = gridData.tileNumbers[Random.Range(0, gridData.tileNumbers.Count)];

            return occupiedNumbers.Contains(rolledNumber) ? RollCage() : rolledNumber;
        }

        public void TokenWasCollected(TextMeshPro freedNumber)
        {
            occupiedNumbers.Remove(freedNumber);
        }

        private void UpdateOccupiedSquares()
        {
            occupiedNumbers.Clear();
            foreach (TextMeshPro textObj in gridData.tileNumbers)
            {
                if (textObj.GetComponent<NumberSquare>().IsSomethingStandingOn())
                    occupiedNumbers.Add(textObj);
            }
            //print(occupiedNumbers.Count);
        }
    }
}