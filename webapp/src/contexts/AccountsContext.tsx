import React from "react";
import useFetch from "../hooks/useFetch";
import { loginAccount, logoutAccount, registerAccount, userDetails } from "../utils/url-builder.api";

type Credentials = {
    username: string;
    password: string;
};

type RegisterInformation = {
    username: string;
    password: string;
} & UserInformation;

type TAccountsContext = {
    register: (registrationInfo: RegisterInformation) => void;
    login: (credentials: Credentials) => void;
    logout: () => void;
    userInformation: UserInformation | null;
};

export type UserInformation = {
    displayName: string;
};

export const AccountsContext = React.createContext<TAccountsContext>({} as TAccountsContext);

export default function AccountsProvider({ children }: { children?: React.ReactNode }) {
    const { data, refreshData } = useFetch<UserInformation | null>(null);

    React.useEffect(() => {
        refreshData(userDetails());
    }, []);

    const register = (registrationInfo: RegisterInformation) => {
        refreshData(registerAccount(), {
            method: "POST",
            body: JSON.stringify({ ...registrationInfo }),
        });
    };

    const login = (credentials: Credentials) => {
        refreshData(loginAccount(), {
            method: "POST",
            credentials: "include",
            body: JSON.stringify({ ...credentials }),
        });
    };

    const logout = () => {
        refreshData(logoutAccount(), {
            method: "POST",
        });
    };

    return (
        <AccountsContext.Provider
            value={{
                userInformation: data,
                register,
                login,
                logout,
            }}>
            {children}
        </AccountsContext.Provider>
    );
}
