import { Volume1, Volume2, VolumeOff, VolumeX } from "lucide-react";
import { HoverCard, HoverCardContent, HoverCardTrigger } from "@ui/hover-card";
import React from "react";
import { Slider } from "./ui/slider";

export default function VolumeControl({ volume, isMuted, onVolumeChange, onClick }: { volume: number; isMuted: boolean; onVolumeChange: (volume: number) => void; onClick: () => void }) {
    return (
        <div className="h-full flex w-6">
            <HoverCard openDelay={0} closeDelay={0}>
                <HoverCardTrigger onClick={onClick}>
                    {isMuted ? <VolumeOff /> : volume === 0 ? <VolumeX /> : volume < 0.5 ? <Volume1 /> : <Volume2 />}
                </HoverCardTrigger>
                <HoverCardContent className="p-4 w-full rounded-none -mb-2 bg-zinc-900">
                    <Slider orientation="vertical" className="h-20" min={0} max={100} value={[volume * 100]} onValueChange={e => onVolumeChange(e[0])} />
                </HoverCardContent>
            </HoverCard>
        </div>
    );
}
