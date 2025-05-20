import React from "react";
import { Lyrics, MusicList } from "../components";
import getViewportResolution, { ViewportResolution } from "../utils/viewportResolution";

export function Component() {
    const [showSonglist, setShowSonglist] = React.useState<boolean>(true);
    const [bodyHeight, setBodyHeight] = React.useState<number>(0);
    const bodyRef = React.useRef<HTMLDivElement | null>(null);
    console.log("hi");

    React.useEffect(() => {
        const resizeHandler = () => {
            if (bodyRef.current) {
                setBodyHeight(bodyRef.current.clientHeight);
            }
        };

        resizeHandler();

        window.addEventListener("resize", resizeHandler);

        return () => {
            window.removeEventListener("resize", resizeHandler);
        };
    }, []);

    const resolution = getViewportResolution();

    function onSongSelected() {
        if (resolution < ViewportResolution.Laptop) {
            setShowSonglist(false);
        }
    }

    function onKeyDown(e: React.KeyboardEvent) {
        if (e.key.toLowerCase() === "d") {
            setShowSonglist(!showSonglist);
        }
    }

    return (
        <div
            ref={bodyRef}
            tabIndex={1}
            onKeyDown={onKeyDown}
            className="r-0 relative flex h-full max-h-full w-full justify-end laptop:right-[33.33333%] laptop:w-[133.33333%] desktop:right-[25%] desktop:w-[125%]">
            <MusicList showSonglist={showSonglist} onSongSelected={onSongSelected} />
            <Lyrics height={bodyHeight / 2} showSonglist={showSonglist} />
        </div>
    );
}
