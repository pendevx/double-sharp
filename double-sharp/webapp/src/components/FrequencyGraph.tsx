import React from "react";
import { MusicContext } from "../contexts/MusicContext";
import AudioAnalyzer, { IAudioAnalyzer } from "../utils/AudioAnalyzer";

type FrequencyGraphProps = {
    audioRef: React.RefObject<HTMLAudioElement>;
};

function FrequencyGraph({ audioRef }: FrequencyGraphProps) {
    const audioCtx = React.useRef<IAudioAnalyzer | null>(null);
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

            audioCtx.current = new AudioAnalyzer(audioRef.current);

            let len: number | null;
            let barWidth: number | null;
            const totalWidth = canvas.current.width;
            const totalHeight = canvas.current.height;

            function raf() {
                requestAnimationFrame(() => {
                    if (!musicContext.isPlaying) {
                        return;
                    }

                    const freqData = audioCtx.current?.getFreqs();
                    canvasCtx.current?.clearRect(0, 0, totalWidth, totalHeight);

                    if (freqData == null) {
                        return;
                    }

                    if (len == null && barWidth == null) {
                        len = freqData.length / 2.5;
                        barWidth = totalWidth / len;
                    }

                    for (let i = 0; i < (len as number); i++) {
                        canvasCtx.current?.fillRect((barWidth as number) * i, 0, (barWidth as number) - 2, (freqData[i] / 255) * (totalHeight - 20));
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
            <canvas className="block h-14 w-full -scale-y-100 bg-black" ref={canvas} />
            <div className="absolute inset-0 bg-[linear-gradient(to_top,#6666,#0000)]" />
            <div className="bg-rainbow absolute inset-0 opacity-80 mix-blend-multiply" />
        </div>
    );
}

export default FrequencyGraph;
