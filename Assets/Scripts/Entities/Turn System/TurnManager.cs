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
        private List<string> alreadyRolledNumbers;

        private void Start()
        {
            turnEntities = FindObjectsOfType<Component>().OfType<ITurnEntity>().ToList();
            turnEntitiesObjects = turnEntities.Cast<Component>().Select(x => x.gameObject).ToList();
            alreadyRolledNumbers = new List<string>();
            turnEntities[0].DoTurn(NextTurn);
            currentTurnPointer.position = (turnEntities[0] as Component).transform.position;
            print($"TurnManager: Number of Turn Entities is '{turnEntities.Count}");
        }

        private void NextTurn()
        {
            currentTurn++;
            if (currentTurn >= turnEntities.Count)
            {
                var selectedNumber = RollCage();
                print($"Rolled number: {selectedNumber}");
                // drop token on number
                currentTurn = 0;
            }

            var currentEntity = turnEntitiesObjects[currentTurn];
            if (currentEntity == null || !currentEntity.activeInHierarchy)
            {
                turnEntities.RemoveAt(currentTurn);
                turnEntitiesObjects.RemoveAt(currentTurn);
            }

            if (currentTurn >= turnEntities.Count)
                currentTurn = 0;

            turnEntities[currentTurn].DoTurn(NextTurn);
            currentTurnPointer.position = (turnEntities[currentTurn] as Component).transform.position + Vector3.up;
        }

        private string RollCage()
        {
            return gridData.tileNumbers[Random.Range(0, gridData.tileNumbers.Count)].text;
        }
    }
}
