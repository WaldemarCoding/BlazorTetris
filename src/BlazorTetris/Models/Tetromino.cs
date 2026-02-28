namespace BlazorTetris.Models;

/// <summary>
/// Represents an active tetromino piece with its position and rotation state.
/// All shapes are defined in a 4×4 local grid; cells are (row, col) offsets from the piece's top-left origin.
/// </summary>
public class Tetromino
{
    public TetrominoType Type { get; init; }
    public int Row { get; set; }
    public int Col { get; set; }
    public int Rotation { get; set; }

    /// <summary>
    /// Returns the board cells occupied by this piece at its current position.
    /// </summary>
    public IEnumerable<(int Row, int Col)> GetCells()
    {
        var offsets = TetrominoData.Cells[(int)Type - 1][Rotation];
        return offsets.Select(o => (Row + o.Row, Col + o.Col));
    }

    /// <summary>
    /// Returns the board cells for a hypothetical position/rotation.
    /// </summary>
    public static IEnumerable<(int Row, int Col)> GetCells(
        TetrominoType type, int rotation, int row, int col)
    {
        var offsets = TetrominoData.Cells[(int)type - 1][rotation % 4];
        return offsets.Select(o => (row + o.Row, col + o.Col));
    }

    public Tetromino Clone() => new()
    {
        Type = Type,
        Row = Row,
        Col = Col,
        Rotation = Rotation,
    };
}

/// <summary>
/// Static shape data for all seven tetrominoes across four rotation states each.
/// Shapes use SRS (Super Rotation System) conventions and fit within a 4×4 bounding box.
/// </summary>
public static class TetrominoData
{
    // [pieceIndex][rotation] -> array of (row, col) cell offsets
    public static readonly (int Row, int Col)[][][] Cells =
    [
        // I — cyan
        [
            [(1, 0), (1, 1), (1, 2), (1, 3)],
            [(0, 2), (1, 2), (2, 2), (3, 2)],
            [(2, 0), (2, 1), (2, 2), (2, 3)],
            [(0, 1), (1, 1), (2, 1), (3, 1)],
        ],
        // O — yellow
        [
            [(0, 1), (0, 2), (1, 1), (1, 2)],
            [(0, 1), (0, 2), (1, 1), (1, 2)],
            [(0, 1), (0, 2), (1, 1), (1, 2)],
            [(0, 1), (0, 2), (1, 1), (1, 2)],
        ],
        // T — purple
        [
            [(0, 1), (1, 0), (1, 1), (1, 2)],
            [(0, 1), (1, 1), (1, 2), (2, 1)],
            [(1, 0), (1, 1), (1, 2), (2, 1)],
            [(0, 1), (1, 0), (1, 1), (2, 1)],
        ],
        // S — green
        [
            [(0, 1), (0, 2), (1, 0), (1, 1)],
            [(0, 1), (1, 1), (1, 2), (2, 2)],
            [(1, 1), (1, 2), (2, 0), (2, 1)],
            [(0, 0), (1, 0), (1, 1), (2, 1)],
        ],
        // Z — red
        [
            [(0, 0), (0, 1), (1, 1), (1, 2)],
            [(0, 2), (1, 1), (1, 2), (2, 1)],
            [(1, 0), (1, 1), (2, 1), (2, 2)],
            [(0, 1), (1, 0), (1, 1), (2, 0)],
        ],
        // J — blue
        [
            [(0, 0), (1, 0), (1, 1), (1, 2)],
            [(0, 1), (0, 2), (1, 1), (2, 1)],
            [(1, 0), (1, 1), (1, 2), (2, 2)],
            [(0, 1), (1, 1), (2, 0), (2, 1)],
        ],
        // L — orange
        [
            [(0, 2), (1, 0), (1, 1), (1, 2)],
            [(0, 1), (1, 1), (2, 1), (2, 2)],
            [(1, 0), (1, 1), (1, 2), (2, 0)],
            [(0, 0), (0, 1), (1, 1), (2, 1)],
        ],
    ];

    /// <summary>Spawn column: centres a 4-wide piece on a 10-wide board.</summary>
    public const int SpawnCol = 3;
    public const int SpawnRow = 0;
}
