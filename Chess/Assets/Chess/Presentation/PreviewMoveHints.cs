using System;
using System.Collections.Generic;

namespace ChessClassic.Presentation
{
    /// <summary>
    /// Move hints for the initial position and the sample c4 position only.
    /// These positions have no available castling or en-passant moves.
    /// This is not a game session or a general move-execution engine.
    /// </summary>
    public static class PreviewMoveHints
    {
        public static char[] Position(bool afterC4)
        {
            var board = new char[64];
            const string backRank = "RNBQKBNR";
            for (int file = 0; file < 8; file++)
            {
                board[file] = backRank[file];
                board[8 + file] = 'P';
                board[48 + file] = 'p';
                board[56 + file] = char.ToLowerInvariant(backRank[file]);
            }
            if (afterC4) { board[10] = '\0'; board[26] = 'P'; }
            return board;
        }

        public static List<int> Destinations(char[] board, int from)
        {
            return ChessClassic.Domain.BoardPosition.TryCreate(board, out var position)
                ? ChessClassic.Domain.BasicMoveRules.Destinations(position, from)
                : new List<int>();
        }
    }
}
