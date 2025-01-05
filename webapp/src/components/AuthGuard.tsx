import React, { Suspense, use } from "react";
import { useNavigate } from "react-router";
import useFetch from "../hooks/useFetch";

export default function AuthGuard({ children }: { children: React.ReactNode }) {
    return (
        <Suspense fallback={<p className="text-white">loading!!!!!!!</p>}>
            <AuthGuardHelper>
                {children}
            </AuthGuardHelper>
        </Suspense>
    )
}

function AuthGuardHelper({ children }: { children: React.ReactNode }) {
    // const { data: isAuthenticated, refreshData, isFetching } = useFetch(null, "/api/accounts/checkRole?role=Admin");
    const isPermitted = use(fetch("/api/accounts/checkRole?role=Admin"));
    const navigate = useNavigate();

    // console.log(isPermitted);

    // React.useEffect(() => {
    //     refreshData();
    // }, []);

    // if (isFetching) {
    //     return <></>
    // } else if (!isFetching && isAuthenticated === false) {
    //     navigate("/");
    // } else {
    //     return <>{children}</>;
    // }

    React.useEffect(() => {
        // isPermitted.json().then(p => {
        //     console.log("hi");
        //     if (p === false) {
        //         navigate("/");
        //     }
        // })
    }, [isPermitted]);

    return children;
}
