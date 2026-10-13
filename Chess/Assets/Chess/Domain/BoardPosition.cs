using System;
using System.Collections.Generic;

namespace ChessClassic.Domain
{
    /// <summary>An immutable 64-square position. Index 0 is a1; uppercase is White.</summary>
    public sealed class BoardPosition
    {
        private readonly char[] cells;
        public IReadOnlyList<char> Cells { get; }
        public char this[int square] => cells[square];

        private BoardPosition(char[] cells)
        {
            this.cells = cells;
            Cells = Array.AsReadOnly(cells);
        }

        public static bool TryCreate(IReadOnlyList<char> source, out BoardPosition position)
        {
            position = null;
            if (source == null || source.Count != 64) return false;
            var copy = new char[64];
            for (int i = 0; i < copy.Length; i++)
            {
                if (source[i] != '\0' && "PNBRQKpnbrqk".IndexOf(source[i]) < 0) return false;
                copy[i] = source[i];
            }
            position = new BoardPosition(copy);
            return true;
        }

        public char[] CopyCells() => (char[])cells.Clone();
    }
}
