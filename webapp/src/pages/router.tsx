import { Suspense, lazy } from "react";
import { createBrowserRouter, createRoutesFromElements, Route, RouterProvider } from "react-router";
import RootLayout from "./Layout";
import AuthProvider from "../contexts/AuthContext";

const Index = lazy(() => import("@pages/page.tsx"));
const Admin = lazy(() => import("@pages/admin/page.tsx"));

const routes = createRoutesFromElements(
    <Route
        element={
            <AuthProvider>
                <RootLayout />
            </AuthProvider>
        }>
        <Route index element={<Index />} />
        <Route path="admin" element={<Admin />} />
    </Route>
);

const router = createBrowserRouter(routes);

export default (
    <Suspense fallback={<p className="text-white">page loading...</p>}>
        <RouterProvider router={router} />
    </Suspense>
);
