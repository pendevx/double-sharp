import AuthGuard, { Role } from "../../components/AuthGuard";

export default function AdminLayout() {
    return (
        <AuthGuard requiredRole={Role.Admin}>
            <p className="text-white">asdlfhasdjlhfjdsal</p>
        </AuthGuard>
    );
}
