import React from "react";

export const AccountSettingsContext = React.createContext<number>(1);

export default function KeyedForm({ children, ...props }: { children?: React.ReactNode } & React.FormHTMLAttributes<HTMLFormElement>) {
    const [key, setKey] = React.useState<number>(0);

    return (
        <AccountSettingsContext.Provider value={key}>
            <form {...props} onReset={() => setKey(key + 1)}>
                {children}
            </form>
        </AccountSettingsContext.Provider>
    );
}
