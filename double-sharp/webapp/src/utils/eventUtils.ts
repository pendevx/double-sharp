export type TouchMouseEvent = MouseEvent | TouchEvent;
export type TouchMouseEventMap = {
    move: string;
    end: string;
    cancel: string;
};
export type TouchMouseEventType = "touch" | "mouse";

export function getEventXPos(e: TouchMouseEvent): number {
    if (e instanceof TouchEvent) {
        return e.touches[0].clientX;
    } else {
        return e.clientX;
    }
}

export function getEventYPos(e: TouchMouseEvent): number {
    if (e instanceof TouchEvent) {
        return e.touches[0].clientY;
    } else {
        return e.clientY;
    }
}

export function getEvents(type: TouchMouseEventType): TouchMouseEventMap {
    const map = {
        touch: {
            move: "touchmove",
            end: "touchend",
            cancel: "touchcancel",
        },
        mouse: {
            move: "mousemove",
            end: "mouseup",
            cancel: "mouseleave",
        },
    };

    if (!(type in map)) throw new Error("Invalid event type");

    return map[type];
}
