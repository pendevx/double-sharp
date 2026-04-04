import React from "react";
import { MusicContext } from "../contexts/MusicContext";
import DoublesharpAudioCtx, { IDoublesharpAudioCtx } from "../utils/AudioCtx";

type FrequencyGraphProps = {
    audioRef: React.RefObject<HTMLAudioElement>;
    audioCtx: React.RefObject<IDoublesharpAudioCtx | null>;
};

const DB_MIN = -80;
const DB_MAX = 0;

function magnitudeToDb(mag: number) {
    if (mag < 1e-10) return DB_MIN;
    return 20 * Math.log10(mag);
}

function FrequencyGraph({ audioRef, audioCtx }: FrequencyGraphProps) {
    const canvas = React.useRef<HTMLCanvasElement | null>(null);
    const canvasCtx = React.useRef<CanvasRenderingContext2D | null>(null);
    const musicContext = React.useContext(MusicContext);

    React.useEffect(function () {
        if (canvas.current == null) {
            return;
        }

        const ctx = canvas.current.getContext("2d");
        const dpr = window.devicePixelRatio || 1;

        if (ctx == null) {
            return;
        }

        ctx.scale(dpr, dpr);
        const rect = canvas.current.getBoundingClientRect();
        canvas.current.width = rect.width * dpr;
        canvas.current.height = rect.height * dpr;

        ctx.fillStyle = "white";

        canvasCtx.current = ctx;
    }, []);

    React.useEffect(
        function () {
            if (!musicContext.isPlaying || canvas.current == null) {
                return;
            }

            audioCtx.current = new DoublesharpAudioCtx(audioRef.current);

            let barWidth: number;
            const totalWidth = canvas.current.width;
            const totalHeight = canvas.current.height;

            const W = canvas.current.width;
            const H = canvas.current.height;
            const pad = { top: 0, bottom: 0, left: 0, right: 0 };
            const plotW = W - pad.left - pad.right;
            const plotH = H - pad.top - pad.bottom;

            function raf() {
                requestAnimationFrame(() => {
                    if (!musicContext.isPlaying || musicContext.isMuted) {
                        return;
                    }

                    const freqData = audioCtx.current?.getFreqs()!;
                    canvasCtx.current?.clearRect(0, 0, totalWidth, totalHeight);

                    const numBins = audioCtx.current?.cqtKernel.length;

                    if (numBins === 0) {
                        return;
                    }

                    if (barWidth == null) {
                        barWidth = plotW / numBins!;
                    }

                    for (let k = 0; k < numBins!; k++) {
                        const db = magnitudeToDb(freqData[k]);
                        const t = Math.max(0, Math.min(1, (db - DB_MIN) / (DB_MAX - DB_MIN)));
                        const barH = t * plotH;
                        const x = pad.left + k * barWidth;
                        const y = pad.top + plotH - barH;

                        canvasCtx.current?.fillRect(x, y, Math.max(barWidth - 0.5, 1), barH);
                    }

                    raf();
                });
            }

            raf();
        },
        [musicContext.isPlaying]
    );

    return (
        <div className="relative">
            <canvas className="block h-14 w-full bg-black" ref={canvas} />
            <div className="absolute inset-0 bg-[linear-gradient(to_top,#6666,#0000)]" />
            <div className="bg-rainbow absolute inset-0 opacity-80 mix-blend-multiply" />
        </div>
    );
}

export default FrequencyGraph;
