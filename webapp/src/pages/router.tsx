import { Suspense, lazy } from "react";
import { createBrowserRouter, createRoutesFromElements, Route, RouterProvider } from "react-router";
import RootLayout from "../App";

export const routeMap = {
    home: "@pages/page.tsx",
    admin: "@pages/admin/page.tsx",
};

const Index = lazy(() => import("@pages/page.tsx"));
const Admin = lazy(() => import("@pages/admin/page.tsx"));

const routes = createRoutesFromElements(
    <Route element={<RootLayout />}>
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
