using UnityEngine;

namespace Entities.Tokens
{
    public enum TokenType
    {
        NOTHING,
        SHIELD,
        WATER,
        METEOR,
    }
    
    public abstract class Token : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D col)
        {
            OnTokenPickup(col);
            Destroy(gameObject);
        }

        protected abstract void OnTokenPickup(Collision2D col);
    }
}