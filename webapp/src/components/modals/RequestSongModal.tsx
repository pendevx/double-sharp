import React from "react";
import ModalTemplate from "./ModalTemplate";
import { FileUpload, UrlUpload } from "./SongRequestModals";

type UploadTypes = "file" | "url";

export default function RequestSongModal() {
    const [uploadMethod, setUploadMethod] = React.useState<UploadTypes>("url");

    const switchUploadMethod = () => setUploadMethod(uploadMethod === "file" ? "url" : "file");

    return (
        <ModalTemplate>
            <h3 className="mb-4 text-xl font-semibold">Request to upload a song</h3>

            {uploadMethod === "url" ? <UrlUpload toggleUploadSource={switchUploadMethod} /> : <FileUpload toggleUploadSource={switchUploadMethod} />}
        </ModalTemplate>
    );
}
