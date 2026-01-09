import React from "react";

export default function useFileUpload() {
    const [file, setFile] = React.useState<File | null>(null);
    const [preview, setPreview] = React.useState<string | null>(null);

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files;

        if (files && files.length > 0) {
            const file = files[0];

            setFile(file);
            setPreview(URL.createObjectURL(file));
        }
    };

    return { file, preview, handleFileChange };
}
