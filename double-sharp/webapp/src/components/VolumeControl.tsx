import { Volume1, Volume2, VolumeOff, VolumeX } from "lucide-react";
import { HoverCard, HoverCardContent, HoverCardTrigger } from "@ui/hover-card";
import React from "react";
import { Slider } from "./ui/slider";

export default function VolumeControl({ volume, isMuted, onVolumeChange, onClick }: { volume: number; isMuted: boolean; onVolumeChange: (volume: number) => void; onClick: () => void }) {
    return (
        <div className="h-full flex w-8">
            <HoverCard openDelay={0} closeDelay={0}>
                <HoverCardTrigger onClick={onClick} className="flex items-center justify-center w-full">
                    {isMuted ? <VolumeOff /> : volume === 0 ? <VolumeX /> : volume < 0.5 ? <Volume1 /> : <Volume2 />}
                </HoverCardTrigger>
                <HoverCardContent className="p-4 w-16 rounded-none bg-zinc-900 flex flex-col items-center gap-2" side="top" sideOffset={0}>
                    <Slider orientation="vertical" className="h-20" min={0} max={1} step={0.01} value={[volume]} onValueChange={e => onVolumeChange(e[0])} />
                    <aside className="text-white">{Math.round(volume * 100)}</aside> 
                </HoverCardContent>
            </HoverCard>
        </div>
    );
}
