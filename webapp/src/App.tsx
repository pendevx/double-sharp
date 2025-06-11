import { MusicPlayerControl, FrequencyGraph, SettingsButton, UploadSongButton, BlurredModal, FullScreenOverlay } from "./components";
import React, { RefObject } from "react";
import { MusicContext } from "./contexts/MusicContext";
import { CurrentSongModal, RequestSongModal, SettingsModal } from "./components/modals";
import { Outlet } from "react-router";

const modalReducer = (state: Modal, action: { type: Modal; toggle?: boolean }): Modal => (action.toggle === false ? action.type : action.type === state ? Modal.None : action.type);

enum Modal {
    None,
    Fullscreen,
    Settings,
    RequestSong,
}

export default function App() {
    const [activeModal, dispatchModal] = React.useReducer(modalReducer, Modal.None);
    const [keyDownSignal, setKeyDownSignal] = React.useState<string>();
    const audioRef = React.useRef<HTMLAudioElement>(null);
    const musicContext = React.useContext(MusicContext);

    function onKeyDown(e: React.KeyboardEvent) {
        setKeyDownSignal(e.key.toLowerCase());
        setTimeout(() => setKeyDownSignal(undefined));

        switch (e.key.toLowerCase()) {
            case " ": {
                e.preventDefault();

                if (musicContext.currentSongId == null) {
                    musicContext.next();
                } else {
                    musicContext.isPlaying ? musicContext.pause() : musicContext.play();
                }
                break;
            }

            case "escape": {
                dispatchModal({ type: Modal.None });
                break;
            }

            case "f": {
                dispatchModal({ type: Modal.Fullscreen });
                break;
            }

            case "s": {
                dispatchModal({ type: Modal.Settings });
                break;
            }

            case "u": {
                dispatchModal({ type: Modal.RequestSong });
                break;
            }
        }
    }

    const hideFullscreen = () => dispatchModal({ type: Modal.None });

    return (
        <div onKeyDown={onKeyDown} tabIndex={0} className="font-sans">
            {" "}
            {/* todo: create a GlobalEventsContext which will listen for global events. Wrap around App.tsx (place in index.tsx) */}
            <h1 className="hidden">pendevx music</h1>
            <div className="fixed inset-0 flex">
                <div className="flex h-full w-full flex-col justify-between">
                    <div className="mt-4 h-full overflow-hidden px-4">
                        <Outlet context={{ keyDownSignal }} />
                    </div>

                    <div>
                        <FrequencyGraph audioRef={audioRef as RefObject<HTMLAudioElement>} />
                        <MusicPlayerControl audioRef={audioRef as RefObject<HTMLAudioElement>} goFullscreen={() => dispatchModal({ type: Modal.Fullscreen, toggle: false })} />
                    </div>
                </div>
            </div>
            <BlurredModal show={activeModal != Modal.None}>
                <ModalContainer isActive={activeModal === Modal.Fullscreen} hideFullscreen={hideFullscreen}>
                    <CurrentSongModal
                        closeModal={hideFullscreen}
                        toggleSongList={() => {
                            dispatchModal({ type: Modal.Fullscreen });
                            setKeyDownSignal("d");
                        }}
                    />
                </ModalContainer>

                <ModalContainer isActive={activeModal === Modal.Settings} hideFullscreen={hideFullscreen}>
                    <SettingsModal />
                </ModalContainer>

                <ModalContainer isActive={activeModal === Modal.RequestSong} hideFullscreen={hideFullscreen}>
                    <RequestSongModal />
                </ModalContainer>
            </BlurredModal>
            <div className="fixed right-6 top-6 w-12 p-1 laptop:w-14">
                <div className="flex flex-col gap-2">
                    {/* In the near future:
                    - Settings should become its own page
                    - RequestSong can become its own page, likely will do so.

                    Can extract the left-right and hideable left-side layout into a reusable component to use in landing page, admin page, and settings pages. */}
                    <SettingsButton onClick={() => dispatchModal({ type: Modal.Settings })} />
                    <UploadSongButton onClick={() => dispatchModal({ type: Modal.RequestSong })} />
                </div>
            </div>
        </div>
    );
}

type ModalContainerProps = {
    children: React.ReactNode;
    isActive: boolean;
    hideFullscreen: () => void;
};

function ModalContainer({ children, isActive, hideFullscreen }: ModalContainerProps) {
    return (
        <div className={`fixed inset-0 z-20 transition-all duration-1000 ${!isActive && "translate-y-full"}`}>
            <FullScreenOverlay hideFullscreen={hideFullscreen}>{children}</FullScreenOverlay>
        </div>
    );
}
