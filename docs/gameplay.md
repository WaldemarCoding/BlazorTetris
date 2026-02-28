# Gameplay reference

## Controls

| Key | Action |
|---|---|
| `←` / `→` | Move piece left / right |
| `↑` | Rotate clockwise |
| `Z` | Rotate counter-clockwise |
| `↓` | Soft drop (one row) |
| `Space` | Hard drop (instant) |
| `C` | Hold current piece |
| `P` | Pause / resume |

## Scoring

| Lines cleared | Points |
|---|---|
| 1 (Single) | 100 × level |
| 2 (Double) | 300 × level |
| 3 (Triple) | 500 × level |
| 4 (Tetris) | 800 × level |
| Hard drop bonus | 2 × rows dropped |

## Level progression

Level increases by 1 for every **10 lines** cleared (level = `floor(lines / 10) + 1`).

## Drop speed

| Level | Interval |
|---|---|
| 1 | 1000 ms |
| 2 | 910 ms |
| 5 | 640 ms |
| 10 | 190 ms |
| 11+ | 100 ms (minimum) |

Formula: `max(100, 1000 − (level − 1) × 90)` ms.

## Pieces

| Name | Colour | Shape |
|---|---|---|
| I | Cyan | Straight line (4) |
| O | Yellow | 2×2 square |
| T | Purple | T-shape |
| S | Green | S-skew |
| Z | Red | Z-skew |
| J | Blue | J-shape |
| L | Orange | L-shape |

## Ghost piece

A translucent preview shows where the current piece will land if dropped instantly. This helps planning without obscuring the board.

## Hold

Press `C` to store the current piece. The held piece can be swapped in again at any time, but only once per lock (you cannot hold the same piece twice without locking another first).

## Game over

The game ends when a newly spawned piece cannot be placed at the spawn position (rows 0–1, columns 3–6).
