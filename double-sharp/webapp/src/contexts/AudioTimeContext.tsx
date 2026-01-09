import React from "react";

export type AudioTimeContextType = {
    currentTime: number | null;
    setCurrentTime: React.Dispatch<React.SetStateAction<number>>;
    totalDuration: number | null;
    setTotalDuration: React.Dispatch<React.SetStateAction<number>>;
    requestTime: number | null;
    setRequestTime: React.Dispatch<React.SetStateAction<number>>;
};

export const AudioTimeContext = React.createContext<AudioTimeContextType>({} as AudioTimeContextType);

export default function AudioTimeProvider({ children }: { children: React.ReactNode }) {
    const [currentTime, setCurrentTime] = React.useState<number>(0);
    const [requestTime, setRequestTime] = React.useState<number>(0);
    const [totalDuration, setTotalDuration] = React.useState<number>(0);

    return (
        <AudioTimeContext.Provider
            value={{
                currentTime,
                setCurrentTime,
                totalDuration,
                setTotalDuration,
                requestTime,
                setRequestTime,
            }}>
            {children}
        </AudioTimeContext.Provider>
    );
}
