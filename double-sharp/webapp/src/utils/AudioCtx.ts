import singleton from "./singleton";

export interface IDoublesharpAudioCtx {
    getFreqs(): Uint8Array;
    setVolume(volume: number): void;
}

class DoublesharpAudioCtx implements IDoublesharpAudioCtx {
    private audioCtx: AudioContext;
    private analyzer: AnalyserNode;
    private gain: GainNode;
    private freqsArr: Uint8Array<ArrayBuffer>;

    constructor(audio: HTMLAudioElement) {
        this.audioCtx = new AudioContext();
        const input = this.audioCtx.createMediaElementSource(audio);

        this.analyzer = this.audioCtx.createAnalyser();
        this.analyzer.fftSize = 1024;
        this.analyzer.smoothingTimeConstant = 0.75;

        const count = this.analyzer.frequencyBinCount;
        this.freqsArr = new Uint8Array(count);

        this.gain = this.audioCtx.createGain();
        this.gain.gain.setValueAtTime(1, this.audioCtx.currentTime);

        input.connect(this.analyzer).connect(this.gain).connect(this.audioCtx.destination);
    }

    getFreqs(): Uint8Array {
        this.analyzer.getByteFrequencyData(this.freqsArr);

        return this.freqsArr;
    }

    setVolume(volume: number): void {
        this.gain.gain.setValueAtTime(volume, this.audioCtx.currentTime);
    }
}

export default singleton(DoublesharpAudioCtx);
