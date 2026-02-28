using BlazorTetris.Models;

namespace BlazorTetris.Services;

/// <summary>
/// Orchestrates the game loop and exposes the game state + actions to the UI.
/// Uses a PeriodicTimer that adjusts interval based on the current level.
/// </summary>
public sealed class GameService : IAsyncDisposable
{
    private readonly GameState _state = new();
    private CancellationTokenSource? _cts;
    private Task? _loopTask;

    // ── Public state (read-only facade) ───────────────────────────────────────

    public GameState State => _state;

    /// <summary>Raised after every state change so the UI can re-render.</summary>
    public event Action? OnStateChanged;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    public async Task StartNewGameAsync()
    {
        await StopLoopAsync();
        _state.StartNew();
        NotifyChanged();
        StartLoop();
    }

    public void PauseOrResume()
    {
        if (_state.Status == GameStatus.Running)
            _state.Pause();
        else if (_state.Status == GameStatus.Paused)
            _state.Resume();

        NotifyChanged();
    }

    // ── Input actions (called from UI key handlers) ───────────────────────────

    public void MoveLeft()     { _state.MoveLeft();             NotifyChanged(); }
    public void MoveRight()    { _state.MoveRight();            NotifyChanged(); }
    public void SoftDrop()     { _state.MoveDown();             NotifyChanged(); }
    public void HardDrop()     { _state.HardDrop();             NotifyChanged(); }
    public void RotateCW()     { _state.RotateClockwise();      NotifyChanged(); }
    public void RotateCCW()    { _state.RotateCounterClockwise(); NotifyChanged(); }
    public void Hold()         { _state.Hold();                 NotifyChanged(); }

    // ── Game loop ─────────────────────────────────────────────────────────────

    private void StartLoop()
    {
        _cts = new CancellationTokenSource();
        _loopTask = RunLoopAsync(_cts.Token);
    }

    private async Task RunLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var interval = GetDropInterval(_state.Level);
            await Task.Delay(interval, ct).ConfigureAwait(false);

            if (ct.IsCancellationRequested) break;

            if (_state.Status == GameStatus.Running)
            {
                _state.MoveDown();
                NotifyChanged();
            }

            if (_state.Status == GameStatus.GameOver)
                break;
        }
    }

    private static int GetDropInterval(int level) =>
        Math.Max(100, 1000 - (level - 1) * 90);

    private async Task StopLoopAsync()
    {
        if (_cts is not null)
        {
            await _cts.CancelAsync();
            if (_loopTask is not null)
                await _loopTask.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            _cts.Dispose();
            _cts = null;
        }
    }

    private void NotifyChanged() => OnStateChanged?.Invoke();

    public async ValueTask DisposeAsync()
    {
        await StopLoopAsync();
    }
}
