import React from "react";
import { useForm } from "react-hook-form";
import { standardSchemaResolver } from "@hookform/resolvers/standard-schema";
import z from "zod/v4";
import useFetch from "../../../hooks/useFetch";

const formModel = z.object({
    title: z.string().min(1, "Title is required"),
    file: z
        .instanceof(FileList)
        .transform(val => (val instanceof FileList && val.length === 1 ? val[0] : null))
        .refine(file => file instanceof File && file.size > 0, "File is required"),
});

type FormModel = z.infer<typeof formModel>;

type UploaderProps = {
    toggleUploadSource?: () => void;
    postSubmit?: (response: number) => Promise<void>;
};

export default function FileUpload({ toggleUploadSource, postSubmit }: UploaderProps) {
    const [fileName, setFileName] = React.useState<string>("Choose a file");
    const [submitting, setSubmitting] = React.useState<boolean>(false);
    const { refreshData } = useFetch<any>();
    const { register, handleSubmit, formState } = useForm<FormModel>({ resolver: standardSchemaResolver(formModel) });

    const onSubmit = async (data: FormModel) => {
        setSubmitting(true);

        const guid = (await refreshData("/api/song-requests/request/file", {
            method: "POST",
            body: data.file,
            headers: { "Content-Type": data.file?.type ?? "" },
        })) as string;

        const uploadData = {
            id: guid,
            title: data.title,
        };

        const id = (await refreshData("/api/song-requests/request/file-details", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=UTF-8" },
            body: JSON.stringify(uploadData),
        })) as number;

        await postSubmit?.(id);
        setSubmitting(false);
    };

    const { ref, onChange, ...fileInputProps } = register("file", { required: true });

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="mt-2 flex flex-col gap-4">
            <div>
                <RaisedInputPlaceholder
                    className="block w-full rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white p-2"
                    inputClass="text-white bg-[#080808] px-2 h-fit bottom-0 top-0 "
                    placeholder="Title"
                    required
                    type="text"
                    {...register("title", { required: true })}
                />
            </div>

            <label className="relative bottom-0 top-0 block h-40 rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white p-2 text-white">
                <input
                    type="file"
                    required
                    className="absolute inset-0 hidden"
                    ref={ref}
                    {...fileInputProps}
                    onChange={e => {
                        onChange(e);
                        const file = e.target.files?.[0];
                        if (file) setFileName(file.name);
                    }}
                />

                <div className="absolute inset-0 flex items-center justify-center text-xl">{fileName}</div>
            </label>

            <div className="mt-4 grid w-full grid-cols-2 gap-4">
                <button
                    type="reset"
                    onClick={toggleUploadSource}
                    className="w-full cursor-pointer rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white bg-[#333] p-2 text-center text-white">
                    Switch to URL Upload
                </button>
                <button
                    type="submit"
                    className="w-full cursor-pointer rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white bg-[#004317] p-2 text-center text-white disabled:cursor-not-allowed disabled:bg-[#355c42] [&:hover:not(:disabled)]:bg-[#117b38]"
                    disabled={!formState.isValid}>
                    {submitting ? "Submitting..." : "Submit Song Request"}
                </button>
            </div>
        </form>
    );
}

type RaisedInputPlaceholderProps = {
    className?: string;
    placeholder: string;
    inputClass?: string;
    onchange?: (id: number, isValid: boolean, key: string, e: React.ChangeEvent<HTMLInputElement>) => void;
    before?: React.ReactNode;
} & React.InputHTMLAttributes<HTMLInputElement>;

function RaisedInputPlaceholder({ className, placeholder, inputClass, onchange, before, ...props }: RaisedInputPlaceholderProps) {
    const [value, setValue] = React.useState<string>("");

    const onChangeHandler = (e: React.ChangeEvent<HTMLInputElement>) => {
        props.onChange?.(e);
        setValue(e.target.value);
    };

    return (
        <label className={`group relative cursor-text ${className}`}>
            <span
                className={`absolute my-auto origin-left cursor-text px-2 align-bottom transition-transform duration-300 group-focus-within:-translate-y-5 group-focus-within:scale-90 ${inputClass} ${value.length > 0 && "-translate-y-5 scale-90"}`}>
                {placeholder}
            </span>
            <div className={`h-full w-full cursor-text ${inputClass} flex px-2`}>
                {before}
                <input className={`${inputClass} grow`} {...props} onChange={onChangeHandler} />
            </div>
        </label>
    );
}
