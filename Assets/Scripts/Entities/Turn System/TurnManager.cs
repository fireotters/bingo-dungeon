using System.Collections.Generic;
using System.Linq;
using Level;
using TMPro;
using UnityEngine;

namespace Entities.Turn_System
{
    public class TurnManager : MonoBehaviour
    {
        public Transform currentTurnPointer;
        public List<ITurnEntity> turnEntities;
        public List<GameObject> turnEntitiesObjects;
        private int currentTurn;
        [SerializeField] private GridData gridData;
        [SerializeField] private GameObject token;
        private List<TextMeshPro> alreadyRolledNumbers;

        private void Start()
        {
            turnEntities = FindObjectsOfType<Component>().OfType<ITurnEntity>().ToList();
            turnEntitiesObjects = turnEntities.Cast<Component>().Select(x => x.gameObject).ToList();
            alreadyRolledNumbers = new List<TextMeshPro>();
            turnEntities[0].DoTurn(NextTurn);
            currentTurnPointer.position = (turnEntities[0] as Component).transform.position;
            print($"TurnManager: Number of Turn Entities is '{turnEntities.Count}");
        }

        private void NextTurn()
        {
            currentTurn++;
            var currentEntity = turnEntitiesObjects[currentTurn % turnEntitiesObjects.Count];
            if (currentEntity == null || !currentEntity.activeInHierarchy)
            {
                turnEntities.RemoveAt(currentTurn);
                turnEntitiesObjects.RemoveAt(currentTurn);
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
            // TODO generate a random token to drop on the board
            // if a meteor falls down, create a tile in the obstacles tilemap! w/ tilemap.SetTile(tile, positionInMap)
            Instantiate(token, number.transform.position, Quaternion.identity);
        }

        private TextMeshPro RollCage()
        {
            var rolledNumber = gridData.tileNumbers[Random.Range(0, gridData.tileNumbers.Count)];

            return alreadyRolledNumbers.Contains(rolledNumber) ? RollCage() : rolledNumber;
        }
    }
}