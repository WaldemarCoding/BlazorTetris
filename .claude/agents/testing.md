---
name: testing
description: >
  Use this agent when writing, running, or debugging unit tests for the game.
  Examples: "add tests for line-clear scoring", "test that game-over fires correctly",
  "check test coverage", "debug a failing test".
---

# Testing agent

## Scope
`tests/BlazorTetris.Tests/GameStateTests.cs` and any new test files in that project.

## Test stack
- **xUnit 2.x** — `[Fact]` for single cases, `[Theory] + [InlineData]` for parameterised.
- Test project references the main project directly — no mocking framework needed
  because `GameState` and `Tetromino` are pure C# with no external dependencies.
- Run tests: `dotnet test tests/BlazorTetris.Tests`
- Run with coverage: `dotnet test --collect:"XPlat Code Coverage"`

## Conventions
- Group tests by the feature/method under test using nested classes or clear naming.
- Helper `StartedGame()` gives you a fresh `GameState` in `Running` status.
- Force piece position before testing movement to avoid randomness affecting assertions.
- Do **not** test timing (the `GameService` loop) — that belongs in integration tests.
- All 30+ existing tests must remain green after any change.

## Useful patterns
```csharp
// Get a started game
var g = new GameState();
g.StartNew();

// Pin piece location to make tests deterministic
g.CurrentPiece!.Col = 4;
g.CurrentPiece!.Row = 10;

// Assert board cell was locked
Assert.NotEqual(0, g.GetCell(row, col));
```
