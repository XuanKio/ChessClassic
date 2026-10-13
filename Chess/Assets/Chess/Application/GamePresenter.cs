using System;
using ChessClassic.Domain;

namespace ChessClassic.Application
{
    /// <summary>Owns view state and translates inputs; never touches Unity objects.</summary>
    public sealed class GamePresenter : IDisposable
    {
        private readonly IGameSession session;
        private readonly IBoardView board;
        private readonly IGameHud hud;
        private bool flipped = true;
        private int selected = -1;
        private string status = "Xuân · Đối thủ  |  10 phút";
        private bool disposed;

        public GamePresenter(IGameSession session, IBoardView board, IGameHud hud)
        {
            this.session = session;
            this.board = board;
            this.hud = hud;
            board.SquareSelected += Select;
            hud.CommandRequested += Execute;
            session.Changed += OnSessionChanged;
            Render();
        }

        private void Select(int square)
        {
            if (square < 0 || square >= 64) return;
            selected = selected == square || session.Current.Position[square] == '\0' ? -1 : square;
            status = "Ô " + (char)('a' + square % 8) + (square / 8 + 1);
            Render();
        }

        private void Execute(GameCommand command)
        {
            switch (command)
            {
                case GameCommand.Flip: flipped = !flipped; Render(); break;
                case GameCommand.Reset: flipped = true; session.Reset(); break;
                case GameCommand.First: session.Seek(0); break;
                case GameCommand.Previous: session.Seek(session.Ply - 1); break;
                case GameCommand.Replay: session.Seek(session.Ply == 0 ? session.PlyCount : 0); break;
                case GameCommand.Next: session.Seek(session.Ply + 1); break;
                case GameCommand.Last: session.Seek(session.PlyCount); break;
            }
        }

        private void OnSessionChanged()
        {
            selected = -1;
            status = "Xuân · Đối thủ  |  10 phút";
            Render();
        }

        private void Render()
        {
            var snapshot = session.Current;
            board.Render(new BoardFrame(snapshot.Position.Cells,
                BasicMoveRules.Destinations(snapshot.Position, selected), flipped,
                selected, snapshot.LastMoveFrom, snapshot.LastMoveTo));
            hud.Render(new HudFrame(snapshot, flipped, status));
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            board.SquareSelected -= Select;
            hud.CommandRequested -= Execute;
            session.Changed -= OnSessionChanged;
        }
    }
}
