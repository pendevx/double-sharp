import React from "react";

type BlurredModalProps = {
    show: boolean;
    transitionDurationMs?: number;
    children?: React.ReactNode;
};

export default function BlurredModal({ show, transitionDurationMs = 1000, children }: BlurredModalProps) {
    const ref = React.useRef<HTMLDivElement | null>(null);
    const hideTimer = React.useRef<number>(-1);

    React.useEffect(() => {
        if (show && ref.current) {
            ref.current.classList.remove("hidden");
            setTimeout(() => ref.current?.classList.add("opacity-100"));
            clearTimeout(hideTimer.current);

            return;
        }

        if (ref.current?.classList.contains("opacity-100")) {
            hideTimer.current = setTimeout(() => ref.current?.classList.add("hidden"), transitionDurationMs);
            ref.current?.classList.remove("opacity-100");
        }
    }, [show]);

    // children is placed after modal so that it can be rendered on top of the modal, with separate animation.
    // children is allowed, so that the modal can be used as a wrapper to show conceptual relationship between the modal and the modal content.
    return (
        <>
            <div ref={ref} className="fixed inset-0 z-10 hidden bg-[#0008] opacity-0 transition-all" style={{ transitionDuration: transitionDurationMs + "ms", backdropFilter: "blur(5px)" }} />
            {children}
        </>
    );
}
