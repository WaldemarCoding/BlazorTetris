namespace BlazorTetris.Models;

public enum TetrominoType
{
    I = 1,
    O = 2,
    T = 3,
    S = 4,
    Z = 5,
    J = 6,
    L = 7,
}

public static class TetrominoColors
{
    public static string GetCssClass(int colorIndex) => colorIndex switch
    {
        1 => "cell-I",
        2 => "cell-O",
        3 => "cell-T",
        4 => "cell-S",
        5 => "cell-Z",
        6 => "cell-J",
        7 => "cell-L",
        _ => "cell-empty",
    };
}
