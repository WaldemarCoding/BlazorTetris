# Architecture

## Technology choices

| Layer | Technology | Reason |
|---|---|---|
| Runtime | Blazor WebAssembly (.NET 10) | Runs entirely in the browser; no server required |
| Rendering | HTML + CSS grid | No canvas/JS — pure Blazor component re-renders |
| Game loop | `Task.Delay` loop (async) | Simple, no JS interop needed for timing |
| Styling | Hand-written CSS | Bootstrap removed to keep bundle small |
| Tests | xUnit | Standard .NET testing; no Blazor dependencies needed |
| CI/CD | GitHub Actions | Free for public repos; Pages deployment built-in |

## Component diagram

```
Home.razor  (page / keyboard handler)
│
├── GameBoard.razor          ← renders 10×20 board + ghost
├── NextPiecePreview ×2      ← "Hold" (left) and "Next" (right)
├── ScoreBoard.razor         ← score / level / lines
└── KeyBindingsPanel.razor   ← static control reference
        │
        │ @inject
        ▼
GameService  (singleton-scoped)
│  event OnStateChanged
│  async Task RunLoopAsync()
│
└── GameState  (owns all mutable state)
    ├── int[20,10] _board
    ├── Tetromino? CurrentPiece
    ├── Tetromino? NextPiece
    ├── Tetromino? HeldPiece
    └── int Score / Level / LinesCleared
```

## Data flow

1. **Gravity tick** — `GameService.RunLoopAsync` calls `_state.MoveDown()` at the current level interval, then invokes `OnStateChanged`.
2. **Key press** — `Home.razor` `@onkeydown` handler calls the matching `GameService` action method synchronously; `OnStateChanged` is invoked afterwards.
3. **Re-render** — `OnStateChanged` calls `InvokeAsync(StateHasChanged)` on the page, which triggers a Blazor diff on the entire component tree under `Home.razor`.
4. **Board reads** — `GameBoard` reads `GameState.GetCell(r, c)` and checks `CurrentPiece.GetCells()` + `GetGhostPiece().GetCells()` to compute each cell's CSS class.

## Tetromino shape system

Shapes use the **SRS (Super Rotation System)** conventions stored as `(Row, Col)` offsets from the top-left of a 4×4 bounding box. Each piece has 4 rotation states. The piece's board position (`Row`, `Col`) is added to each offset at render time.

Wall-kick uses a simple column-offset sequence `[0, 1, -1, 2, -2]`; full SRS kick tables can be added to `GameState.TryRotate` without touching any other code.

## Deployment

GitHub Actions builds a Release publish of the Blazor WASM app, sets the `<base href>` to match the GitHub Pages sub-path, and pushes the `wwwroot` artefact to the `gh-pages` branch. A `404.html → index.html` redirect handles client-side routing.
