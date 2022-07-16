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
        public ITurnEntity[] turnEntities;
        private int currentTurn;
        [SerializeField] private GridData gridData;
        private List<string> alreadyRolledNumbers;

        private void Start()
        {
            turnEntities = FindObjectsOfType<Component>().OfType<ITurnEntity>().ToArray();
            alreadyRolledNumbers = new List<string>();
            turnEntities[0].DoTurn(NextTurn);
            currentTurnPointer.position = (turnEntities[0] as Component).transform.position;
            print($"TurnManager: Number of Turn Entities is '{turnEntities.Length}");
        }

        private void NextTurn()
        {
            currentTurn++;
            if (currentTurn >= turnEntities.Length)
            {
                var selectedNumber = RollCage();
                print($"Rolled number: {selectedNumber}");
                // drop token on number
                currentTurn = 0;
            }
            
            turnEntities[currentTurn].DoTurn(NextTurn);
            currentTurnPointer.position = (turnEntities[currentTurn] as Component).transform.position + Vector3.up;
        }

        private string RollCage()
        {
            return gridData.tileNumbers[Random.Range(0, gridData.tileNumbers.Count)].text;
        }
    }
}
