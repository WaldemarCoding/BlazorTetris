# Blazor Tetris

A fully browser-based Tetris game built with **Blazor WebAssembly (.NET 10)**.
No JavaScript, no canvas — pure Blazor components with CSS grid rendering.

## Play

> Once deployed: `https://<your-username>.github.io/blazorTetris/`

## Controls

| Key | Action |
|---|---|
| `←` `→` | Move |
| `↑` | Rotate CW |
| `Z` | Rotate CCW |
| `↓` | Soft drop |
| `Space` | Hard drop |
| `C` | Hold |
| `P` | Pause |

## Local development

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
git clone https://github.com/<you>/blazorTetris.git
cd blazorTetris

# Dev server with hot reload
dotnet run --project src/BlazorTetris

# Run all unit tests
dotnet test

# Production build
dotnet publish src/BlazorTetris -c Release
```

## Repository structure

```
BlazorTetris.sln
src/
  BlazorTetris/           # Blazor WASM app
    Models/               # Pure C# game logic (no Blazor deps)
    Services/             # Game loop + event bridge to UI
    Components/           # Razor UI components
    Pages/                # Routed pages
    wwwroot/              # Static assets + CSS
tests/
  BlazorTetris.Tests/     # xUnit unit tests (30 tests)
docs/
  architecture.md
  gameplay.md
.claude/
  CLAUDE.md               # Claude Code project guide
  agents/                 # Specialised sub-agents
.github/
  workflows/              # CI: build + test + deploy to Pages
```

## CI / CD

Every push to `main`: build → test → deploy to GitHub Pages.
Pull requests: build + test only.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

## License

MIT
