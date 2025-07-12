import React from "react";
import z from "zod/v4";
import { standardSchemaResolver } from "@hookform/resolvers/standard-schema";
import { useForm } from "react-hook-form";

const options = [
    {
        display: "YouTube",
        value: "YouTube",
        prefix: "https://www.youtube.com/watch?v=",
    },
    {
        display: "YouTube Music",
        value: "YouTubeMusic",
        prefix: "https://music.youtube.com/watch?v=",
    },
];

const formModel = z.object({
    title: z.string().min(1, "Title is required"),
    url: z.string().min(1, "URL is required"),
    source: z.enum(options.map(o => o.value)),
});

type FormModel = z.infer<typeof formModel>;

type UploaderProps = {
    toggleUploadSource: () => void;
};

export default function UrlUpload({ toggleUploadSource }: UploaderProps) {
    const [currentSource, setCurrentSource] = React.useState<string>("Please select");
    const [submitting, setSubmitting] = React.useState<boolean>(false);

    const { register, handleSubmit, formState, getFieldState } = useForm<FormModel>({ resolver: standardSchemaResolver(formModel) });

    const submitSongRequest = async (data: FormModel) => {
        setSubmitting(true);

        await fetch("/api/song-requests/request/url", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=UTF-8" },
            body: JSON.stringify({
                name: data.title,
                url: data.url,
                source: data.source,
            }),
        });

        setSubmitting(false);
    };

    const source = options.find(o => o.value === currentSource);

    const { ref, onChange: onSourceChange, ...sourceProps } = register("source", { required: true });

    return (
        <form onSubmit={handleSubmit(submitSongRequest)} onKeyDown={e => e.stopPropagation()}>
            <div className="flex flex-col gap-4">
                <RaisedInputPlaceholder
                    className={`block w-full rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white p-2 ${getFieldState("title").invalid ? "border-red-600" : ""}`}
                    inputClass="text-white bg-[#080808] h-fit bottom-0 top-0"
                    placeholder="Title"
                    required
                    type="text"
                    {...register("title", { required: true })}
                />

                <div className="grid grid-cols-2 gap-4">
                    <RaisedInputPlaceholder
                        className={`block w-full rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white p-2 ${getFieldState("url").invalid ? "border-red-600" : ""}`}
                        inputClass="text-white bg-[#080808] h-fit bottom-0 top-0 disabled:opacity-50 disabled:cursor-not-allowed pointer-events-none disabled:pointer-events-auto"
                        placeholder="URL"
                        required
                        type="text"
                        before={<span className="text-gray-400 [line-height:normal]">{source?.prefix}</span>}
                        forceRaise={!!source?.prefix}
                        disabled={!source}
                        title="Please select a source first"
                        {...register("url", { required: true })}
                    />

                    <label className="rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white pr-2">
                        <select
                            className="block w-full p-2 text-white"
                            {...register("source", { required: true })}
                            ref={ref}
                            onChange={e => {
                                onSourceChange(e);
                                setCurrentSource(e.target.value);
                            }}
                            {...sourceProps}>
                            <option className="text-black">Please select</option>
                            {options.map(option => (
                                <option key={option.value} className="text-black" value={option.value}>
                                    {option.display}
                                </option>
                            ))}
                        </select>
                    </label>
                </div>
            </div>

            <div className="mt-4 grid w-full grid-cols-2 gap-4">
                <button
                    type="reset"
                    onClick={toggleUploadSource}
                    className="w-full cursor-pointer rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white bg-[#333] p-2 text-center text-white">
                    Switch to File Upload
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
    forceRaise?: boolean;
} & React.InputHTMLAttributes<HTMLInputElement>;

function RaisedInputPlaceholder({ className, placeholder, inputClass, onchange, before, forceRaise, ...props }: RaisedInputPlaceholderProps) {
    const [value, setValue] = React.useState<string>("");

    const onChangeHandler = (e: React.ChangeEvent<HTMLInputElement>) => {
        props.onChange?.(e);
        setValue(e.target.value);
    };

    return (
        <label className={`group relative cursor-text ${className}`}>
            <span
                className={`absolute my-auto origin-left cursor-text px-2 align-bottom transition-transform duration-300 group-focus-within:-translate-y-5 group-focus-within:scale-90 ${inputClass} ${(forceRaise || value.length > 0) && "-translate-y-5 scale-90"}`}>
                {placeholder}
            </span>
            <div className={`h-full w-full cursor-text ${inputClass} flex px-2`}>
                {before}
                <input className={`${inputClass} grow`} {...props} onChange={onChangeHandler} />
            </div>
        </label>
    );
}
