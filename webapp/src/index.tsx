import React from "react";
import ReactDOM from "react-dom/client";
import router from "@pages/router";
import "./reset.css";
import "./index.css";
import MusicProvider from "./contexts/MusicContext";
import AccountProvider from "./contexts/AccountsContext";
import AudioTimeProvider from "./contexts/AudioTimeContext";
import PageEventsProvider from "./contexts/PageEventsContext";

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
    <React.StrictMode>
        <AccountProvider>
            <MusicProvider>
                <AudioTimeProvider>
                    <PageEventsProvider>{router}</PageEventsProvider>
                </AudioTimeProvider>
            </MusicProvider>
        </AccountProvider>
    </React.StrictMode>
);
