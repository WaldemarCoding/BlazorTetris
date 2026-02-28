---
name: ui-design
description: >
  Use this agent when working on Blazor components, CSS styling, layout,
  animations, or the HTML/render output of the game. Examples: "add a dark
  glow to the active piece", "make the board responsive on mobile",
  "add a high-score table component", "animate line-clear with CSS keyframes".
---

# UI-design agent

## Scope
- `src/BlazorTetris/Components/` — Razor components
- `src/BlazorTetris/Pages/Home.razor` — main page
- `src/BlazorTetris/Layout/` — layout shell
- `src/BlazorTetris/wwwroot/css/app.css` — all custom styles

## Key facts
- Board is a CSS grid: 20 rows × 10 cols, each cell `28 × 28 px` (`board-cell` class).
- Cell colours are CSS classes: `cell-I … cell-L`, `cell-empty`, `cell-ghost`.
- No Bootstrap usage (it was removed from `index.html`). All styles live in `app.css`.
- `GameBoard.razor` loops over all 20×10 cells and calls `GetCellClass(r, c)` to
  determine priority: active piece → ghost → locked board.
- Piece previews use a 4×4 grid at `18 × 18 px` per cell (`piece-cell`).
- Overlays (start/pause/game-over) use `position: absolute; inset: 0` over `.board-wrapper`.
- `.game-container` is a flex row: left sidebar | board | right sidebar.

## Workflow
1. Read the component/CSS file before modifying it.
2. Prefer CSS-only animations over JavaScript.
3. Verify the build still compiles after changes: `dotnet build src/BlazorTetris`.
4. For new components, add them to `_Imports.razor` if the namespace isn't already imported.
5. CSS scoped to a component can use a `.razor.css` sibling file (Blazor CSS isolation).
