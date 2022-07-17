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
    
    public abstract class Token : MonoBehaviour
    {
        public TextMeshPro assignedNum;
        public TurnManager turnManager;
        private Vector3 debugDestination;

        private void OnCollisionEnter2D(Collision2D col)
        {
            OnTokenPickup(col);
            turnManager.TokenWasCollected(assignedNum);
            Destroy(gameObject);
        }

        public void MoveTo(Vector3 destination)
        {
            debugDestination = destination;
            transform.DOMove(destination, 1f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(debugDestination, .5f);
        }

        protected abstract void OnTokenPickup(Collision2D col);
    }
}