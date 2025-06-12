import React from "react";
import { BlurredModal, FullScreenOverlay, Lyrics, MusicList, SettingsButton, UploadSongButton } from "../components";
import getViewportResolution, { ViewportResolution } from "../utils/viewportResolution";
import { PageEventsContext } from "../contexts/PageEventsContext";
import { useNavigate, useOutletContext } from "react-router";
import { Modal, OutletContextType } from "./Layout";
import { CurrentSongModal, RequestSongModal, SettingsModal } from "../components/modals";
import { MusicContext } from "../contexts/MusicContext";
import AuthGuard from "../components/AuthGuard";
import { AdminLock } from "../icons";
import { Role } from "../contexts/AuthContext";

export default function IndexPage() {
    const [showSonglist, setShowSonglist] = React.useState<boolean>(true);
    const [bodyHeight, setBodyHeight] = React.useState<number>(0);
    const bodyRef = React.useRef<HTMLDivElement | null>(null);
    const pageEventsContext = React.useContext(PageEventsContext);
    const musicContext = React.useContext(MusicContext);
    const { activeModal, dispatchModal } = useOutletContext<OutletContextType>();
    const navigate = useNavigate();

    React.useEffect(() => {
        const resizeHandler = () => {
            if (bodyRef.current) {
                setBodyHeight(bodyRef.current.clientHeight);
            }
        };

        const onPageKeyDown = (e: React.KeyboardEvent) => {
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

                case "d": {
                    setShowSonglist(!showSonglist);
                    break;
                }
            }
        };

        resizeHandler();

        window.addEventListener("resize", resizeHandler);
        pageEventsContext.addListener("onKeyDown", onPageKeyDown);

        return () => {
            window.removeEventListener("resize", resizeHandler);
            pageEventsContext.removeListener("onKeyDown", onPageKeyDown);
        };
    }, [showSonglist]);

    const resolution = getViewportResolution();

    function onSongSelected() {
        if (resolution < ViewportResolution.Laptop) {
            setShowSonglist(false);
        }
    }

    const hideFullscreen = () => dispatchModal({ type: Modal.None });

    const showSongModal = () => {
        hideFullscreen();
        setShowSonglist(true);
    };

    return (
        <>
            <div ref={bodyRef} className="r-0 relative flex h-full max-h-full w-full justify-end laptop:right-[33.33333%] laptop:w-[133.33333%] desktop:right-[25%] desktop:w-[125%]">
                <MusicList showSonglist={showSonglist} onSongSelected={onSongSelected} />
                <Lyrics height={bodyHeight / 2} showSonglist={showSonglist} />
            </div>
            <BlurredModal show={activeModal !== Modal.None}>
                <ModalContainer isActive={activeModal === Modal.Fullscreen} hideFullscreen={hideFullscreen}>
                    <CurrentSongModal closeModal={hideFullscreen} showSongModal={showSongModal} />
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
                    <AuthGuard requiredRole={Role.Admin}>
                        <div className="flex h-full w-full items-center justify-center bg-[#0f0f0f] transition-all duration-300 hover:bg-[#666]" onClick={() => navigate("/admin")}>
                            <AdminLock />
                        </div>
                    </AuthGuard>
                </div>
            </div>
        </>
    );
}

type ModalContainerProps = {
    children: React.ReactNode;
    isActive: boolean;
    hideFullscreen: () => void;
};

export function ModalContainer({ children, isActive, hideFullscreen }: ModalContainerProps) {
    return (
        <div className={`fixed inset-0 z-20 transition-all duration-1000 ${!isActive && "translate-y-full"}`}>
            <FullScreenOverlay hideFullscreen={hideFullscreen}>{children}</FullScreenOverlay>
        </div>
    );
}
