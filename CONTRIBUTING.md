# Contributing

## Getting started

1. Fork the repo and clone it locally.
2. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download).
3. Run `dotnet run --project src/BlazorTetris` to start the dev server.
4. Run `dotnet test` to verify all tests pass.

## Branch strategy

| Branch | Purpose |
|---|---|
| `main` | Stable, auto-deployed to GitHub Pages |
| `feature/<name>` | New features |
| `fix/<name>` | Bug fixes |

## Pull requests

- Keep PRs focused — one feature or fix per PR.
- All tests must pass (`dotnet test`).
- The build must succeed (`dotnet build`).
- Add or update tests in `tests/BlazorTetris.Tests/` for any logic changes.

## Code conventions

- **Models** (`src/BlazorTetris/Models/`) must remain pure C# — no Blazor or DI dependencies.
- CSS classes for cell colours follow the pattern `cell-<PieceType>` (e.g. `cell-I`, `cell-T`).
- Tetromino shapes are defined exclusively in `TetrominoData.Cells` — do not hardcode shapes elsewhere.
- Use the sub-agents in `.claude/agents/` when working with Claude Code on specific areas.

## Running locally

```bash
dotnet run --project src/BlazorTetris     # http://localhost:5000
dotnet test                               # all unit tests
dotnet build                              # quick compile check
```
