using System;
using DG.Tweening;
using Entities.Turn_System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Entities.Tokens
{
    public enum TokenType
    {
        Nothing,
        Shield,
        Water,
        Meteor
    }
    
    public class Token : MonoBehaviour
    {
        public TextMeshPro assignedNum;
        public TurnManager turnManager;
        private Vector3 debugDestination;

        public void MoveTo(Vector3 destination)
        {
            debugDestination = destination;
            transform.DOMove(destination, 1f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(debugDestination, .5f);
        }
    }
}