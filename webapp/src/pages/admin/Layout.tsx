import { Outlet, useLocation } from "react-router";
import { Scrollable } from "../../components";
import AuthGuard from "../../components/AuthGuard";
import { Role } from "../../contexts/AuthContext";

const adminPaths = [
    {
        label: "Song Requests",
        path: "/admin/song-requests",
    },
    {
        label: "Upload Song",
        path: "/admin/upload-song",
    },
];

export default function Layout() {
    const location = useLocation();

    return (
        <div className="flex h-full gap-4 text-white">
            <Scrollable className="basis-1/4">
                <ul>
                    {adminPaths &&
                        adminPaths.map(({ label, path }) => (
                            <li
                                key={path}
                                className={`block cursor-pointer overflow-hidden text-nowrap pb-2 pl-4 pt-2 text-[#7c7c7c] transition-colors duration-200 hover:bg-[#2b2b2b] hover:text-white ${location.pathname === path ? "bg-[#2b2b2b] text-[#ffc421] mobile:hover:bg-[#2b2b2b] mobile:hover:text-[#ffc421] laptop:hover:bg-[#555555] laptop:hover:text-[#ffc421]" : ""}`}>
                                <a href={path} className="block h-full w-full">
                                    {label}
                                </a>
                            </li>
                        ))}
                </ul>
            </Scrollable>
            <Scrollable className="basis-3/4">
                <AuthGuard requiredRole={Role.Admin}>
                    <Outlet />
                </AuthGuard>
            </Scrollable>
        </div>
    );
}
