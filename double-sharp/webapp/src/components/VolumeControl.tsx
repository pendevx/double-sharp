import { Volume1, Volume2, VolumeOff, VolumeX } from "lucide-react";
import { HoverCard, HoverCardContent, HoverCardTrigger } from "@ui/hover-card";
import React from "react";
import { Slider } from "./ui/slider";
import { MusicContext } from "@/contexts/MusicContext";

export default function VolumeControl() {
    const musicContext = React.useContext(MusicContext);

    return (
        <div className="flex h-full w-12">
            <HoverCard openDelay={0} closeDelay={0}>
                <HoverCardTrigger onClick={musicContext.toggleMuted} className="flex w-full items-center justify-center">
                    {musicContext.isMuted ? <VolumeOff /> : musicContext.volume === 0 ? <VolumeX /> : musicContext.volume < 0.5 ? <Volume1 /> : <Volume2 />}
                </HoverCardTrigger>
                <HoverCardContent className="flex w-16 flex-col items-center gap-2 rounded-none bg-zinc-900 p-4" side="top" sideOffset={0}>
                    <Slider orientation="vertical" className="h-20" min={0} max={1} step={0.01} value={[musicContext.volume]} onValueChange={e => musicContext.setVolume(e[0])} />
                    <aside className="text-white">{Math.round(musicContext.volume * 100)}</aside>
                </HoverCardContent>
            </HoverCard>
        </div>
    );
}
