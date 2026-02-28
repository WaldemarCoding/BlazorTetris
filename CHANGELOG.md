# Changelog

All notable changes to this project will be documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

## [0.1.0] — 2026-02-28

### Added
- Full Tetris game loop: gravity, locking, line clearing, scoring, level progression.
- All seven standard tetrominoes with SRS rotation (I, O, T, S, Z, J, L).
- Ghost piece (landing preview).
- Hold piece mechanic (one swap per lock).
- Keyboard controls: move, rotate CW/CCW, soft drop, hard drop, hold, pause.
- Start / pause / game-over overlays.
- Score panel (score, level, lines cleared).
- Next piece and hold piece previews.
- 30 xUnit unit tests covering game state, movement, rotation, scoring, and shape data.
- GitHub Actions CI/CD: build → test → deploy to GitHub Pages on push to `main`.
- Claude Code sub-agents for game logic, UI design, and testing.
- Architecture and gameplay documentation.
