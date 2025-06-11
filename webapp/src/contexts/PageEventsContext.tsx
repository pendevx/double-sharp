import React from "react";

export type PageEventsContextType = {
    onKeyDown: (e: React.KeyboardEvent) => void;
    onKeyUp: (e: React.KeyboardEvent) => void;
};

type PageEventsContextTypeWithListeners = {
    [K in keyof PageEventsContextType]: PageEventsContextType[K][];
};

export const PageEventsContext = React.createContext<any>({});

export default function PageEventsProvider({ children }: { children: React.ReactNode }) {
    const [listeners, setListeners] = React.useState<{ [K in keyof PageEventsContextType]: PageEventsContextTypeWithListeners[K] }>({} as PageEventsContextTypeWithListeners);

    const addListener = <T extends keyof PageEventsContextType>(event: T, listener: PageEventsContextType[T]) => setListeners(prev => ({ ...prev, ...listener }));
    const removeListener = <T extends keyof PageEventsContextType>(event: T, listener: PageEventsContextType[T]) =>
        setListeners(prev => ({ ...prev, [event]: prev[event].filter(l => l !== listener) }));

    const invokeListeners = (event: keyof PageEventsContextType, e: React.KeyboardEvent) => listeners[event]?.forEach((listener: (e: React.KeyboardEvent) => void) => listener(e));

    return (
        <PageEventsContext.Provider value={{ addListener, removeListener }}>
            <div onKeyDown={e => invokeListeners("onKeyDown", e)} onKeyUp={e => invokeListeners("onKeyUp", e)} tabIndex={0}>
                {children}
            </div>
        </PageEventsContext.Provider>
    );
}
