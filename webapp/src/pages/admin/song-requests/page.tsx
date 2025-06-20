import React from "react";
import useFetch from "../../../hooks/useFetch";

type SongRequest = {
    id: number;
    name: string;
    source: string;
    sourceUrl: string;
    uploadedBy: string;
};

export default function SongRequests() {
    const { data, refreshData } = useFetch<SongRequest[]>([], "/api/song-requests/1");

    React.useEffect(() => {
        refreshData();
    }, []);

    return (
        <div className="grid grid-cols-4 gap-4">
            {data.map(songRequest => (
                <div key={songRequest.id} className="flex flex-col gap-2 rounded-lg bg-gray-800 p-4">
                    <h3 className="text-lg font-semibold text-white">{songRequest.name}</h3>
                    <div className="text-sm text-gray-400">
                        <p>Requested by: {songRequest.uploadedBy}</p>
                        <p>Source: {songRequest.source}</p>
                    </div>
                    <a href={songRequest.sourceUrl} className="block text-blue-400 hover:underline" target="_blank" rel="noopener noreferrer">
                        Listen
                    </a>
                </div>
            ))}
        </div>
    );
}
