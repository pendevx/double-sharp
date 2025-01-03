import { Suspense } from "react";
import { createBrowserRouter, LazyRouteFunction, RouteObject, RouterProvider } from "react-router";

const lazy = (path: string, exportName: string = "default"): LazyRouteFunction<RouteObject> =>
    async () => {
        const module = await import(/* @vite-ignore */ path);
        return { Component: module[exportName] };
    }

const routes : RouteObject[] = [
    {
        path: "/",
        lazy: lazy("./page"),
    },
    {
        path: "/admin",
        lazy: lazy("./admin/page"),
    }
];

const router = createBrowserRouter(routes);

export default (
    <Suspense fallback={<p className="text-white">page loading...</p>}>
       <RouterProvider router={router} />
    </Suspense>
);
