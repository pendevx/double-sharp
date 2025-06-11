import React from "react";
import { Lyrics, MusicList } from "../components";
import getViewportResolution, { ViewportResolution } from "../utils/viewportResolution";
import { useOutletContext } from "react-router";
import { PageEventsContext } from "../contexts/PageEventsContext";

export default function IndexPage() {
    const [showSonglist, setShowSonglist] = React.useState<boolean>(true);
    const [bodyHeight, setBodyHeight] = React.useState<number>(0);
    const { keyDownSignal } = useOutletContext<any>();
    const [readingKeyDown, setReadingKeyDown] = React.useState<boolean>(false);
    const bodyRef = React.useRef<HTMLDivElement | null>(null);
    // const { addListener, removeListener } = React.useContext(PageEventsContext);

    if (!readingKeyDown && keyDownSignal) {
        setReadingKeyDown(true);

        if (keyDownSignal === "d") {
            setShowSonglist(!showSonglist);
        }
    } else if (!keyDownSignal && readingKeyDown) {
        setReadingKeyDown(false);
    }

    React.useEffect(() => {
        const resizeHandler = () => {
            if (bodyRef.current) {
                setBodyHeight(bodyRef.current.clientHeight);
            }
        };

        const onPageKeyDown = (e: KeyboardEvent) => {
            if (e.key.toLowerCase() === "d") {
                setShowSonglist(!showSonglist);
            }
        };

        resizeHandler();

        window.addEventListener("resize", resizeHandler);
        // addListener("onKeyDown", onPageKeyDown);

        return () => {
            window.removeEventListener("resize", resizeHandler);
            // removeListener("onKeyDown", onPageKeyDown);
        };
    }, []);

    const resolution = getViewportResolution();

    function onSongSelected() {
        if (resolution < ViewportResolution.Laptop) {
            setShowSonglist(false);
        }
    }

    return (
        <div ref={bodyRef} className="r-0 relative flex h-full max-h-full w-full justify-end laptop:right-[33.33333%] laptop:w-[133.33333%] desktop:right-[25%] desktop:w-[125%]">
            <MusicList showSonglist={showSonglist} onSongSelected={onSongSelected} />
            <Lyrics height={bodyHeight / 2} showSonglist={showSonglist} />
        </div>
    );
}
