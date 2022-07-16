using System;

namespace Entities.Tokens
{
    public class NothingToken : Token
    {
        protected override void OnTokenPickup()
        {
            throw new NotImplementedException();
        }
    }
}