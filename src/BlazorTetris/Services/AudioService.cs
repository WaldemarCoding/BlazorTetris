using Microsoft.JSInterop;

namespace BlazorTetris.Services;

// Thin wrapper so C# can call the JS audio engine without scattering
// JSInterop strings across the codebase.
public sealed class AudioService(IJSRuntime js)
{
    public ValueTask PlayMoveAsync()              => js.InvokeVoidAsync("BlazorTetrisAudio.playMove");
    public ValueTask PlayRotateAsync()            => js.InvokeVoidAsync("BlazorTetrisAudio.playRotate");
    public ValueTask PlayLockAsync()              => js.InvokeVoidAsync("BlazorTetrisAudio.playLock");
    public ValueTask PlayHardDropAsync()          => js.InvokeVoidAsync("BlazorTetrisAudio.playHardDrop");
    public ValueTask PlayLineClearAsync(int lines)=> js.InvokeVoidAsync("BlazorTetrisAudio.playLineClear", lines);
    public ValueTask PlayLevelUpAsync()           => js.InvokeVoidAsync("BlazorTetrisAudio.playLevelUp");
    public ValueTask PlayGameOverAsync()          => js.InvokeVoidAsync("BlazorTetrisAudio.playGameOver");
    public ValueTask StartMusicAsync()            => js.InvokeVoidAsync("BlazorTetrisAudio.startMusic");
    public ValueTask StopMusicAsync()             => js.InvokeVoidAsync("BlazorTetrisAudio.stopMusic");
    public ValueTask<bool> ToggleMuteAsync()      => js.InvokeAsync<bool>("BlazorTetrisAudio.toggleMute");
}
