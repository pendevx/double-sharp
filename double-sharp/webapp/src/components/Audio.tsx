import React from "react";
import { AudioTimeContext } from "@/contexts/AudioTimeContext";
import { MusicContext } from "@/contexts/MusicContext";
import { downloadSong } from "@/utils/url-builder.api";
import { processRequestUrl } from "@/hooks/useFetch";

type ActionType = { type: Modal; toggle?: boolean };

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

export default function Audio({ audioRef }: { audioRef: React.RefObject<HTMLAudioElement | null> }) {
    const musicContext = React.useContext(MusicContext);
    const audioTimeContext = React.useContext(AudioTimeContext);
    const [currentSongId, setCurrentSongId] = React.useState(0);

    React.useEffect(
        function () {
            audioTimeContext.setTotalDuration(audioRef.current?.duration || 0);

            if (navigator.mediaSession) {
                navigator.mediaSession.setActionHandler("previoustrack", musicContext.previous);
                navigator.mediaSession.setActionHandler("nexttrack", musicContext.next);
                navigator.mediaSession.setActionHandler("play", musicContext.play);
                navigator.mediaSession.setActionHandler("pause", musicContext.pause);
            }
        },
        [musicContext]
    );

    React.useEffect(() => {
        if (audioRef.current && (audioTimeContext.requestTime ?? 0) >= 0) {
            audioRef.current.currentTime = audioTimeContext.requestTime || 0;
            audioTimeContext.setRequestTime(-1);
        }
    }, [audioTimeContext.requestTime]);

    const timeUpdateHandler = () => {
        if (audioRef.current) audioTimeContext.setCurrentTime(audioRef.current!.currentTime);
    };

    const onError = async () => {
        const audio = audioRef.current!;
        const currentTime = audio.currentTime;

        audio.pause();

        // full reset
        audio.removeAttribute("src");
        audio.load();

        audio.src = processRequestUrl(downloadSong(musicContext.currentSongId) + `?t=${Date.now()}`);

        // Wait until the browser knows duration / can seek
        await new Promise<void>(resolve => {
            const handler = () => {
                audio.removeEventListener("loadedmetadata", handler);
                resolve();
            };
            audio.addEventListener("loadedmetadata", handler);
        });

        // Now it's safe
        audio.currentTime = currentTime;

        try {
            await audio.play();
        } catch (err) {
            console.warn("play failed", err);
        }
    };

    if (audioRef.current && musicContext.currentSongId && currentSongId !== musicContext.currentSongId) {
        setCurrentSongId(musicContext.currentSongId);
        document.title = musicContext.currentSong.name || "pendevx music";

        const songUrl = downloadSong(musicContext.currentSongId);

        audioRef.current.pause();
        audioRef.current.src = processRequestUrl(songUrl);
        audioRef.current.load();
        audioRef.current.play().then(musicContext.play);
    }

    if (musicContext.isPlaying && audioRef.current?.paused) {
        audioRef.current?.play();
    } else if (!musicContext.isPlaying && !audioRef.current?.paused) {
        audioRef.current?.pause();
    }

    return (
        <audio
            ref={audioRef}
            onTimeUpdate={timeUpdateHandler}
            onEnded={musicContext.next}
            onLoadedMetadata={() => audioTimeContext.setTotalDuration(audioRef.current?.duration || 0)}
            crossOrigin="anonymous"
            loop={musicContext.playBehaviour === "loop"}
            muted={musicContext.isMuted}
            onError={onError}
        />
    );
}
