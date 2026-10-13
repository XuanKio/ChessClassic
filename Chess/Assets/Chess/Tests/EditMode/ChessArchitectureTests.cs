using System;
using System.Collections.Generic;
using ChessClassic.Application;
using ChessClassic.Domain;
using ChessClassic.Infrastructure;
using NUnit.Framework;

namespace ChessClassic.Tests
{
    public sealed class ChessArchitectureTests
    {
        [Test]
        public void BoardPositionRejectsInvalidDataAndOwnsItsCells()
        {
            Assert.False(BoardPosition.TryCreate(null, out _));
            Assert.False(BoardPosition.TryCreate(new char[63], out _));
            var cells = new char[64];
            cells[0] = 'X';
            Assert.False(BoardPosition.TryCreate(cells, out _));
            cells[0] = 'K';
            Assert.True(BoardPosition.TryCreate(cells, out var position));
            cells[0] = '\0';
            var exported = position.CopyCells();
            exported[0] = '\0';
            Assert.AreEqual('K', position[0]);
            Assert.Throws<NotSupportedException>(() => ((IList<char>)position.Cells)[0] = 'Q');
        }

        [TestCase(50, new[] { 34, 42 })]
        [TestCase(26, new[] { 34 })]
        [TestCase(57, new[] { 40, 42 })]
        [TestCase(56, new int[0])]
        [TestCase(58, new int[0])]
        [TestCase(10, new int[0])]
        public void SampleHintsRespectGeometryAndOccupancy(int square, int[] expected)
        {
            CollectionAssert.AreEquivalent(expected,
                BasicMoveRules.Destinations(new SampleGameSession().Current.Position, square));
        }

        [Test]
        public void PinnedPieceCannotExposeItsKing()
        {
            var cells = new char[64];
            cells[4] = 'K'; cells[12] = 'R'; cells[60] = 'r'; cells[56] = 'k';
            BoardPosition.TryCreate(cells, out var position);
            var moves = BasicMoveRules.Destinations(position, 12);
            Assert.False(moves.Contains(11));
            Assert.True(moves.Contains(20));
            Assert.True(moves.Contains(60));
        }

        [Test]
        public void PawnCannotJumpOrCaptureForwardAndKingAvoidsAttacks()
        {
            var cells = new char[64];
            cells[4] = 'K'; cells[60] = 'k'; cells[12] = 'P'; cells[20] = 'n'; cells[21] = 'p';
            BoardPosition.TryCreate(cells, out var position);
            var moves = BasicMoveRules.Destinations(position, 12);
            Assert.False(moves.Contains(20));
            Assert.False(moves.Contains(28));
            Assert.True(moves.Contains(21));
            cells[12] = '\0'; cells[20] = 'r';
            BoardPosition.TryCreate(cells, out position);
            Assert.False(BasicMoveRules.Destinations(position, 4).Contains(12));
        }

        [Test]
        public void SelectionFlipAndHistoryUpdateBothViewsWithoutMutatingOldFrame()
        {
            var board = new FakeBoard(); var hud = new FakeHud(); var session = new SampleGameSession();
            using (var presenter = new GamePresenter(session, board, hud))
            {
                board.Select(50);
                var beforeFlip = board.Frame;
                CollectionAssert.AreEquivalent(new[] { 34, 42 }, beforeFlip.Destinations);
                hud.Send(GameCommand.Flip);
                Assert.False(board.Frame.Flipped);
                Assert.True(beforeFlip.Flipped);
                Assert.False(hud.Frame.WhiteOnTop);
                CollectionAssert.AreEquivalent(beforeFlip.Destinations, board.Frame.Destinations);
                hud.Send(GameCommand.Previous);
                Assert.AreEqual(-1, board.Frame.Selected);
                Assert.AreEqual('P', board.Frame.Pieces[10]);
                Assert.AreEqual('\0', board.Frame.Pieces[26]);
                Assert.AreEqual("10:00", hud.Frame.WhiteClock);
                Assert.AreEqual("", hud.Frame.MoveText);
                hud.Send(GameCommand.Next);
                Assert.AreEqual('P', board.Frame.Pieces[26]);
                Assert.AreEqual("9:56", hud.Frame.WhiteClock);
                hud.Send(GameCommand.Reset);
                Assert.True(board.Frame.Flipped);
                Assert.AreEqual(-1, board.Frame.Selected);
            }
        }

