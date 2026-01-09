import React from "react";

export const processRequestUrl = (url: string) => (url.startsWith("/api") ? `${import.meta.env.VITE_BACKEND_URL}${url.replace("/api", "")}` : "");

export default function useFetch<T>(fallbackValue?: T, defaultUrl?: string) {
    const fallback = fallbackValue as T;

    const [data, setData] = React.useState<T>(fallback);
    const [error, setError] = React.useState<Error | null>(null);
    const [isFetching, setIsFetching] = React.useState<boolean>(false);
    const aborter = React.useRef<AbortController | null>(null);
    const refreshData = React.useCallback(
        /**
         * Fetches data. Any incomplete requests at the time of a new request being sent will will be aborted.
         * @param {string} url The URL to request
         * @param {object} options Fetch API options object
         * @returns A promise resolving to the returned data as a JSON object
         */
        async function (url: string | null = null, options: RequestInit = {}) {
            setError(null);

            aborter.current && aborter.current.abort();
            aborter.current = new AbortController();

            if (!url && !defaultUrl) {
                setData(fallback);
                setError(new Error("A URL must be provided."));
                return;
            }

            const finalUrl = url || (defaultUrl as string);

            setIsFetching(true);

            try {
                const fetchOptions: RequestInit = {
                    ...options,
                    method: options.method ?? "GET",
                    credentials: "include",
                    signal: aborter.current?.signal,
                };

                if (!["GET", "DELETE"].includes(fetchOptions.method as string)) {
                    // @ts-ignore
                    if (fetchOptions.headers?.["Content-Type"] === undefined && !!fetchOptions.body) {
                        fetchOptions.headers = Object.assign(fetchOptions.headers ?? {}, {
                            "Content-Type": "application/json; charset=UTF-8",
                        });
                    }
                }

                fetchOptions.headers = Object.assign(fetchOptions.headers ?? {}, {
                    Authorization: localStorage.getItem("token"),
                });

                const response = await fetch(processRequestUrl(finalUrl), fetchOptions);
                const json: T = await response.json();
                setData(json);
                return json;
            } catch (err) {
                setError(err as Error);
                setData(fallback);
            } finally {
                setIsFetching(false);
            }
        },
        []
    );

    return {
        data,
        error,
        isFetching,
        refreshData,
        setData,
    };
}
