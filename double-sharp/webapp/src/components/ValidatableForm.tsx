import React from "react";
import RaisedInputPlaceholder from "./RaisedInputPlaceholder";

type ValidatableFormProps = {
    fields: React.InputHTMLAttributes<HTMLInputElement>[];
    reportValidity: ReportValidity;
    className?: string;
    children?: React.ReactNode;
};

export type ReportValidity = {
    (subformValid: boolean): void;
};

export default function ValidatableForm({ fields, reportValidity, className, children }: ValidatableFormProps) {
    const [valid, setValid] = React.useState<boolean[]>(Array(fields.length).fill(false));

    const checkValidity = (id: number, v: boolean) => {
        const newValid = [...valid];
        newValid[id] = v;
        setValid(newValid);

        reportValidity(newValid.every(x => x));
    };

    return (
        <div className={className}>
            {fields.map((field, i) => (
                <RaisedInputPlaceholder
                    key={i}
                    className="block w-full rounded-br-lg rounded-tl-lg border-[1px] border-solid border-white p-2"
                    colorClass="text-white bg-[#080808]"
                    onchange={checkValidity}
                    index={i}
                    {...field}
                />
            ))}

            {children}
        </div>
    );
}
