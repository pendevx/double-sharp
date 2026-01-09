import React from "react";
import { TouchMouseEvent, TouchMouseEventType, getEventXPos, getEvents } from "../utils/eventUtils";

const commonStyles = "absolute mt-auto mb-auto top-0 bottom-0";

type MusicProgressBarProps = {
    songDurationSecs: number;
    currentTime?: number;
    onFastForward: (secs: number) => void;
};

type TSeek = {
    isSeeking: boolean | null;
    pos: number | null;
};

export default function MusicProgressBar({ songDurationSecs, currentTime = 0, onFastForward }: MusicProgressBarProps) {
    const [seek, setSeek] = React.useState<TSeek>({ isSeeking: null, pos: null });
    const sliderRef = React.useRef<HTMLDivElement | null>(null);

    function onSeekStart(e: TouchMouseEvent, type: TouchMouseEventType) {
        if (sliderRef.current == null) {
            return;
        }

        const { move, end, cancel } = getEvents(type);
        const sliderRect = sliderRef.current.getBoundingClientRect();

        const moveHandler = (e: Event) => onSeekMove(e as TouchMouseEvent);

        document.body.addEventListener(move, moveHandler);
        document.body.addEventListener(end, onSeekEnd);
        document.body.addEventListener(cancel, onSeekEnd);

        setSeek({
            isSeeking: true,
            pos: ((getEventXPos(e) - sliderRect.left) / sliderRect.width) * songDurationSecs,
        });

        function onSeekMove(e: TouchMouseEvent) {
            const xPos = getEventXPos(e);

            let pos;

            if (xPos < sliderRect.left) {
                pos = 0;
            } else if (xPos > sliderRect.right) {
                pos = songDurationSecs;
            } else {
                pos = ((xPos - sliderRect.left) / sliderRect.width) * songDurationSecs;
            }

            setSeek(old => ({
                ...old,
                pos,
            }));
        }

        function onSeekEnd() {
            document.body.removeEventListener(move, moveHandler);
            document.body.removeEventListener(end, onSeekEnd);
            document.body.removeEventListener(cancel, onSeekEnd);

            setSeek(old => ({
                ...old,
                isSeeking: false,
            }));
        }
    }

    if (seek.isSeeking === false) {
        onFastForward(seek.pos || 0);

        setSeek({
            isSeeking: null,
            pos: null,
        });
    }

    return (
        <div className="relative block h-3 w-full" ref={sliderRef} onMouseDown={e => onSeekStart(e.nativeEvent, "mouse")} onTouchStart={e => onSeekStart(e.nativeEvent, "touch")}>
            <div className={`${commonStyles} left-0 h-1 w-full bg-gray-400`} />
            <div className={`${commonStyles} left-0 h-1 bg-[#cea127]`} style={{ width: `${((seek.pos != null ? seek.pos : currentTime) / songDurationSecs) * 100}%` }}>
                <i className={`${commonStyles} ${seek.pos != null ? "w-3 rounded-[50%]" : ""} right-0 h-3 w-1 translate-x-1/2 bg-inherit duration-200 hover:w-3 hover:rounded-[50%]`} />
            </div>
        </div>
    );
}
