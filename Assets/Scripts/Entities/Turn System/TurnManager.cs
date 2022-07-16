using System;
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
        public List<GameObject> turnEntitiesObjects;
        private int currentTurn;
        [SerializeField] private GridData gridData;
        [SerializeField] private Token[] tokens;
        private List<TextMeshPro> alreadyRolledNumbers;
        [SerializeField] private TileBase meteorTile;
        [SerializeField] private Tilemap obstacleTilemap;
        
        private void Start()
        {
            CreateListITurnEntity();
            turnEntitiesObjects = turnEntities.Cast<Component>().Select(x => x.gameObject).ToList();
            alreadyRolledNumbers = new List<TextMeshPro>();
            turnEntities[0].DoTurn(NextTurn);
            currentTurnPointer.position = (turnEntities[0] as Component).transform.position + Vector3.up;
            print($"TurnManager: Number of Turn Entities is '{turnEntities.Count}");
        }

        void CreateListITurnEntity()
        {
            turnEntities = entitiesContainer.GetComponentsInChildren<ITurnEntity>().ToList();
            ITurnEntity player = turnEntities.Find(x => (x as Component).gameObject.CompareTag("Player"));
            turnEntities.Remove(player);
            turnEntities.Insert(0, player);
        }

        private void NextTurn()
        {
            currentTurn++;
            int currentTurnTemp = currentTurn % turnEntitiesObjects.Count;
            var currentEntity = turnEntitiesObjects[currentTurnTemp];
            if (currentEntity == null || !currentEntity.activeInHierarchy)
            {
                turnEntities.RemoveAt(currentTurnTemp);
                turnEntitiesObjects.RemoveAt(currentTurnTemp);
            }

            if (currentTurn >= turnEntities.Count)
            {
                var selectedNumber = RollCage();
                alreadyRolledNumbers.Add(selectedNumber);
                print($"Rolled number: {selectedNumber}");
                DropTokenOn(selectedNumber);
                currentTurn = 0;
            }

            turnEntities[currentTurn].DoTurn(NextTurn);
            currentTurnPointer.position = (turnEntities[currentTurn] as Component).transform.position + Vector3.up;
        }

        private void DropTokenOn(TextMeshPro number)
        {
            var values = Enum.GetValues(typeof(TokenType));
            var type = (TokenType) values.GetValue(Random.Range(0, values.Length));

            switch (type)
            {
                case TokenType.NOTHING:
                case TokenType.SHIELD:
                case TokenType.WATER:
                    Instantiate(tokens[(int)type], number.transform.position, Quaternion.identity);
                    break;
                case TokenType.METEOR:
                    obstacleTilemap.SetTile(obstacleTilemap.WorldToCell(number.transform.position), meteorTile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private TextMeshPro RollCage()
        {
            var rolledNumber = gridData.tileNumbers[Random.Range(0, gridData.tileNumbers.Count)];

            return alreadyRolledNumbers.Contains(rolledNumber) ? RollCage() : rolledNumber;
        }
    }
}