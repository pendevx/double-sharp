import { Suspense } from "react";
import { createBrowserRouter, createRoutesFromElements, Route, RouterProvider } from "react-router";
import RootLayout from "../App";

export const routeMap = {
    home: "@pages/page.tsx",
    admin: "@pages/admin/page.tsx",
};

const routes = createRoutesFromElements(
    <Route element={<RootLayout />}>
        <Route index lazy={() => import("@pages/page.tsx")} />
        <Route path="admin" lazy={() => import("@pages/admin/page.tsx")} />
    </Route>
);

const router = createBrowserRouter(routes);

export default (
    <Suspense fallback={<p className="text-white">page loading...</p>}>
        <RouterProvider router={router} />
    </Suspense>
);
