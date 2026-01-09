import React from "react";

type PageEvents = {
    onKeyDown: (e: React.KeyboardEvent) => void;
    onKeyUp: (e: React.KeyboardEvent) => void;
};

type PageEventsWithListeners = {
    [K in keyof PageEvents]: PageEvents[K][];
};

export type PageEventsContextType = {
    addListener: (event: keyof PageEvents, listener: (e: React.KeyboardEvent) => void) => void;
    removeListener: (event: keyof PageEvents, listener: (e: React.KeyboardEvent) => void) => void;
};

export const PageEventsContext = React.createContext<PageEventsContextType>({} as PageEventsContextType);

export default function PageEventsProvider({ children }: { children: React.ReactNode }) {
    const [listeners, setListeners] = React.useState<PageEventsWithListeners>({} as PageEventsWithListeners);

    const addListener = React.useCallback((event: keyof PageEvents, listener: (e: React.KeyboardEvent) => void) => {
        setListeners(prev => ({
            ...prev,
            [event]: [...(prev[event] || []), listener],
        }));
    }, []);

    const removeListener = React.useCallback((event: keyof PageEvents, listener: (e: React.KeyboardEvent) => void) => {
        setListeners(prev => ({
            ...prev,
            [event]: (prev[event] || []).filter(l => l !== listener),
        }));
    }, []);

    const invokeListeners = (event: keyof PageEvents, e: React.KeyboardEvent) => listeners[event]?.forEach((listener: (e: React.KeyboardEvent) => void) => listener(e));

    return (
        <PageEventsContext.Provider value={{ addListener, removeListener }}>
            <div onKeyDown={e => invokeListeners("onKeyDown", e)} onKeyUp={e => invokeListeners("onKeyUp", e)} tabIndex={0}>
                {children}
            </div>
        </PageEventsContext.Provider>
    );
}