        [Test]
        public void EmptyRepeatedAndInvalidInputDoNotLeaveStaleHints()
        {
            var board = new FakeBoard(); var hud = new FakeHud();
            using (var presenter = new GamePresenter(new SampleGameSession(), board, hud))
            {
                board.Select(50); board.Select(50);
                Assert.AreEqual(-1, board.Frame.Selected);
                Assert.IsEmpty(board.Frame.Destinations);
                board.Select(50); board.Select(42);
                Assert.AreEqual(-1, board.Frame.Selected);
                int renders = board.RenderCount;
                board.Select(-1); board.Select(64);
                Assert.AreEqual(renders, board.RenderCount);
            }
        }

        [Test]
        public void SessionCanBeReplacedAndPresenterUnsubscribesOnDispose()
        {
            var session = new FakeSession(); var board = new FakeBoard(); var hud = new FakeHud();
            var presenter = new GamePresenter(session, board, hud);
            var cells = new char[64]; cells[4] = 'K'; cells[60] = 'k'; cells[27] = 'Q';
            BoardPosition.TryCreate(cells, out var changed);
            session.Publish(new GameSnapshot(changed, 65, 120, "Alice", "Bob", "1. Qd4", 3, 27));
            Assert.AreEqual('Q', board.Frame.Pieces[27]);
            Assert.AreEqual("Alice", hud.Frame.WhiteName);
            Assert.AreEqual("1:05", hud.Frame.WhiteClock);
            presenter.Dispose(); presenter.Dispose();
            int renders = board.RenderCount;
            board.Select(27); hud.Send(GameCommand.Flip); session.Publish(session.Current);
            Assert.AreEqual(renders, board.RenderCount);
            using (var replacement = new GamePresenter(session, board, hud))
            {
                renders = board.RenderCount;
                board.Select(27);
                Assert.AreEqual(renders + 1, board.RenderCount);
            }
        }

        [Test]
        public void SessionHistoryClampsAndDoesNotMutateInitialSnapshot()
        {
            var session = new SampleGameSession();
            session.Seek(-100); var initial = session.Current;
            session.Seek(100);
            Assert.AreEqual(1, session.Ply);
            Assert.AreEqual('P', initial.Position[10]);
            Assert.AreEqual('\0', initial.Position[26]);
            Assert.AreEqual('P', session.Current.Position[26]);
        }

        private sealed class FakeBoard : IBoardView
        {
            public event Action<int> SquareSelected;
            public BoardFrame Frame;
            public int RenderCount;
            public void Render(BoardFrame frame) { Frame = frame; RenderCount++; }
            public void Select(int square) => SquareSelected?.Invoke(square);
        }

        private sealed class FakeHud : IGameHud
        {
            public event Action<GameCommand> CommandRequested;
            public HudFrame Frame;
            public void Render(HudFrame frame) => Frame = frame;
            public void Send(GameCommand command) => CommandRequested?.Invoke(command);
        }

        private sealed class FakeSession : IGameSession
        {
            public event Action Changed;
            public GameSnapshot Current { get; private set; } = new SampleGameSession().Current;
            public int Ply => 0;
            public int PlyCount => 0;
            public void Seek(int ply) => Changed?.Invoke();
            public void Reset() => Changed?.Invoke();
            public void Publish(GameSnapshot snapshot) { Current = snapshot; Changed?.Invoke(); }
        }
    }
}
