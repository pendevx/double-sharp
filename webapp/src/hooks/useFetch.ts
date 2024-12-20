import React from "react";

export default function useFetch<T>(fallbackValue?: T, defaultUrl?: string) {
    const [data, setData] = React.useState<T | undefined>(fallbackValue);
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

            console.log(url, defaultUrl);

            if (!url && !defaultUrl) {
                setData(fallbackValue);
                setError(new Error("A URL must be provided."));
                return;
            }

            const finalUrl = url || (defaultUrl as string);

            setIsFetching(true);

            try {
                const fetchOptions: any = {
                    ...options,
                    headers: Object.assign({ "Content-Type": "application/json" }, options.headers),
                    signal: aborter.current?.signal,
                };

                const response = await fetch(finalUrl, fetchOptions);

                if (!response.ok) {
                    throw new Error(`Failed to fetch data: ${response.statusText}`);
                }

                const json: T = await response.json();
                setData(json);
                return json;
            } catch (err) {
                setError(err as Error);
                setData(fallbackValue);
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
