import React from "react";
import { getEventYPos, getEvents } from "../utils/eventUtils";
import debounce from "../utils/debounce";

export default function Scrollable({ className, children, showScroller = true, scrollTop, onScroll, smooth = false }) {
    const [showCustomScrollbar, setShowCustomScrollbar] = React.useState(true);
    const [scrollbarHeight, setScrollbarHeight] = React.useState(0);
    const [scrolling, setScrolling] = React.useState(false);
    const [scrollPercentage, setScrollPercentage] = React.useState(0);
    const containerRef = React.useRef(null);
    const contentRef = React.useRef(null);
    const preventEvent = React.useRef(false);
    const scrollEnd = React.useCallback(
        debounce(() => {
            console.log("set to false");
            preventEvent.current = false;
        }, 50),
        []
    );

    React.useEffect(
        function () {
            function updateScrollbarHeight() {
                setScrollbarHeight((containerRef.current.clientHeight * containerRef.current.clientHeight) / contentRef.current.scrollHeight);
            }

            if (containerRef.current.scrollHeight > containerRef.current.clientHeight && getComputedStyle(containerRef.current, "::-webkit-scrollbar").display === "none") {
                setShowCustomScrollbar(true);
                updateScrollbarHeight();
            } else {
                setShowCustomScrollbar(false);
                return;
            }

            window.addEventListener("resize", updateScrollbarHeight);

            return function () {
                window.removeEventListener("resize", updateScrollbarHeight);
            };
        },
        [showScroller]
    );

    function setScrollTop(next) {
        containerRef.current.scrollTop = next;
        preventEvent.current = true;
    }

    React.useEffect(
        function () {
            if (scrollTop != null) {
                setScrollTop(scrollTop);
            }
        },
        [scrollTop]
    );

    function scrollbarTop() {
        return scrollPercentage * (containerRef.current?.clientHeight - scrollbarHeight) || 0;
    }

    function onContainerScroll() {
        const currentDistancePercentage = containerRef.current.scrollTop / (contentRef.current.scrollHeight - containerRef.current.clientHeight);
        setScrollPercentage(currentDistancePercentage);

        if (!preventEvent.current) {
            onScroll && onScroll();
        }

        scrollEnd();
    }

    function onScrollStart(e, type) {
        const mousedownY = getEventYPos(e) - containerRef.current.getBoundingClientRect().top - scrollbarTop();
        const { move, end, cancel } = getEvents(type);

        document.body.addEventListener(move, onScrollMove);
        document.body.addEventListener(end, onScrollEnd);
        document.body.addEventListener(cancel, onScrollEnd);

        setScrolling(true);

        function onScrollMove(e) {
            const bottomHeight = containerRef.current.scrollHeight - containerRef.current.clientHeight;
            const totalDistance = containerRef.current.clientHeight - scrollbarHeight;
            const distanceFromOrigin = getEventYPos(e) - containerRef.current.getBoundingClientRect().top - mousedownY;
            const currentDistancePercentage = distanceFromOrigin / totalDistance;
            const nextScrollTop = currentDistancePercentage * (contentRef.current.scrollHeight - containerRef.current.clientHeight);

            let nextScrollPercentage = currentDistancePercentage;

            if (nextScrollTop < 0) {
                nextScrollPercentage = 0;
            } else if (nextScrollTop > bottomHeight) {
                nextScrollPercentage = 1;
            }

            setScrollPercentage(nextScrollPercentage);
            containerRef.current.scrollTop = nextScrollTop;
            preventEvent.current = false;

            scrollEnd();
        }

        function onScrollEnd() {
            document.body.removeEventListener(move, onScrollMove);
            document.body.removeEventListener(end, onScrollEnd);
            document.body.removeEventListener(cancel, onScrollEnd);

            setScrolling(false);
            scrollEnd();
        }
    }

    return (
        <div className={`${className} flex grow gap-2`}>
            <div ref={containerRef} className={`grow overflow-auto ${smooth && "scroll-smooth"}`} onScroll={onContainerScroll}>
                <div ref={contentRef}>{children}</div>
            </div>

            {showCustomScrollbar && showScroller && (
                <span className="sticky top-0 h-full min-w-2">
                    <i className="absolute inset-0 ml-auto mr-auto w-[1px] bg-[#333]" />
                    <i
                        className={`absolute z-10 block w-full rounded-xl bg-[#666] transition-colors duration-100 hover:bg-[#aaa] ${scrolling ? "bg-[#aaa]" : ""}`}
                        style={{
                            height: scrollbarHeight || 0,
                            top: scrollbarTop(),
                        }}
                        onMouseDown={e => onScrollStart(e, "mouse")}
                        onTouchStart={e => onScrollStart(e, "touch")}
                    />
                </span>
            )}
        </div>
    );
}
