using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entities
{
    public class TurnManager : MonoBehaviour
    {
        public Transform currentTurnPointer;
        public ITurnEntity[] turnEntities;
        int currentTurn = 0;

        void Start()
        {
            turnEntities = FindObjectsOfType<Component>().OfType<ITurnEntity>().ToArray();
            turnEntities[0].DoTurn(NextTurn);
            currentTurnPointer.position = (turnEntities[0] as Component).transform.position;
            print("TurnManager: Number of Turn Entities is '" + turnEntities.Length + "'");
        }

        void NextTurn()
        {
            currentTurn++;
            if(currentTurn >= turnEntities.Length)
                currentTurn = 0;
            turnEntities[currentTurn].DoTurn(NextTurn);
            currentTurnPointer.position = (turnEntities[currentTurn] as Component).transform.position + Vector3.up;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
