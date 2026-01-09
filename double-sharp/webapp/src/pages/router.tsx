import { lazy } from "react";
import { createBrowserRouter, createRoutesFromElements, Route, RouterProvider } from "react-router";
import RootLayout from "./Layout";
import AuthProvider from "../contexts/AuthContext";

const Index = lazy(() => import("@pages/page.tsx"));

// Public Pages
const Artists = lazy(() => import("@pages/artists/page.tsx"));
const ArtistInfo = lazy(() => import("@pages/artists/artist-info/page.tsx"));
const RequestArtist = lazy(() => import("@pages/artists/request-artist/page.tsx"));

// Admin Pages
const SongRequests = lazy(() => import("@pages/admin/song-requests/page.tsx"));
const UploadSongs = lazy(() => import("@pages/admin/upload-song/page.tsx"));

const AdminLayout = lazy(() => import("@pages/admin/Layout.tsx"));

const routes = createRoutesFromElements(
    <Route
        element={
            <AuthProvider>
                <RootLayout />
            </AuthProvider>
        }>
        <Route index element={<Index />} />

        <Route path="artists" element={<Artists />}>
            <Route path=":artistId" element={<ArtistInfo />} />
            <Route path="request-artist" element={<RequestArtist />} />
        </Route>

        <Route path="admin" element={<AdminLayout />}>
            <Route path="song-requests" element={<SongRequests />} />
            <Route path="upload-song" element={<UploadSongs />} />
        </Route>
    </Route>
);

const router = createBrowserRouter(routes);

export default <RouterProvider router={router} />;
