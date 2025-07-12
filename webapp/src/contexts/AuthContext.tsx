import React from "react";

export enum Role {
    User,
    Admin,
}

export type AuthContextType = {
    hasRole: (role: Role) => boolean | undefined;
    checkRoleAsync: (role: Role) => Promise<boolean>;
};

export const AuthContext = React.createContext<AuthContextType>({} as AuthContextType);

const roleMap = new Map<Role, boolean>();

export default function AuthProvider({ children }: { children: React.ReactNode }) {
    const checkRoleAsync: AuthContextType["checkRoleAsync"] = async role => {
        if (roleMap.has(role)) {
            return roleMap.get(role) as boolean;
        }

        const hasRole = await fetch(`/api/accounts/checkUserHasRole/${role}`).then(res => res.json());

        roleMap.set(role, hasRole);

        return hasRole;
    };

    const hasRole: AuthContextType["hasRole"] = role => {
        if (roleMap.has(role)) {
            return roleMap.get(role) as boolean;
        }

        return undefined;
    };

    return <AuthContext.Provider value={{ checkRoleAsync, hasRole }}>{children}</AuthContext.Provider>;
}
