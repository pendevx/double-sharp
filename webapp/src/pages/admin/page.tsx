import AuthGuard, { Role } from "../../components/AuthGuard";

export function Component() {
    console.log("admin page loaded");
    return (
        <AuthGuard requiredRole={Role.Admin}>
            <p className="text-white">asdlfhasdjlhfjdsal</p>
        </AuthGuard>
    );
}
