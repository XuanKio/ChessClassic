using ChessClassic.Domain;

namespace ChessClassic.Application
{
    /// <summary>Read-only session data shared by local, replay or future remote adapters.</summary>
    public sealed class GameSnapshot
    {
        public BoardPosition Position { get; }
        public int WhiteSeconds { get; }
        public int BlackSeconds { get; }
        public string WhiteName { get; }
        public string BlackName { get; }
        public string MoveNotation { get; }
        public int LastMoveFrom { get; }
        public int LastMoveTo { get; }

        public GameSnapshot(BoardPosition position, int whiteSeconds, int blackSeconds,
            string whiteName, string blackName, string moveNotation, int lastMoveFrom = -1, int lastMoveTo = -1)
        {
            Position = position;
            WhiteSeconds = whiteSeconds;
            BlackSeconds = blackSeconds;
            WhiteName = whiteName;
            BlackName = blackName;
            MoveNotation = moveNotation;
            LastMoveFrom = lastMoveFrom;
            LastMoveTo = lastMoveTo;
        }
    }
}
