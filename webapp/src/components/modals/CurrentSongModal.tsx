import React from "react";
import { MusicContext } from "../../contexts/MusicContext";
import { MusicProgressBar } from "../";
import { formatTime } from "../../utils/formats";
import { MusicIconSvg, PlayBehaviourIcon, MusicPausedSvg, MusicPlaySvg, ChangeTrack, Cross, SongList } from "../../icons";
import { nextPlayBehaviour } from "../../utils/playBehaviour";
import { AudioTimeContext } from "../../contexts/AudioTimeContext";

type CurrentSongModalProps = {
    closeModal: () => void;
    toggleSongList: () => void;
};

export default function CurrentSongModal({ closeModal, toggleSongList }: CurrentSongModalProps) {
    const musicContext = React.useContext(MusicContext);
    const audioTimeContext = React.useContext(AudioTimeContext);

    function onFastForward(secs: number) {
        audioTimeContext.setRequestTime(secs);
    }

    function handlePlayPause() {
        if (musicContext.isPlaying) {
            musicContext.pause();
        } else {
            musicContext.play();
        }
    }

    function changePlayBehaviour() {
        musicContext.setPlayBehaviour(nextPlayBehaviour(musicContext.playBehaviour));
    }

    return (
        <div className="grid h-[1px] w-3/5 grow grid-rows-[1fr,4fr,1fr,3fr] gap-y-4 pb-8 pt-8 tablet:w-1/2 laptop:w-1/3 desktop:w-1/4">
            <div>
                <h2 className="text-center text-3xl">{musicContext.currentSong.name}</h2>
                <h3 className="text-center text-xl">{"Artist"}</h3>
            </div>

            <div className="rounded-2xl bg-white p-24">
                <MusicIconSvg />
            </div>

            <div className="flex flex-col">
                <MusicProgressBar songDurationSecs={audioTimeContext.totalDuration || 0} currentTime={audioTimeContext.currentTime || 0} onFastForward={onFastForward} />

                <span className="textfont-semibold w-full text-center">
                    {formatTime(audioTimeContext.currentTime || 0)} / {formatTime(audioTimeContext.totalDuration || 0)}
                </span>
            </div>

            <div className="grid grid-cols-3 grid-rows-2 gap-x-10 gap-y-16 p-2 laptop:gap-y-0">
                <IconContainer>
                    <SongList className="p-1" onClick={toggleSongList} />
                </IconContainer>

                <IconContainer onClick={closeModal}>
                    <Cross className="p-1" fill="fill-[#cea127]" />
                </IconContainer>

                <IconContainer onClick={changePlayBehaviour}>
                    <PlayBehaviourIcon playBehaviour={musicContext.playBehaviour} className="h-full w-full" fill="fill-white" />
                </IconContainer>

                <IconContainer onClick={musicContext.previous}>
                    <ChangeTrack className="rotate-180" />
                </IconContainer>

                <div className="flex aspect-square h-full items-center justify-center tablet:p-1 laptop:p-2" onClick={handlePlayPause}>
                    <div className="h-full w-full cursor-pointer rounded-[50%] bg-zinc-800 transition-colors duration-300 hover:bg-[#cea12765] tablet:p-1 laptop:p-2">
                        {musicContext.isPlaying ? <MusicPausedSvg /> : <MusicPlaySvg />}
                    </div>
                </div>

                <IconContainer onClick={musicContext.next}>
                    <ChangeTrack />
                </IconContainer>
            </div>
        </div>
    );
}

type IconContainerProps = {
    onClick?: () => void;
    children?: React.ReactNode;
};

function IconContainer({ onClick, children }: IconContainerProps) {
    return (
        <div className="flex aspect-square h-full items-center justify-center tablet:p-2 laptop:p-4" onClick={onClick}>
            <div className="h-full w-full cursor-pointer rounded-[50%] transition-colors duration-300 hover:bg-gray-800 tablet:p-2 laptop:p-4">{children}</div>
        </div>
    );
}
