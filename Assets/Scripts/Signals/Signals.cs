namespace Signals
{
    public struct SignalMainMenuStartGame
    {
        public string levelToLoad;
    }


    public enum GameEndCondition
    {
        Loss, BingoWin, PieceWin
    }
    public struct SignalGameEnded
    {
        public GameEndCondition result;
    }

    public struct SignalToggleFfw
    {
        public bool Enabled;
    }

    public struct SignalPieceAdded
    {
    }

    public struct SignalEnemyDied
    {
    }
}

