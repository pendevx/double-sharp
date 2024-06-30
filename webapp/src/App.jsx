import { MusicIconSvg, MusicLoopSvg, MusicShuffleSvg } from "./icons";
import { MusicPlayerControl, MusicItem, CircularSvg } from "./components";
import React from "react";
import { MusicContext, AudioAnalyzer } from "./contexts";

let audioCtx;
let ctx;

export default function App() {
    const audioRef = React.useRef(null);
    const canvas = React.useRef(null);
    const musicContext = React.useContext(MusicContext);

    function onSongSelect(index) {
        musicContext.selectSongAndPlay(index);
    }

    function onPlayBehaviourChange(behaviour) {
        musicContext.setPlayBehaviour(behaviour === musicContext.playBehaviour ? null : behaviour);
    }

    function onKeyDown(e) {
        
        switch (e.key) {
            case " ": {
                e.preventDefault();

                if (musicContext.currentSong.key == null) {
                    musicContext.next();
                } else {
                    musicContext.isPlaying ? musicContext.pause() : musicContext.play();
                }
                break;
            }
        }
    }

    function onplay() {
        if (!audioCtx) {
            audioCtx = new AudioAnalyzer(audioRef.current);
        }

        const totalWidth = canvas.current.width;

        function raf() {
            requestAnimationFrame(() => {
                if (!musicContext.isPlaying) {
                    return;
                }
                
                const freqData = audioCtx.getFreqs();
                ctx.clearRect(0, 0, canvas.current.width, canvas.current.height);

                const len = freqData.length / 2.2;
                const barWidth = totalWidth / len;

                for (let i = 0; i < len; i++) {
                    ctx.fillRect(barWidth * i, 0, barWidth - 2, freqData[i] / 255 * (canvas.current.height - 20));
                }

                raf();
            });
        }

        raf();
    }

    React.useEffect(function() {
        ctx = canvas.current.getContext("2d");
        const dpr = window.devicePixelRatio || 1;

        ctx.scale(dpr, dpr);
        const rect = canvas.current.getBoundingClientRect();
        canvas.current.width = rect.width * dpr;
        canvas.current.height = rect.height * dpr;

        ctx.fillStyle = "white";
    }, []);

    return (
        <div className="font-sans" onKeyDown={onKeyDown} tabIndex={0}>
            <div className="mb-40">
                <div className="bg-black h-4 sticky top-0 z-10"></div>

                <div className="grid grid-cols-[2fr,1fr] pl-4 pr-4 items-start">
                    <div className="bg-[#363636] text-white rounded-xl overflow-hidden w-[80%] ml-auto mr-auto sticky top-4">
                        {musicContext.musicList.map((x,i) => <MusicItem key={x.etag} onClick={onSongSelect} index={i} name={/\/(?<filename>.*)\.mp3/gi.exec(x.key)?.groups?.filename} /> )}
                    </div>

                    <div className="bg-[#363636] text-white rounded-xl w-full mr-auto p-4 sticky top-4">directories placeholder</div>
                </div>

                <div className="bg-black fixed left-0 right-0 bottom-0 pt-4">
                    <div className="relative">
                        <canvas className="w-full h-14 block bg-black -scale-y-100" ref={canvas} />

                        <div className="absolute inset-0 opacity-50 mix-blend-multiply bg-rainbow" />
                    </div>
                    <MusicPlayerControl ref={audioRef} onplay={onplay} />
                </div>
            </div>

            {/* <div className="fixed bottom-8 right-6 flex flex-col-reverse gap-4 items-end">
                <CircularSvg width="5rem" height="5rem" className={`animate-spin ${musicContext.isPlaying ? "" : "animation-paused"}`}>
                    <MusicIconSvg />
                </CircularSvg>

                <div className="flex flex-col gap-4 mt-4">
                    <CircularSvg width="2.5rem" height="2.5rem" onClick={() => onPlayBehaviourChange("loop")} selected={musicContext.playBehaviour === "loop"}>
                        <MusicLoopSvg />
                    </CircularSvg>

                    <CircularSvg width="2.5rem" height="2.5rem" onClick={() => onPlayBehaviourChange("shuffle")} selected={musicContext.playBehaviour === "shuffle"}>
                        <MusicShuffleSvg />
                    </CircularSvg>
                </div>
            </div> */}
        </div>
    )
}
