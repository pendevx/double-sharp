import React from "react";
import createPRNG from "../utils/pseudo-rng";
import { playBehaviours } from "../utils/playBehaviour";
import { useStoreRef, useStoreState } from "../hooks/useStore";
import useFetch from "../hooks/useFetch";
import { listSongs } from "../utils/url-builder.api";
import { PlayBehaviour } from "../types/playBehaviour";

type TMusicContext = {
    currentSong: SongInformation;
    currentSongId: number;
    isPlaying: boolean;
    playBehaviour: PlayBehaviour;
    setPlayBehaviour: (behaviour: PlayBehaviour) => void;
    previous: () => void;
    next: () => void;
    musicList: SongInfoDTO[];
    selectSongById: (id: number, play?: boolean) => void;
    play: () => void;
    pause: () => void;
};

type SongInformation = {
    name: string;
    index: number;
};

type SongInfoDTO = {
    id: number;
    name: string;
};

export const MusicContext = React.createContext<TMusicContext>({} as TMusicContext);

const randomSeed = () => Math.floor(Math.random() * 2 ** 16);

export default function MusicProvider({ children }: { children: React.ReactNode }) {
    const [currentSong, setCurrentSong] = React.useState<SongInformation>({ name: "", index: -1 });
    const [currentSongId, setCurrentSongId] = useStoreState<number>("lastSongId", Number);
    const [isPlaying, setIsPlaying] = React.useState<boolean>(false);
    const { data: musicList, refreshData } = useFetch<SongInfoDTO[]>([]);
    const [playBehaviour, updatePlayBehaviour] = useStoreState<PlayBehaviour>("playBehaviour");
    const shuffleSeed = useStoreRef<number>("seed", Number);
    const playOrder = React.useMemo<SongInfoDTO[]>(
        function (): SongInfoDTO[] {
            if (playBehaviour === "shuffle") {
                const prng = createPRNG(shuffleSeed.current);
                return [...musicList].sort(() => prng.next().value / 2 ** 31 - 0.5) as SongInfoDTO[];
            }

            return musicList;
        },
        [musicList, playBehaviour, shuffleSeed]
    );

    React.useEffect(function () {
        refreshData(listSongs());
    }, []);

    function previous() {
        const index = (currentSong.index - 1 + playOrder.length) % playOrder.length || 0;
        selectSongByIndex(index);
    }

    function next() {
        const index = (currentSong.index + 1) % playOrder.length || 0;
        selectSongByIndex(index);
    }

    function selectSongByIndex(index: number, play: boolean = true) {
        const song = playOrder[index];

        if (index < 0 || index >= playOrder.length) {
            throw new Error("Invalid song index");
        }

        selectSong(song.name, song.id, index, play);
    }

    function selectSongById(id: number, play: boolean = true) {
        const index = playOrder.findIndex(x => x.id === id);

        if (index === -1) {
            throw new Error("Invalid song id");
        }

        selectSong(playOrder[index].name, id, index, play);
    }

    function selectSong(name: string, id: number, index: number, play: boolean = true) {
        setCurrentSong({ name, index });
        setCurrentSongId(id);
        setIsPlaying(play);
    }

    function setPlayBehaviour(behaviour: PlayBehaviour) {
        if (behaviour === "shuffle") {
            shuffleSeed.current = randomSeed();
        }

        updatePlayBehaviour(behaviour);
    }

    const play = () => setIsPlaying(true);
    const pause = () => setIsPlaying(false);

    if (currentSong.index === -1) {
        if (playBehaviour === "shuffle") {
            shuffleSeed.current ||= randomSeed();
        }

        if (playOrder.length) {
            if (!currentSongId || !playOrder.some(x => x.id === currentSongId)) {
                selectSong(playOrder[0]?.name || "", playOrder[0]?.id, 0, false); // default to first song in the list
            } else {
                selectSongById(currentSongId, false);
            }
        }
    }

    return (
        <MusicContext.Provider
            value={{
                currentSong,
                currentSongId,
                isPlaying,
                playBehaviour,
                setPlayBehaviour,
                previous,
                next,
                musicList,
                selectSongById,
                play,
                pause,
            }}>
            {children}
        </MusicContext.Provider>
    );
}
