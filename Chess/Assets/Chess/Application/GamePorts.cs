using System;

namespace ChessClassic.Application
{
    public enum GameCommand { Flip, Reset, First, Previous, Replay, Next, Last }

    public interface IGameSession
    {
        event Action Changed;
        GameSnapshot Current { get; }
        int Ply { get; }
        int PlyCount { get; }
        void Seek(int ply);
        void Reset();
    }

    public interface IBoardView
    {
        event Action<int> SquareSelected;
        void Render(BoardFrame frame);
    }

    public interface IGameHud
    {
        event Action<GameCommand> CommandRequested;
        void Render(HudFrame frame);
    }
}
