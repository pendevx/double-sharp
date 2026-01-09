import singleton from "./singleton";

export interface IAudioAnalyzer {
    getFreqs(): Uint8Array;
}

class AudioAnalyzer implements IAudioAnalyzer {
    private analyzer: AnalyserNode;
    private freqsArr: Uint8Array;

    constructor(audio: HTMLAudioElement) {
        const audioCtx = new AudioContext();
        const input = audioCtx.createMediaElementSource(audio);

        this.analyzer = audioCtx.createAnalyser();
        this.analyzer.fftSize = 1024;
        this.analyzer.smoothingTimeConstant = 0.75;

        const count = this.analyzer.frequencyBinCount;
        this.freqsArr = new Uint8Array(count);

        input.connect(this.analyzer).connect(audioCtx.destination);
    }

    getFreqs(): Uint8Array {
        this.analyzer.getByteFrequencyData(this.freqsArr);

        return this.freqsArr;
    }
}

export default singleton(AudioAnalyzer);
