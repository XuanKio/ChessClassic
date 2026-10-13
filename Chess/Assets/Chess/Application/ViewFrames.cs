using System;
using System.Collections.Generic;

namespace ChessClassic.Application
{
    public sealed class BoardFrame
    {
        public IReadOnlyList<char> Pieces { get; }
        public IReadOnlyList<int> Destinations { get; }
        public bool Flipped { get; }
        public int Selected { get; }
        public int LastMoveFrom { get; }
        public int LastMoveTo { get; }

        public BoardFrame(IReadOnlyList<char> pieces, IEnumerable<int> destinations, bool flipped,
            int selected, int lastMoveFrom, int lastMoveTo)
        {
            var copy = new char[pieces.Count];
            for (int i = 0; i < copy.Length; i++) copy[i] = pieces[i];
            Pieces = Array.AsReadOnly(copy);
            Destinations = new List<int>(destinations).AsReadOnly();
            Flipped = flipped;
            Selected = selected;
            LastMoveFrom = lastMoveFrom;
            LastMoveTo = lastMoveTo;
        }
    }

    public sealed class HudFrame
    {
        public string WhiteName { get; }
        public string BlackName { get; }
        public string WhiteClock { get; }
        public string BlackClock { get; }
        public string Status { get; }
        public string MoveText { get; }
        public bool WhiteOnTop { get; }

        public HudFrame(GameSnapshot snapshot, bool flipped, string status)
        {
            WhiteName = snapshot.WhiteName;
            BlackName = snapshot.BlackName;
            WhiteClock = FormatClock(snapshot.WhiteSeconds);
            BlackClock = FormatClock(snapshot.BlackSeconds);
            Status = status;
            MoveText = snapshot.MoveNotation;
            WhiteOnTop = flipped;
        }

        private static string FormatClock(int seconds)
        {
            seconds = Math.Max(0, seconds);
            return seconds / 60 + ":" + (seconds % 60).ToString("00");
        }
    }
}
