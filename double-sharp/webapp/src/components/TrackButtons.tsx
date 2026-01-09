import { ChangeTrack } from "../icons";
import { MusicPausedSvg, MusicPlaySvg } from "../icons";

type TrackButtonsProps = {
    isPlaying: boolean;
    previous: () => void;
    next: () => void;
    handlePlayPause: () => void;
    className?: string;
};

export default function TrackButtons({ isPlaying, previous, next, handlePlayPause, className }: TrackButtonsProps) {
    return (
        <div className={`grid h-full grid-cols-3 ${className}`}>
            <div className="flex items-center justify-center">
                <ChangeTrack className="transition-color h-full rotate-180 cursor-pointer rounded-[50%] p-1 duration-300 hover:bg-gray-800" onClick={() => previous()} />
            </div>

            <div className="transition-color flex h-full w-fit cursor-pointer items-center justify-center rounded-[50%] duration-300 hover:bg-gray-800" onClick={handlePlayPause}>
                {isPlaying ? <MusicPausedSvg /> : <MusicPlaySvg />}
            </div>

            <div className="flex items-center justify-center">
                <ChangeTrack className="transition-color h-full cursor-pointer rounded-[50%] p-1 duration-300 hover:bg-gray-800" onClick={() => next()} />
            </div>
        </div>
    );
}
