import useFetch from "../../../hooks/useFetch";
import { FileUpload } from "../../../components/modals/SongRequestModals";

export default function UploadSong() {
    const { refreshData } = useFetch();

    const approveSong = async (id: number) => {
        await refreshData(`/api/song-requests/approve/${id}`, {
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
