import React from "react";
import { MusicContext } from "../contexts/MusicContext";
import { CircularSvg } from "./";
import { MusicShuffleSvg, MusicLoopSvg, MusicIconSvg } from "../icons";
import { PlayBehaviour } from "../types/playBehaviour";

const height = "h-[60%]";

function playBehaviourStyles(playBehaviour: PlayBehaviour) {
    switch (playBehaviour) {
        case "shuffle":
            return "-translate-x-full";
        case "loop":
            return "translate-x-full";
        case null:
            return "translate-x-0";
        default:
            return "";
    }
}

export default function LoopShuffleControl() {
    const musicContext = React.useContext(MusicContext);

    function onPlayBehaviourChange(behaviour: PlayBehaviour) {
        if (behaviour == null || musicContext.playBehaviour === behaviour) {
            musicContext.setPlayBehaviour(null);
        } else {
            musicContext.setPlayBehaviour(behaviour);
        }
    }

    return (
        <div className="relative z-10 ml-2 hidden h-full items-center laptop:flex">
            <i className={`w-full ${height} absolute -z-10 rounded-[1.5rem] bg-[#585858]`} />
            <i
                className={`absolute left-0 right-0 ${height} -z-10 ml-auto mr-auto aspect-square rounded-[50%] bg-[#cea127] transition-all duration-200 ${playBehaviourStyles(musicContext.playBehaviour)} `}
            />

            <CircularSvg className={height} onClick={() => onPlayBehaviourChange("shuffle")}>
                <MusicShuffleSvg />
            </CircularSvg>

            <CircularSvg className={`${height} ${musicContext.isPlaying ? "" : "animation-paused"} animate-spin p-2`} onClick={() => onPlayBehaviourChange(null)}>
                <MusicIconSvg />
            </CircularSvg>

            <CircularSvg className={height} onClick={() => onPlayBehaviourChange("loop")}>
                <MusicLoopSvg />
            </CircularSvg>
        </div>
    );
}
