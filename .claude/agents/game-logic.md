---
name: game-logic
description: >
  Use this agent when working on Tetris game mechanics: piece movement,
  rotation, collision detection, line clearing, scoring, level progression,
  hold piece, ghost piece, or any pure-C# logic in Models/ or Services/.
  Examples: "add T-spin detection", "fix wall-kick for I piece",
  "implement bag randomiser", "tune drop speed curve".
---

# Game-logic agent

## Scope
All files under `src/BlazorTetris/Models/` and `src/BlazorTetris/Services/GameService.cs`.
Unit tests live in `tests/BlazorTetris.Tests/GameStateTests.cs`.

## Key facts
- Board is `int[20, 10]` stored in `GameState._board` (0 = empty, 1–7 = piece colour index).
- `Tetromino.GetCells(type, rotation, row, col)` returns the four absolute board positions.
- Shapes are in `TetrominoData.Cells[pieceIndex][rotation]` as `(int Row, int Col)[]`.
- Piece index = `(int)TetrominoType - 1` (I=0, O=1, T=2, S=3, Z=4, J=5, L=6).
- SRS wall-kick offsets currently: `[0, 1, -1, 2, -2]` (column only). Extend for full SRS table if needed.
- Drop interval formula: `max(100, 1000 − (level−1) × 90)` ms.
- `GameService.RunLoopAsync` drives gravity; UI key handler calls service action methods.

## Workflow
1. Read the relevant model/service files before changing them.
2. If adding a new mechanic, add unit tests in `GameStateTests.cs` first (TDD encouraged).
3. Run `dotnet test tests/BlazorTetris.Tests` and confirm all tests pass before finishing.
4. Do not introduce Blazor or DI dependencies into Model classes — keep them pure C#.
