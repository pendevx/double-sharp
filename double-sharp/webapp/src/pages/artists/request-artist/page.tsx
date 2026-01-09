import z from "zod/v4";
import React from "react";
import { useForm } from "react-hook-form";
import { standardSchemaResolver } from "@hookform/resolvers/standard-schema";
import useFetch from "../../../hooks/useFetch";

const formModel = z.object({
    name: z.string().min(1, "Name is required"),
    dateOfBirth: z.string().min(1, "Date of Birth is required"),
});

type FormModel = z.infer<typeof formModel>;

export default function RequestArtistForm() {
    const [submitting, setSubmitting] = React.useState<boolean>(false);
    const { register, handleSubmit, formState, getFieldState } = useForm<FormModel>({ resolver: standardSchemaResolver(formModel) });
    const { refreshData } = useFetch();

    const onSubmit = async (data: FormModel) => {
        setSubmitting(true);

        await refreshData("/api/artists/suggest", {
            method: "POST",
            headers: { "Content-Type": "application/json; charset=UTF-8" },
            body: JSON.stringify(data),
        });

        setSubmitting(false);
    };

    return (
        <div className="flex flex-col items-center gap-12 pt-8">
            <h3 className="mb-4 text-center text-xl font-semibold">Suggest a new Artist</h3>

            <form onSubmit={handleSubmit(onSubmit)} onKeyDown={e => e.stopPropagation} className="flex w-3/5 flex-col gap-4">
                <RaisedInputPlaceholder
                    className={`block w-full rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white p-2 ${getFieldState("name").invalid ? "border-red-600" : ""}`}
                    inputClass="text-white bg-[#080808] h-fit bottom-0 top-0"
                    placeholder="Name"
                    required
                    type="text"
                    {...register("name", { required: true })}
                />

                <RaisedInputPlaceholder
                    className={`block w-full rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white p-2 ${getFieldState("dateOfBirth").invalid ? "border-red-600" : ""}`}
                    inputClass="text-white bg-[#080808] h-fit bottom-0 top-0"
                    placeholder="Date of Birth"
                    required
                    type="date"
                    {...register("dateOfBirth", { required: true })}
                />

                <button
                    type="submit"
                    className="w-full cursor-pointer rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white bg-[#004317] p-2 text-center text-white disabled:cursor-not-allowed disabled:bg-[#355c42] [&:hover:not(:disabled)]:bg-[#117b38]"
                    disabled={!formState.isValid || submitting}>
                    {submitting ? "Submitting..." : "Suggest Artist"}
                </button>
            </form>
        </div>
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
