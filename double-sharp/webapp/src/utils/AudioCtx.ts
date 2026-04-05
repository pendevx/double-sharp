import singleton from "./singleton";

const binsPerOctaveEl = "24"; // 12 - 36. 48+ eats CPU
const smoothingEl = "0.85"; // responsive visualizer 0.5-0.7, smooth ui 0.7-0.85
const minFreqEl = "130.81"; // full music 60-80, cleaner+faster 100-130, reasonable UI 130.81
const maxFreqEl = "16744"; // no more than sampleRate / 2. 12k-16k most musical stuff, 16k-18k full audible. no more than 18k

/*
PRESETS

Balanced Visualizer
binsPerOctave = 24;
smoothing = 0.65;
minFreq = 80;
maxFreq = 14000;

Musical-focused
binsPerOctave = 12;
smoothing = 0.7;
minFreq = 65;   // C2
maxFreq = 8000;

High-detail
binsPerOctave = 36;
smoothing = 0.75;
minFreq = 60;
maxFreq = 16000;

Performance friendly
binsPerOctave = 12;
smoothing = 0.6;
minFreq = 120;
maxFreq = 10000;
*/

// ============================================================
// CQT Kernel Builder
// ============================================================
// For each CQT bin k with center frequency f_k:
//   Q = 1 / (2^(1/B) - 1)          where B = bins per octave
//   N_k = round(Q * sr / f_k)       window length for bin k
//   kernel[k][n] = hann(n, N_k) * e^(-j * 2pi * f_k * n / sr) / N_k
//
// We precompute cos/sin tables so the per-frame work is just
// dot-products between the time-domain buffer and the kernel.
// ============================================================

type TBin = {
    freq: number;
    Nk: number;
    cosTable: Float32Array<ArrayBuffer>;
    sinTable: Float32Array<ArrayBuffer>;
};

function buildCQTKernel(sampleRate: number, fftSize: number): TBin[] {
    const binsPerOctave = parseInt(binsPerOctaveEl);
    const fMin = parseFloat(minFreqEl);
    const fMax = parseFloat(maxFreqEl);
    const Q = 1 / (Math.pow(2, 1 / binsPerOctave) - 1);

    const numBins = Math.round(binsPerOctave * Math.log2(fMax / fMin));
    const bins = [];

    for (let k = 0; k < numBins; k++) {
        const freq = fMin * Math.pow(2, k / binsPerOctave);
        if (freq > sampleRate / 2) break; // Nyquist

        let Nk = Math.round((Q * sampleRate) / freq);
        // Clamp to available buffer length
        if (Nk > fftSize) Nk = fftSize;
        // Minimum window: at least 2 full cycles
        const minN = Math.ceil((2 * sampleRate) / freq);
        if (Nk < minN) Nk = minN;
        if (Nk > fftSize) Nk = fftSize;

        // Precompute windowed twiddle factors
        const cosTable = new Float32Array(Nk);
        const sinTable = new Float32Array(Nk);
        const omega = (2 * Math.PI * freq) / sampleRate;

        for (let n = 0; n < Nk; n++) {
            // Hann window
            const w = 0.5 * (1 - Math.cos((2 * Math.PI * n) / (Nk - 1 || 1)));
            cosTable[n] = (w * Math.cos(omega * n)) / Nk;
            sinTable[n] = (w * Math.sin(omega * n)) / Nk;
        }

        bins.push({ freq, Nk, cosTable, sinTable });
    }

    return bins;
}

function computeCQT(timeDomainData: Float32Array, kernel: any) {
    const magnitudes = new Float32Array(kernel.length);

    for (let k = 0; k < kernel.length; k++) {
        const { Nk, cosTable, sinTable } = kernel[k];
        let re = 0,
            im = 0;

        // Use the most recent Nk samples from the buffer
        const offset = timeDomainData.length - Nk;
        for (let n = 0; n < Nk; n++) {
            const sample = timeDomainData[offset + n];
            re += sample * cosTable[n];
            im += sample * sinTable[n];
        }

        magnitudes[k] = Math.sqrt(re * re + im * im);
    }

    return magnitudes;
}

export interface IDoublesharpAudioCtx {
    cqtKernel: TBin[];
    getFreqs(): Float32Array;
    setVolume(volume: number): void;
}

class DoublesharpAudioCtx implements IDoublesharpAudioCtx {
    public cqtKernel: TBin[];
    private audioCtx: AudioContext;
    private analyzer: AnalyserNode;
    private gain: GainNode;
    public smoothedMagnitudes: Float32Array<ArrayBuffer>;

    constructor(audio: HTMLAudioElement) {
        this.audioCtx = new AudioContext();
        const input = this.audioCtx.createMediaElementSource(audio);

        this.analyzer = this.audioCtx.createAnalyser();
        this.analyzer.fftSize = 16384;
        this.analyzer.smoothingTimeConstant = 0.75;

        this.cqtKernel = buildCQTKernel(this.audioCtx.sampleRate, this.analyzer.fftSize);
        this.smoothedMagnitudes = new Float32Array(this.cqtKernel.length);

        this.gain = this.audioCtx.createGain();
        this.gain.gain.setValueAtTime(1, this.audioCtx.currentTime);

        input.connect(this.analyzer).connect(this.gain).connect(this.audioCtx.destination);
    }

    getFreqs(): Float32Array {
        const freqsArr = new Float32Array(this.analyzer.fftSize);
        this.analyzer.getFloatTimeDomainData(freqsArr);

        const rawMag = computeCQT(freqsArr, this.cqtKernel);
        const alpha = parseFloat(smoothingEl);

        for (let k = 0; k < rawMag.length; k++) {
            this.smoothedMagnitudes[k] = alpha * this.smoothedMagnitudes[k] + (1 - alpha) * rawMag[k];
        }

        return this.smoothedMagnitudes;
    }

    setVolume(volume: number): void {
        this.gain.gain.setValueAtTime(volume, this.audioCtx.currentTime);
    }
}

export default singleton(DoublesharpAudioCtx);
