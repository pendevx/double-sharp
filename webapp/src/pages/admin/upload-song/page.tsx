import { FileUpload } from "../../../components/modals/SongRequestModals";

export default function UploadSong() {
    const approveSong = async (response: Response) => {
        if (!response.ok) return;

        const id = await response.json();
        await fetch(`/api/song-requests/approve/${id}`, {
            method: "POST",
        });
    };

    return (
        <>
            <h3 className="mb-2 text-center text-2xl font-semibold">Upload Song</h3>
            <FileUpload postSubmit={approveSong} />
        </>
    );
}
