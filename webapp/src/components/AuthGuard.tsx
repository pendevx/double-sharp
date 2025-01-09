import React, { Suspense } from "react";
import { useNavigate } from "react-router";

export default function AuthGuard({ children }: { children: React.ReactNode }) {
    return (
        <Suspense fallback={<p className="text-white">loading!!!!!!!</p>}>
            <Requires.Admin>{children}</Requires.Admin>
        </Suspense>
    );
}

const checkRole = async (role: string): Promise<boolean> => {
    const res = await fetch(`/api/accounts/checkRole?role=${role}`);
    return res.json();
};

const AuthGuardHelperGenerator = (role: string) =>
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

const Requires = {
    Admin: AuthGuardHelperGenerator("Admin"),
    User: AuthGuardHelperGenerator("User"),
};
