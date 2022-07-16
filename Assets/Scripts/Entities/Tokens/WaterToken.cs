using UnityEngine;

namespace Entities.Tokens
{
    public class WaterToken : Token
    {
        protected override void OnTokenPickup(Collision2D col)
        {
            if (col.gameObject.TryGetComponent<AbstractEntity>(out var entity))
            {
                entity.AddLostTurn();
                Destroy(gameObject);
            }
        }

    }
}