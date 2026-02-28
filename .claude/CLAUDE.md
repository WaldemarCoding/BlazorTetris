# BlazorTetris — Claude Code Project Guide

## Overview
A fully browser-based Tetris game built with **Blazor WebAssembly (.NET 10)**.
No JavaScript, no canvas — pure Blazor components with CSS grid rendering.
Deployed to **GitHub Pages** via GitHub Actions.

## Repository layout
```
BlazorTetris.sln
src/
  BlazorTetris/           # Blazor WASM app
    Models/               # Pure C# game logic (no Blazor deps)
      TetrominoType.cs    # Enum + CSS class helpers
      Tetromino.cs        # Active piece + static SRS shape data
      GameState.cs        # Board, scoring, line-clearing, piece lifecycle
    Services/
      GameService.cs      # Game loop (PeriodicTimer), event bridge to UI
    Components/
      GameBoard.razor     # 10×20 board renderer + ghost piece
      NextPiecePreview.razor
      ScoreBoard.razor
      KeyBindingsPanel.razor
    Pages/
      Home.razor          # Root page, keyboard handler, layout assembly
    Layout/
      MainLayout.razor
tests/
  BlazorTetris.Tests/     # xUnit tests (no Blazor/WASM deps needed)
docs/
  architecture.md
  gameplay.md
.github/
  workflows/
    build-and-deploy.yml  # CI: build → test → publish to GitHub Pages
```

## Architecture principles
- **Models are pure C#** — no Blazor, no DI, easily unit-tested.
- **GameState** owns all mutable board state and exposes action methods.
- **GameService** owns the game loop timer and wires UI callbacks via `OnStateChanged`.
- **Home.razor** subscribes to `OnStateChanged` and calls `InvokeAsync(StateHasChanged)` for thread-safe re-render.
- Ghost piece is computed on every render from the live game state — no extra storage.

## Build & run
```bash
dotnet run --project src/BlazorTetris        # dev server
dotnet test BlazorTetris.slnx               # all unit tests
dotnet publish src/BlazorTetris -c Release   # production build
```

## Key conventions
- Target framework: **net10.0**
- Nullable enabled, implicit usings enabled
- Cell colours are pure CSS classes (`cell-I`, `cell-O`, …) — no inline styles in Razor
- All Tetromino shapes defined in `TetrominoData.Cells` using SRS offsets inside a 4×4 bounding box
- Spawn column: 3 (centres a 4-wide piece on a 10-wide board)
- Drop interval: `max(100, 1000 − (level−1) × 90)` ms

## Sub-agents
| Agent file | Purpose |
|---|---|
| `.claude/agents/game-logic.md` | Extend or debug game mechanics |
| `.claude/agents/ui-design.md` | Work on Blazor components and CSS |
| `.claude/agents/testing.md` | Write or run unit / integration tests |
