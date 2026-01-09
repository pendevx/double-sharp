import React from "react";
import getViewportResolution, { ViewportResolution } from "../utils/viewportResolution";

type FullScreenOverlayProps = {
    hideFullscreen: () => void;
    children: React.ReactNode;
};

export default function FullScreenOverlay({ hideFullscreen, children }: FullScreenOverlayProps) {
    const handleBackgroundClick = (e: React.MouseEvent) => {
        const viewportResolution = getViewportResolution();

        if (viewportResolution > ViewportResolution.Tablet && e.target === e.currentTarget) {
            hideFullscreen();
        }
    };

    return (
        <div className="absolute inset-0 flex flex-col items-center text-white" onClick={handleBackgroundClick}>
            {children}
        </div>
    );
}
