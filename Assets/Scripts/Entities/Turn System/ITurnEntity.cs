namespace Entities
{
    public interface ITurnEntity
    {
        public void DoTurn(System.Action finished);

        public void InitTurn();

        float GetTurns();
    }
}