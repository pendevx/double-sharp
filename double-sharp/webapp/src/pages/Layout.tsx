import { MusicPlayerControl, FrequencyGraph } from "../components";
import React, { RefObject } from "react";
import { Outlet } from "react-router";
import { IDoublesharpAudioCtx } from "../utils/AudioCtx";
import { AudioTimeContext } from "@/contexts/AudioTimeContext";
import { MusicContext } from "@/contexts/MusicContext";
import Audio from "@/components/Audio";

type ActionType = { type: Modal; toggle?: boolean };
const modalReducer = (state: Modal, action: ActionType): Modal => (action.toggle === false ? action.type : action.type === state ? Modal.None : action.type);

export enum Modal {
    None,
    Fullscreen,
    Settings,
    RequestSong,
}

export type OutletContextType = {
    activeModal: Modal;
    dispatchModal: React.Dispatch<ActionType>;
};

export default function Layout() {
    const [activeModal, dispatchModal] = React.useReducer(modalReducer, Modal.None);
    const audioCtx = React.useRef<IDoublesharpAudioCtx | null>(null);
    const audioRef = React.useRef<HTMLAudioElement>(null);
    const musicContext = React.useContext(MusicContext);
    const audioTimeContext = React.useContext(AudioTimeContext);

    React.useEffect(() => {
        if (audioRef.current && (audioTimeContext.requestTime ?? 0) >= 0) {
            audioRef.current.currentTime = audioTimeContext.requestTime || 0;
            audioTimeContext.setRequestTime(-1);
        }
    }, [audioTimeContext.requestTime]);

    React.useEffect(() => {
        if (audioCtx.current) {
            audioCtx.current.setVolume(musicContext.isMuted ? 0 : musicContext.volume);
        }
    }, [musicContext.volume]);

    const requiresAudioRef = (then: Function) => audioRef.current && then();

    const fastforwardHandler = (secs: number) =>
        requiresAudioRef(() => {
            audioRef.current!.currentTime = secs;
            audioTimeContext.setCurrentTime(secs);
        });
    const handlePlayPause = () =>
        requiresAudioRef(() => {
            if (audioRef.current!.paused) {
                musicContext.play();
            } else {
                musicContext.pause();
            }
        });

    return (
        <div className="font-sans">
            <div className="fixed inset-0 flex">
                <div className="flex h-full w-full flex-col justify-between">
                    <div className="mt-4 h-full overflow-hidden px-4">
                        <Outlet context={{ activeModal, dispatchModal }} />
                    </div>

                    <div>
                        <FrequencyGraph audioRef={audioRef as RefObject<HTMLAudioElement>} audioCtx={audioCtx as RefObject<IDoublesharpAudioCtx>} />
                        <MusicPlayerControl
                            duration={audioRef.current?.duration}
                            goFullscreen={() => dispatchModal({ type: Modal.Fullscreen, toggle: false })}
                            handlePlayPause={handlePlayPause}
                            fastforwardHandler={fastforwardHandler}
                        />
                    </div>
                </div>
            </div>

            <Audio audioRef={audioRef} />
        </div>
    );
}
