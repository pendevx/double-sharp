import { UploadSong } from "../icons";

type UploadSongButtonProps = {
    className?: string;
    onClick?: () => void;
    isOpen?: boolean;
};

export default function UploadSongButton({ className, onClick }: UploadSongButtonProps) {
    return (
        <div className={`flex h-full w-full items-center justify-center bg-[#0f0f0f] transition-all duration-300 hover:bg-[#666] ${className}`} onClick={onClick}>
            <UploadSong />
        </div>
    );
}
