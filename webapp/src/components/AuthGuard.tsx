import React, { Suspense } from "react";
import { useNavigate } from "react-router";
import { AuthContext, Role } from "../contexts/AuthContext";

export default function AuthGuard({ children, requiredRole }: { children: React.ReactNode; requiredRole: Role }) {
    const RoleValidator = AuthGuardHelperGenerator(requiredRole);

    return (
        <Suspense>
            <RoleValidator>{children}</RoleValidator>
        </Suspense>
    );
}

const AuthGuardHelperGenerator = (role: Role) => {
    const authContext = React.useContext(AuthContext);
    const hasRole = authContext.hasRole(role);

    if (hasRole) {
        return ({ children }: { children: React.ReactNode }) => <>{children}</>;
    }

    return React.lazy(async () => {
        const hasRole = await authContext.checkRoleAsync(role);
        return {
            default: ({ children }: { children: React.ReactNode }) => {
                const navigate = useNavigate();

                React.useEffect(() => {
                    if (hasRole === false) {
                        navigate("/");
                    }
                }, []);

                return <>{hasRole && children}</>;
            },
        };
    });
};
