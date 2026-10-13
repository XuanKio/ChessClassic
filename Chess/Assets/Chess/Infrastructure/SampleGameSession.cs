using System;
using ChessClassic.Application;
using ChessClassic.Domain;

namespace ChessClassic.Infrastructure
{
    /// <summary>Two-ply replay fixture, deliberately separate from a playable game session.</summary>
    public sealed class SampleGameSession : IGameSession
    {
        private readonly GameSnapshot[] snapshots;
        public event Action Changed;
        public int Ply { get; private set; } = 1;
        public int PlyCount => snapshots.Length - 1;
        public GameSnapshot Current => snapshots[Ply];

        public SampleGameSession()
        {
            var cells = new char[64];
            const string backRank = "RNBQKBNR";
            for (int file = 0; file < 8; file++)
            {
                cells[file] = backRank[file];
                cells[8 + file] = 'P';
                cells[48 + file] = 'p';
                cells[56 + file] = char.ToLowerInvariant(backRank[file]);
            }
            BoardPosition.TryCreate(cells, out var initial);
            cells[10] = '\0';
            cells[26] = 'P';
            BoardPosition.TryCreate(cells, out var afterC4);
            snapshots = new[]
            {
                new GameSnapshot(initial, 600, 600, "Đối thủ", "Xuân", ""),
                new GameSnapshot(afterC4, 596, 594, "Đối thủ", "Xuân", "1.       c4", 10, 26)
            };
        }

        public void Seek(int ply)
        {
            Ply = Math.Max(0, Math.Min(PlyCount, ply));
            Changed?.Invoke();
        }

        public void Reset() => Seek(PlyCount);
    }
}
