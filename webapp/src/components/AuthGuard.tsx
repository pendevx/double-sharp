import React, { Suspense } from "react";
import { useNavigate } from "react-router";

export default function AuthGuard({ children, requiredRole }: { children: React.ReactNode; requiredRole: Role }) {
    const RoleValidator = AuthGuardHelperGenerator(requiredRole);

    return (
        <Suspense fallback={<p className="text-white">loading!!!!!!!</p>}>
            <RoleValidator>{children}</RoleValidator>
        </Suspense>
    );
}

const checkRole = async (role: string): Promise<boolean> => {
    const res = await fetch(`/api/accounts/checkRole?role=${role}`);
    return res.json();
};

const AuthGuardHelperGenerator = (role: Role) =>
    React.lazy(() =>
        checkRole(Role[role]).then(p => {
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

export enum Role {
    User,
    Admin,
}
