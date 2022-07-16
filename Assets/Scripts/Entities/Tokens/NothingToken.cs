using System;
using UnityEngine;

namespace Entities.Tokens
{
    public class NothingToken : Token
    {
        protected override void OnTokenPickup(Collision2D col)
        {
           Destroy(gameObject);
        }
    }
}