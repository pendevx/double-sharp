type MusicItemProps = {
    id: number;
    onClick: (id: number) => void;
    isPlaying: boolean;
    name: string;
};

export default function MusicItem({ id, onClick, isPlaying, name }: MusicItemProps) {
    function handleClick(e: React.MouseEvent<HTMLLIElement>) {
        onClick(id);
        e.currentTarget.blur();
    }

    return (
        <li onClick={handleClick}>
            <button
                className={`block cursor-pointer overflow-hidden text-nowrap pb-2 pl-4 pt-2 text-[#7c7c7c] transition-colors duration-200 hover:bg-[#2b2b2b] hover:text-white ${isPlaying ? "bg-[#2b2b2b] text-[#ffc421] mobile:hover:bg-[#2b2b2b] mobile:hover:text-[#ffc421] laptop:hover:bg-[#555555] laptop:hover:text-[#ffc421]" : ""}`}>
                {name}
            </button>
        </li>
    );
}
