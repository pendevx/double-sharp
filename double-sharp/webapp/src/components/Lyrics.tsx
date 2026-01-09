import React from "react";
import { Scrollable } from ".";
import { MusicContext } from "../contexts/MusicContext";
import { downloadLyrics } from "../utils/url-builder.api";
import useFetch from "../hooks/useFetch";
import { AudioTimeContext } from "../contexts/AudioTimeContext";

const SCROLL_IGNORE_DURATION = 1500;

type LyricsProps = {
    height: number;
    showSonglist: boolean;
};

type LyricsLine = {
    time: number;
    words: string;
};

export default function Lyrics({ height, showSonglist }: LyricsProps) {
    const { data: lyrics, refreshData } = useFetch<LyricsLine[]>([]);
    const [index, setIndex] = React.useState<number>(0);
    const [scrollTop, setScrollTop] = React.useState<number | null>(0);
    const getHeight = React.useCallback(
        (el: HTMLDivElement | null) => {
            if (el) setLineHeight(el.scrollHeight / lyrics.length);
        },
        [lyrics]
    );
    const [lineHeight, setLineHeight] = React.useState<number>(0);
    const [currentSongId, setCurrentSongId] = React.useState<number>(0);
    const timerRef = React.useRef<number | null>(null);
    const musicContext = React.useContext(MusicContext);
    const audioTimeContext = React.useContext(AudioTimeContext);

    function updateIndex(i: number) {
        setIndex(i);
        setScrollTop(i * lineHeight);

        audioTimeContext.setRequestTime(lyrics[i].time);
    }

    function handleScroll() {
        if (timerRef.current) clearTimeout(timerRef.current);

        if (scrollTop === null) return;

        setScrollTop(null);

        timerRef.current = setTimeout(() => {
            timerRef.current = null;
        }, SCROLL_IGNORE_DURATION);
    }

    if (currentSongId !== musicContext.currentSongId) {
        if (musicContext.currentSongId > 0) {
            setCurrentSongId(musicContext.currentSongId);
            refreshData(downloadLyrics(musicContext.currentSongId));
        }

        const highlightedIndex = lyrics.findIndex(({ time }) => time > (audioTimeContext.currentTime ?? 0));
        const nextIndex = highlightedIndex === -1 ? lyrics.length - 1 : highlightedIndex - 1;

        setIndex(nextIndex);

        if (!timerRef.current) {
            setScrollTop(nextIndex * lineHeight);
        }
    }

    return (
        <Scrollable
            className={`relative flex w-full pl-4 text-center text-white transition-all duration-1000 laptop:w-2/4 laptop:grow-0 laptop:pl-0 desktop:w-3/5 ${!showSonglist && "laptop:w-3/4 desktop:w-4/5"}`}
            showScroller={false}
            scrollTop={scrollTop || 0}
            onScroll={handleScroll}
            smooth={true}>
            <div style={{ height: height - (lineHeight || 0) / 2 }} />
            <div ref={getHeight}>
                {lyrics.map(({ words }, i) => (
                    <p
                        key={i}
                        className={`mb-6 overflow-hidden whitespace-pre py-[2px] transition-all duration-300 ${index === i ? "text-2xl font-medium text-white" : "text-gray-500"}`}
                        onClick={() => updateIndex(i)}>
                        {" " + words + " "}
                    </p>
                ))}
            </div>
            <div style={{ height }} />
        </Scrollable>
    );
}
