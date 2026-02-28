namespace BlazorTetris.Models;

public enum GameStatus { Idle, Running, Paused, GameOver }

public record struct LinesClearedResult(int LinesCleared, int ScoreGained);

/// <summary>
/// Holds all mutable game state and exposes pure operations on it.
/// The GameService orchestrates timing and user input; this class owns the board logic.
/// </summary>
public class GameState
{
    public const int Rows = 20;
    public const int Cols = 10;

    // Board stores the color index (0=empty, 1–7=piece type).
    private readonly int[,] _board = new int[Rows, Cols];

    public Tetromino? CurrentPiece { get; private set; }
    public Tetromino? NextPiece { get; private set; }
    public Tetromino? HeldPiece { get; private set; }

    public int Score { get; private set; }
    public int Level { get; private set; } = 1;
    public int LinesCleared { get; private set; }
    public GameStatus Status { get; private set; } = GameStatus.Idle;
    public bool CanHold { get; private set; } = true;

    private static readonly Random _rng = new();

    // ── Read-only board access ────────────────────────────────────────────────

    public int GetCell(int row, int col) => _board[row, col];

    /// <summary>
    /// Computes the ghost piece — the furthest-down position of the current piece.
    /// Returns null if there is no current piece.
    /// </summary>
    public Tetromino? GetGhostPiece()
    {
        if (CurrentPiece is null) return null;

        var ghost = CurrentPiece.Clone();
        while (CanPlace(ghost.Type, ghost.Rotation, ghost.Row + 1, ghost.Col))
            ghost.Row++;

        return ghost.Row == CurrentPiece.Row ? null : ghost;
    }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    public void StartNew()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                _board[r, c] = 0;

        Score = 0;
        Level = 1;
        LinesCleared = 0;
        CanHold = true;
        HeldPiece = null;
        Status = GameStatus.Running;

