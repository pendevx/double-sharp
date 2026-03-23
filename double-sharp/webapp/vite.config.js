import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/postcss";
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
        alias: [
            { find: "@pages", replacement: path.resolve(__dirname, "src/pages") },
            {
                find: "@",
                replacement: path.resolve(__dirname, "src"),
            },
            {
                find: "@ui",
                replacement: path.resolve(__dirname, "src/components/ui"),
            },
        ],
    },
    server: {
        host: "0.0.0.0",
    },
});
