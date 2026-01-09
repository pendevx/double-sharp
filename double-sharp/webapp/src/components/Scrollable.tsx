import React from "react";
import { TouchMouseEvent, TouchMouseEventType, getEventYPos, getEvents } from "../utils/eventUtils";
import debounce from "../utils/debounce";

type ScrollableProps = {
    className?: string;
    children: React.ReactNode;
    showScroller?: boolean;
    scrollTop?: number;
    onScroll?: () => void;
    smooth?: boolean;
};

export default function Scrollable({ className, children, showScroller = true, scrollTop, onScroll, smooth = false }: ScrollableProps) {
    const [showCustomScrollbar, setShowCustomScrollbar] = React.useState<boolean>(true);
    const [scrollbarHeight, setScrollbarHeight] = React.useState<number>(0);
    const [scrolling, setScrolling] = React.useState<boolean>(false);
    const [scrollPercentage, setScrollPercentage] = React.useState<number>(0);
    const containerRef = React.useRef<HTMLDivElement | null>(null);
    const contentRef = React.useRef<HTMLDivElement | null>(null);
    const preventEvent = React.useRef<boolean>(false);
    const scrollEnd = React.useMemo(() => debounce(() => (preventEvent.current = false), 50), []);

    React.useEffect(
        function () {
            function updateScrollbarHeight() {
                if (containerRef.current == null || contentRef.current == null) {
                    return;
                }

                setScrollbarHeight((containerRef.current.clientHeight * containerRef.current.clientHeight) / contentRef.current.scrollHeight);
            }

            if (
                containerRef.current != null &&
                containerRef.current.scrollHeight > containerRef.current.clientHeight &&
                getComputedStyle(containerRef.current, "::-webkit-scrollbar").display === "none"
            ) {
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
        [children]
    );

    function updateScrollTop(next: number) {
        if (!containerRef.current) {
            return;
        }

        containerRef.current.scrollTop = next;
        preventEvent.current = true;
    }

    function scrollbarTop() {
        if (!containerRef.current) {
            return 0;
        }

        return scrollPercentage * (containerRef.current.clientHeight - scrollbarHeight);
    }

    function onContainerScroll() {
        if (!containerRef.current || !contentRef.current) {
            return;
        }

        const currentDistancePercentage = containerRef.current.scrollTop / (contentRef.current.scrollHeight - containerRef.current.clientHeight);
        setScrollPercentage(currentDistancePercentage);

        if (!preventEvent.current && onScroll) {
            onScroll();
        }

        scrollEnd();
    }

    function onScrollStart(e: TouchMouseEvent, type: TouchMouseEventType) {
        if (!containerRef.current || !contentRef.current) {
            return;
        }

        const mousedownY = getEventYPos(e) - containerRef.current.getBoundingClientRect().top - scrollbarTop();
        const { move, end, cancel } = getEvents(type);

        const moveHandler = (e: Event) => onScrollMove(e as TouchMouseEvent);

        document.body.addEventListener(move, moveHandler);
        document.body.addEventListener(end, onScrollEnd);
        document.body.addEventListener(cancel, onScrollEnd);

        setScrolling(true);

        function onScrollMove(e: TouchMouseEvent) {
            if (!containerRef.current || !contentRef.current) {
                return;
            }

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
            document.body.removeEventListener(move, moveHandler);
            document.body.removeEventListener(end, onScrollEnd);
            document.body.removeEventListener(cancel, onScrollEnd);

            setScrolling(false);
            scrollEnd();
        }
    }

    if (scrollTop != null && containerRef.current != null) {
        updateScrollTop(scrollTop);
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
                        onMouseDown={e => onScrollStart(e.nativeEvent, "mouse")}
                        onTouchStart={e => onScrollStart(e.nativeEvent, "touch")}
                    />
                </span>
            )}
        </div>
    );
}
