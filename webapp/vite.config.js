import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "tailwindcss";
import * as path from "path";

export default defineConfig({
    plugins: [react(), tailwindcss()],
    build: {
        target: "esnext",
        rollupOptions: {
            output: {
                manualChunks: {
                    vendor: ["react", "react-dom"],
                },
            },
        },
    },
    esbuild: {
        sourcemap: true,
    },
    resolve: {
        // eslint-disable-next-line no-undef
        alias: [{ find: "@pages", replacement: path.resolve(__dirname, "src/pages") }],
    },
    server: {
        host: "0.0.0.0",
    },
});
