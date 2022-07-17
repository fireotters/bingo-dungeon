using UnityEngine;
using TMPro;
using Entities.Turn_System;
using System.Collections;

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
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            OnTokenPickup(col);
            turnManager.TokenWasCollected(assignedNum);
            Destroy(gameObject);
        }

        protected abstract void OnTokenPickup(Collision2D col);
    }
}