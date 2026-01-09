import ArtistsList from "./ArtistsList";
import { Outlet } from "react-router";
import AuthGuard from "../../components/AuthGuard";
import { Role } from "../../contexts/AuthContext";

export default function Artists() {
    return (
        <AuthGuard requiredRole={Role.User}>
            <div className="relative flex h-full gap-4">
                <div className="basis-[30%]">
                    <ArtistsList />
                </div>
                <div className="basis-[70%]">
                    <Outlet />
                </div>
            </div>
        </AuthGuard>
    );
}
