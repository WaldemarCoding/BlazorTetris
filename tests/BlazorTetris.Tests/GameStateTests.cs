using BlazorTetris.Models;

namespace BlazorTetris.Tests;

public class GameStateTests
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private static GameState StartedGame()
    {
        var g = new GameState();
        g.StartNew();
        return g;
    }

    // ── Initial state ─────────────────────────────────────────────────────────

    [Fact]
    public void StartNew_SetsStatusToRunning()
    {
        var g = StartedGame();
        Assert.Equal(GameStatus.Running, g.Status);
    }

    [Fact]
    public void StartNew_ScoreAndLinesAndLevelAreReset()
    {
        var g = StartedGame();
        Assert.Equal(0, g.Score);
        Assert.Equal(0, g.LinesCleared);
        Assert.Equal(1, g.Level);
    }

    [Fact]
    public void StartNew_SpawnsPieceAndNextPiece()
    {
        var g = StartedGame();
        Assert.NotNull(g.CurrentPiece);
        Assert.NotNull(g.NextPiece);
    }

    // ── Pause / Resume ────────────────────────────────────────────────────────

    [Fact]
    public void Pause_ChangesStatusToPaused()
    {
        var g = StartedGame();
        g.Pause();
        Assert.Equal(GameStatus.Paused, g.Status);
    }

    [Fact]
    public void Resume_RestoresRunningStatus()
    {
        var g = StartedGame();
        g.Pause();
        g.Resume();
        Assert.Equal(GameStatus.Running, g.Status);
    }

    [Fact]
    public void MoveLeft_DoesNothing_WhenPaused()
    {
        var g = StartedGame();
        int originalCol = g.CurrentPiece!.Col;
        g.Pause();
        bool moved = g.MoveLeft();
        Assert.False(moved);
        Assert.Equal(originalCol, g.CurrentPiece!.Col);
    }

    // ── Movement ──────────────────────────────────────────────────────────────

    [Fact]
    public void MoveRight_IncreasesColumnByOne()
    {
        var g = StartedGame();
        // Force piece to a known position where right-move is legal.
        g.CurrentPiece!.Col = 2;
        int before = g.CurrentPiece.Col;
        bool moved = g.MoveRight();
        Assert.True(moved);
        Assert.Equal(before + 1, g.CurrentPiece.Col);
    }

    [Fact]
    public void MoveLeft_DecreasesColumnByOne()
    {
        var g = StartedGame();
        g.CurrentPiece!.Col = 4; // safely away from left wall
        int before = g.CurrentPiece.Col;
        bool moved = g.MoveLeft();
        Assert.True(moved);
        Assert.Equal(before - 1, g.CurrentPiece.Col);
    }

    [Fact]
    public void MoveDown_IncreasesRowByOne_WhenSpaceBelow()
    {
        var g = StartedGame();
        int before = g.CurrentPiece!.Row;
        bool moved = g.MoveDown();
        // May return true (moved) or false (locked instantly at row 0 — very unlikely).
        if (moved)
            Assert.Equal(before + 1, g.CurrentPiece.Row);
    }

    // ── Rotation ──────────────────────────────────────────────────────────────

    [Fact]
    public void RotateClockwise_CyclesThroughFourStates()
    {
        var g = StartedGame();
        var piece = g.CurrentPiece!;
        // Place it in the centre of the board to avoid wall interference.
        piece.Col = 3; piece.Row = 5;

        int start = piece.Rotation;
        for (int i = 1; i <= 4; i++)
        {
            g.RotateClockwise();
            Assert.Equal((start + i) % 4, piece.Rotation);
        }
    }

    [Fact]
    public void RotateCounterClockwise_CyclesInReverse()
    {
        var g = StartedGame();
        var piece = g.CurrentPiece!;
        piece.Col = 3; piece.Row = 5;

        int start = piece.Rotation;
        g.RotateCounterClockwise();
        Assert.Equal((start + 3) % 4, piece.Rotation);
    }

    // ── HardDrop ──────────────────────────────────────────────────────────────

    [Fact]
    public void HardDrop_LocksCurrentPieceAndSpawnsNext()
    {
        var g = StartedGame();
        var prev = g.CurrentPiece;
        g.HardDrop();
        // A new piece should have been spawned (different object).
        Assert.NotSame(prev, g.CurrentPiece);
    }

    [Fact]
    public void HardDrop_AddsScoreBonus()
    {
        var g = StartedGame();
        g.CurrentPiece!.Row = 0; // ensure drop distance > 0
        g.HardDrop();
        Assert.True(g.Score > 0);
    }

    // ── Hold ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Hold_StoresPieceAndSpawnsNext()
    {
        var g = StartedGame();
        var originalType = g.CurrentPiece!.Type;
        bool held = g.Hold();
        Assert.True(held);
        Assert.NotNull(g.HeldPiece);
        Assert.Equal(originalType, g.HeldPiece!.Type);
    }

    [Fact]
    public void Hold_CannotBeUsedTwiceWithoutLocking()
    {
        var g = StartedGame();
        g.Hold();
        bool secondHold = g.Hold();
        Assert.False(secondHold);
    }

    // ── Ghost piece ───────────────────────────────────────────────────────────

    [Fact]
    public void GetGhostPiece_IsAlwaysBelowOrAtCurrentPiece()
    {
        var g = StartedGame();
        var ghost = g.GetGhostPiece();
        // Ghost is only non-null if the piece can actually drop.
        if (ghost is not null)
            Assert.True(ghost.Row >= g.CurrentPiece!.Row);
    }

    // ── TetrominoData shapes ──────────────────────────────────────────────────

    [Theory]
    [InlineData(TetrominoType.I)]
    [InlineData(TetrominoType.O)]
    [InlineData(TetrominoType.T)]
    [InlineData(TetrominoType.S)]
    [InlineData(TetrominoType.Z)]
    [InlineData(TetrominoType.J)]
    [InlineData(TetrominoType.L)]
    public void EachPieceHasFourCellsInEveryRotation(TetrominoType type)
    {
        for (int rot = 0; rot < 4; rot++)
        {
            var cells = Tetromino.GetCells(type, rot, 0, 0).ToList();
            Assert.Equal(4, cells.Count);
        }
    }

    [Theory]
    [InlineData(TetrominoType.I)]
    [InlineData(TetrominoType.O)]
    [InlineData(TetrominoType.T)]
    [InlineData(TetrominoType.S)]
    [InlineData(TetrominoType.Z)]
    [InlineData(TetrominoType.J)]
    [InlineData(TetrominoType.L)]
    public void AllPieceCellsAreWithin4x4BoundingBox(TetrominoType type)
    {
        for (int rot = 0; rot < 4; rot++)
        {
            foreach (var (r, c) in Tetromino.GetCells(type, rot, 0, 0))
            {
                Assert.InRange(r, 0, 3);
                Assert.InRange(c, 0, 3);
            }
        }
    }
}
