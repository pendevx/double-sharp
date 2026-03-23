/* eslint-disable react-refresh/only-export-components */
import { LoopShuffleControl, MusicProgressBar, TrackButtons } from ".";
import React from "react";
import { MusicContext } from "../contexts/MusicContext";
import { formatTime } from "../utils/formats";
import { AudioTimeContext } from "../contexts/AudioTimeContext";
import VolumeControl from "./VolumeControl";

type MusicPlayerControlProps = {
    goFullscreen: () => void;
    handlePlayPause: () => void;
    duration: number | undefined;
    fastforwardHandler: (secs: number) => void;
};

export default function MusicPlayerControl({ goFullscreen, handlePlayPause, duration, fastforwardHandler }: MusicPlayerControlProps) {
    const musicContext = React.useContext(MusicContext);
    const audioTimeContext = React.useContext(AudioTimeContext);

    const time = audioTimeContext.currentTime ? formatTime(audioTimeContext.currentTime) : "--:--";
    const totalDuration = duration ? formatTime(duration) : "--:--";
    const songDurationSecs = duration || 0;

    return (
        <div className="bg-black">
            <div className="flex h-16 w-full items-center gap-2 overflow-hidden border-t border-solid border-gray-900 bg-zinc-900 pr-4 pl-4 text-white">
                <p
                    className="desktop:basis-60 flex h-full basis-32 items-center overflow-hidden border-r border-dotted border-slate-600 text-nowrap text-ellipsis hover:cursor-pointer"
                    onClick={goFullscreen}>
                    {musicContext.currentSong.name}
                </p>
                <div className="h-8">
                    <TrackButtons handlePlayPause={handlePlayPause} isPlaying={musicContext.isPlaying} next={musicContext.next} previous={musicContext.previous} className="mr-2 ml-1" />
                </div>
                <div className="tablet:block hidden grow">
                    <MusicProgressBar songDurationSecs={songDurationSecs} currentTime={audioTimeContext.currentTime || 0} onFastForward={fastforwardHandler} />
                </div>
                <span>
                    {time} / {totalDuration}
                </span>
                <VolumeControl />
                <LoopShuffleControl />
            </div>
        </div>
    );
}

// possible error: Unhandled Promise Rejection: AbortError: The play() request was interrupted by a call to pause().
// https://developer.chrome.com/blog/play-request-was-interrupted

// we are able to swallow/ignore the error since:
// the any previous unfulfilled play requests should be exited anyways, and
// the pause current or load new song request should be ran.
// we only want the latest play request to be executed and fulfilled.
