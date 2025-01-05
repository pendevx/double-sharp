import AuthGuard from "../../components/AuthGuard"

export default function AdminLayout() {
    return (
        <AuthGuard>
            <p className="text-white">asdlfhasdjlhfjdsal</p>
        </AuthGuard>
    );
}
