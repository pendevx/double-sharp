import { SongList } from ".";

type ToggleSonglistProps = {
    className?: string;
    onClick?: () => void;
    isOpen?: boolean;
};

export default function ToggleSonglist({ className, onClick }: ToggleSonglistProps) {
    return (
        <div className={`flex aspect-square w-full items-center justify-center bg-[#0f0f0f] p-2 transition-all duration-300 hover:bg-[#666] ${className}`} onClick={onClick}>
            <SongList />
        </div>
    );
}
