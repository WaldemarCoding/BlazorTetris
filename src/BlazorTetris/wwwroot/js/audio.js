'use strict';

// All sound is generated via Web Audio API — no audio files needed.
// Exposed as window.BlazorTetrisAudio for C# JSInterop calls.
(function () {
    let ctx = null;
    let muted = false;
    let musicActive = false;
    let musicTimer = null;

    function getCtx() {
        if (!ctx) ctx = new (window.AudioContext || window.webkitAudioContext)();
        // Browsers suspend AudioContext until a user gesture — resume it here.
        if (ctx.state === 'suspended') ctx.resume();
        return ctx;
    }

    // Core oscillator helper.
    // freq/endFreq: Hz, type: OscillatorType, vol: 0–1, startTime/dur: seconds
    function osc(freq, type, startTime, dur, vol, endFreq) {
        const c = getCtx();
        const o = c.createOscillator();
        const g = c.createGain();
        o.connect(g); g.connect(c.destination);
        o.type = type;
        o.frequency.setValueAtTime(freq, startTime);
        if (endFreq) o.frequency.exponentialRampToValueAtTime(endFreq, startTime + dur);
        g.gain.setValueAtTime(0, startTime);
        g.gain.linearRampToValueAtTime(vol, startTime + 0.01);
        g.gain.exponentialRampToValueAtTime(0.001, startTime + dur);
        o.start(startTime);
        o.stop(startTime + dur + 0.05);
    }

    // ── Tetris theme (Korobeiniki, full: A + B) ──────────────────────────────
    // B = quarter-note at ~188 BPM. [frequency Hz, duration s]. 0 Hz = rest.
    const B = 0.32;

    const PART_A = [
        [659.25,B],    [493.88,B/2],  [523.25,B/2],  [587.33,B],    [523.25,B/2],  [493.88,B/2],
        [440.00,B],    [440.00,B/2],  [523.25,B/2],  [659.25,B],    [587.33,B/2],  [523.25,B/2],
        [493.88,B*1.5],[523.25,B/2],  [587.33,B],    [659.25,B],
        [523.25,B],    [440.00,B],    [440.00,B*2],
        [0,B/2],
        [587.33,B],    [698.46,B/2],  [880.00,B],    [783.99,B/2],  [698.46,B/2],
        [659.25,B*1.5],[523.25,B/2],  [659.25,B],    [587.33,B/2],  [523.25,B/2],
        [493.88,B],    [493.88,B/2],  [523.25,B/2],  [587.33,B],    [659.25,B],
        [523.25,B],    [440.00,B],    [440.00,B*2],
    ];

    // Part B — bridge (slower, lower register)
    const PART_B = [
        [329.63,B*2],  [261.63,B*2],
        [293.66,B*2],  [246.94,B*2],
        [261.63,B*2],  [220.00,B*2],
        [207.65,B*2],  [246.94,B*2],
        [329.63,B*2],  [261.63,B*2],
        [293.66,B*2],  [246.94,B*2],
        [261.63,B],    [329.63,B],    [440.00,B*2],
        [415.30,B*2],  [0,B*2],
    ];

    const THEME = [...PART_A, ...PART_B];

    function scheduleTheme() {
        if (!musicActive || muted) return;
        const c = getCtx();
        let t = c.currentTime + 0.05;
        let total = 0;
        for (const [freq, dur] of THEME) {
            if (freq > 0) osc(freq, 'square', t, dur * 0.82, 0.12);
            t += dur; total += dur;
        }
        musicTimer = setTimeout(scheduleTheme, total * 1000 - 100);
    }

    // ── Public API ────────────────────────────────────────────────────────────
    window.BlazorTetrisAudio = {
        playMove() {
            if (muted) return;
            const t = getCtx().currentTime;
            osc(200, 'square', t, 0.04, 0.12);
        },

        playRotate() {
            if (muted) return;
            const t = getCtx().currentTime;
            osc(380, 'triangle', t, 0.07, 0.18, 560);
        },

        playLock() {
            if (muted) return;
            const t = getCtx().currentTime;
            osc(160, 'sawtooth', t, 0.10, 0.22, 80);
        },

        playHardDrop() {
            if (muted) return;
            const t = getCtx().currentTime;
            osc(280, 'sawtooth', t, 0.04, 0.30, 80);
            osc(140, 'square',   t + 0.04, 0.08, 0.18);
        },

        playLineClear(lines) {
            if (muted) return;
            const t = getCtx().currentTime;
            if (lines >= 4) {
                // Tetris! — ascending fanfare
                [523.25, 659.25, 783.99, 1046.50].forEach((f, i) =>
                    osc(f, 'square', t + i * 0.07, 0.28, 0.35));
            } else {
                [523.25, 659.25, 783.99].slice(0, lines).forEach((f, i) =>
                    osc(f, 'square', t + i * 0.09, 0.18, 0.30));
            }
        },

        playLevelUp() {
            if (muted) return;
            const t = getCtx().currentTime;
            [261.63, 329.63, 392.00, 523.25, 659.25].forEach((f, i) =>
                osc(f, 'triangle', t + i * 0.06, 0.14, 0.28));
        },

        playGameOver() {
            if (muted) return;
            const t = getCtx().currentTime;
            [392.00, 349.23, 329.63, 293.66, 261.63, 220.00].forEach((f, i) =>
                osc(f, 'sawtooth', t + i * 0.18, 0.22, 0.25));
        },

        startMusic() {
            if (musicActive) return;
            musicActive = true;
            scheduleTheme();
        },

        stopMusic() {
            musicActive = false;
            if (musicTimer) { clearTimeout(musicTimer); musicTimer = null; }
        },

        toggleMute() {
            muted = !muted;
            if (muted) {
                this.stopMusic();
            } else {
                musicActive = true;
                scheduleTheme();
            }
            return muted;
        },

        isMuted() { return muted; },
    };
})();
