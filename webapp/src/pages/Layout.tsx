import { MusicPlayerControl, FrequencyGraph } from "../components";
import React, { RefObject } from "react";
import { Outlet } from "react-router";

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
    const audioRef = React.useRef<HTMLAudioElement>(null);

    return (
        <div className="font-sans">
            <div className="fixed inset-0 flex">
                <div className="flex h-full w-full flex-col justify-between">
                    <div className="mt-4 h-full overflow-hidden px-4">
                        <Outlet context={{ activeModal, dispatchModal }} />
                    </div>

                    <div>
                        <FrequencyGraph audioRef={audioRef as RefObject<HTMLAudioElement>} />
                        <MusicPlayerControl audioRef={audioRef as RefObject<HTMLAudioElement>} goFullscreen={() => dispatchModal({ type: Modal.Fullscreen, toggle: false })} />
                    </div>
                </div>
            </div>
        </div>
    );
}
