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
            var result = new List<int>();
            if (from < 0 || from >= 64 || board[from] == '\0') return result;
            bool white = char.IsUpper(board[from]);
            for (int to = 0; to < 64; to++)
            {
                if (from == to || (board[to] != '\0' && char.IsUpper(board[to]) == white)) continue;
                if (char.ToUpperInvariant(board[to]) == 'K' || !CanReach(board, from, to, false)) continue;
                var next = (char[])board.Clone();
                next[to] = next[from];
                next[from] = '\0';
                int king = Array.IndexOf(next, white ? 'K' : 'k');
                bool attacked = false;
                for (int enemy = 0; enemy < 64 && !attacked; enemy++)
                    if (next[enemy] != '\0' && char.IsUpper(next[enemy]) != white)
                        attacked = king >= 0 && CanReach(next, enemy, king, true);
                if (!attacked) result.Add(to);
            }
            return result;
        }

        private static bool CanReach(char[] board, int from, int to, bool attackOnly)
        {
            int dx = to % 8 - from % 8, dy = to / 8 - from / 8;
            int ax = Math.Abs(dx), ay = Math.Abs(dy);
            bool white = char.IsUpper(board[from]);
            switch (char.ToUpperInvariant(board[from]))
            {
                case 'P':
                    int direction = white ? 1 : -1;
                    if (attackOnly) return ax == 1 && dy == direction;
                    if (ax == 1 && dy == direction) return board[to] != '\0';
                    if (dx != 0 || board[to] != '\0') return false;
                    if (dy == direction) return true;
                    return from / 8 == (white ? 1 : 6) && dy == direction * 2
                        && board[from + direction * 8] == '\0';
                case 'N': return ax * ay == 2;
                case 'K': return Math.Max(ax, ay) == 1;
                case 'B': if (ax != ay) return false; break;
                case 'R': if (dx != 0 && dy != 0) return false; break;
                case 'Q': if (dx != 0 && dy != 0 && ax != ay) return false; break;
                default: return false;
            }
            int steps = Math.Max(ax, ay);
            for (int step = 1; step < steps; step++)
                if (board[(from / 8 + Math.Sign(dy) * step) * 8 + from % 8 + Math.Sign(dx) * step] != '\0') return false;
            return steps > 0;
        }
    }
}
