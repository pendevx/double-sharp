import React, { Suspense } from "react";
import { useNavigate } from "react-router";

export enum Role {
    User,
    Admin,
}

export default function AuthGuard({ children, requiredRole }: { children: React.ReactNode; requiredRole: Role }) {
    const RoleValidator = AuthGuardHelperGenerator(requiredRole);

    return (
        <Suspense fallback={<p className="text-white">loading!!!!!!!</p>}>
            <RoleValidator>{children}</RoleValidator>
        </Suspense>
    );
}

const roleEndpoints = {
    [Role.User]: "checkUserIsUser",
    [Role.Admin]: "/checkUserIsAdmin",
}

const checkRole = async (role: Role): Promise<boolean> => {
    const res = await fetch(`/api/accounts/${roleEndpoints[role]}`);
    return res.json();
};

const AuthGuardHelperGenerator = (role: Role) =>
    React.lazy(() =>
        checkRole(role).then(p => {
            return {
                default: ({ children }: { children: React.ReactNode }) => {
                    const navigate = useNavigate();

                    React.useEffect(() => {
                        if (p === false) {
                            navigate("/");
                        }
                    }, []);

                    return <>{p && children}</>;
                },
            };
        })
    );
