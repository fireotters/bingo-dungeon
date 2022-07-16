using UnityEngine;
using DG.Tweening;

namespace Entities.Tokens
{
    public class WaterToken : Token
    {
        protected override void OnTokenPickup(Collision2D col)
        {
            if (col.gameObject.TryGetComponent<AbstractEntity>(out var entity))
            {
                entity.AddLostTurn();
                entity.ChangePath(transform.position);
                Destroy(gameObject);
            }
        }

    }
}