        NextPiece = SpawnRandom();
        SpawnNext();
    }

    public void Pause()
    {
        if (Status == GameStatus.Running) Status = GameStatus.Paused;
    }

    public void Resume()
    {
        if (Status == GameStatus.Paused) Status = GameStatus.Running;
    }

    // ── Input actions ─────────────────────────────────────────────────────────

    public bool MoveLeft()
    {
        if (CurrentPiece is null || Status != GameStatus.Running) return false;
        if (!CanPlace(CurrentPiece.Type, CurrentPiece.Rotation, CurrentPiece.Row, CurrentPiece.Col - 1))
            return false;
        CurrentPiece.Col--;
        return true;
    }

    public bool MoveRight()
    {
        if (CurrentPiece is null || Status != GameStatus.Running) return false;
        if (!CanPlace(CurrentPiece.Type, CurrentPiece.Rotation, CurrentPiece.Row, CurrentPiece.Col + 1))
            return false;
        CurrentPiece.Col++;
        return true;
    }

    /// <summary>Moves piece down one row. Returns false if it locked.</summary>
    public bool MoveDown()
    {
        if (CurrentPiece is null || Status != GameStatus.Running) return false;
        if (CanPlace(CurrentPiece.Type, CurrentPiece.Rotation, CurrentPiece.Row + 1, CurrentPiece.Col))
        {
            CurrentPiece.Row++;
            return true;
        }
        LockPiece();
        return false;
    }

    /// <summary>Instantly drops the piece to the lowest valid position.</summary>
    public int HardDrop()
    {
        if (CurrentPiece is null || Status != GameStatus.Running) return 0;
        int dropped = 0;
        while (CanPlace(CurrentPiece.Type, CurrentPiece.Rotation, CurrentPiece.Row + 1, CurrentPiece.Col))
        {
            CurrentPiece.Row++;
            dropped++;
        }
        Score += dropped * 2;
        LockPiece();
        return dropped;
    }

    public bool RotateClockwise()
    {
        if (CurrentPiece is null || Status != GameStatus.Running) return false;
        int nextRot = (CurrentPiece.Rotation + 1) % 4;
        return TryRotate(nextRot);
    }

    public bool RotateCounterClockwise()
    {
        if (CurrentPiece is null || Status != GameStatus.Running) return false;
        int nextRot = (CurrentPiece.Rotation + 3) % 4;
        return TryRotate(nextRot);
    }

    public bool Hold()
    {
        if (CurrentPiece is null || !CanHold || Status != GameStatus.Running) return false;

        var held = HeldPiece;
        HeldPiece = new Tetromino
        {
            Type = CurrentPiece.Type,
            Row = TetrominoData.SpawnRow,
            Col = TetrominoData.SpawnCol,
            Rotation = 0,
        };

        CanHold = false;

        if (held is null)
        {
            SpawnNext();
        }
        else
        {
            CurrentPiece = new Tetromino
            {
                Type = held.Type,
                Row = TetrominoData.SpawnRow,
                Col = TetrominoData.SpawnCol,
                Rotation = 0,
            };

            if (!CanPlace(CurrentPiece.Type, CurrentPiece.Rotation, CurrentPiece.Row, CurrentPiece.Col))
                TriggerGameOver();
        }
        return true;
    }

    // ── Internal helpers ──────────────────────────────────────────────────────

    private bool TryRotate(int nextRot)
    {
        if (CurrentPiece is null) return false;

        // Try the basic rotation, then wall-kick offsets.
        int[] kicks = [0, 1, -1, 2, -2];
        foreach (int kick in kicks)
        {
            if (CanPlace(CurrentPiece.Type, nextRot, CurrentPiece.Row, CurrentPiece.Col + kick))
            {
                CurrentPiece.Rotation = nextRot;
                CurrentPiece.Col += kick;
                return true;
            }
        }
        return false;
    }

    private bool CanPlace(TetrominoType type, int rotation, int row, int col)
    {
        foreach (var (r, c) in Tetromino.GetCells(type, rotation, row, col))
        {
            if (r < 0 || r >= Rows || c < 0 || c >= Cols) return false;
            if (_board[r, c] != 0) return false;
        }
        return true;
    }

    private void LockPiece()
    {
        if (CurrentPiece is null) return;

        foreach (var (r, c) in CurrentPiece.GetCells())
            _board[r, c] = (int)CurrentPiece.Type;

        var result = ClearLines();
        Score += result.ScoreGained;
        LinesCleared += result.LinesCleared;
        Level = Math.Max(1, LinesCleared / 10 + 1);
        CanHold = true;

        SpawnNext();
    }

    private LinesClearedResult ClearLines()
    {
        int cleared = 0;
        for (int r = Rows - 1; r >= 0; r--)
        {
            if (IsRowFull(r))
            {
                RemoveRow(r);
                r++; // recheck same index after shift
                cleared++;
            }
        }

        int score = cleared switch
        {
            1 => 100 * Level,
            2 => 300 * Level,
            3 => 500 * Level,
            4 => 800 * Level,
            _ => 0,
        };

        return new(cleared, score);
    }

    private bool IsRowFull(int row)
    {
        for (int c = 0; c < Cols; c++)
            if (_board[row, c] == 0) return false;
        return true;
    }

    private void RemoveRow(int rowToRemove)
    {
        for (int r = rowToRemove; r > 0; r--)
            for (int c = 0; c < Cols; c++)
                _board[r, c] = _board[r - 1, c];

        for (int c = 0; c < Cols; c++)
            _board[0, c] = 0;
    }

    private void SpawnNext()
    {
        CurrentPiece = NextPiece;
        NextPiece = SpawnRandom();

        if (CurrentPiece is not null &&
            !CanPlace(CurrentPiece.Type, CurrentPiece.Rotation, CurrentPiece.Row, CurrentPiece.Col))
        {
            TriggerGameOver();
        }
    }

    private static Tetromino SpawnRandom()
    {
        var type = (TetrominoType)_rng.Next(1, 8);
        return new Tetromino
        {
            Type = type,
            Row = TetrominoData.SpawnRow,
            Col = TetrominoData.SpawnCol,
            Rotation = 0,
        };
    }

    private void TriggerGameOver()
    {
        Status = GameStatus.GameOver;
        CurrentPiece = null;
    }
}
