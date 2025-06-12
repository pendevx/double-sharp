import { useNavigate } from "react-router";
import AuthGuard from "../../components/AuthGuard";
import { Role } from "../../contexts/AuthContext";

export default function AdminPage() {
    const navigate = useNavigate();

    return (
        <AuthGuard requiredRole={Role.Admin}>
            <button onClick={() => navigate("/")} className="mb-4 w-fit bg-gray-400 p-4 text-white">
                back to home
            </button>
        </AuthGuard>
    );
}
