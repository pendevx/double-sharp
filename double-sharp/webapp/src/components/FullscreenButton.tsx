import { MusicIconSvg } from "../icons";

type FullscreenButtonProps = {
    className?: string;
    onClick?: () => void;
    isOpen: boolean;
};

export default function FullscreenButton({ className, onClick, isOpen }: FullscreenButtonProps) {
    return (
        <div className={`flex h-full w-full items-center justify-center bg-[#0f0f0f] p-1 transition-all duration-300 hover:bg-[#666] ${className}`} onClick={onClick}>
            <MusicIconSvg isOpen={isOpen} fill="fill-white" />
        </div>
    );
}
