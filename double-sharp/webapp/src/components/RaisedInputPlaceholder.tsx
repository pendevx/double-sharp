import React from "react";
import { AccountSettingsContext } from "./KeyedForm";

type RaisedInputPlaceholderProps = {
    colorClass?: string;
    onchange: (id: number, isValid: boolean, key: string, e: React.ChangeEvent<HTMLInputElement>) => void;
    index: number;
    before?: React.ReactNode;
    forceRaise?: boolean;
} & React.InputHTMLAttributes<HTMLInputElement>;

export default function RaisedInputPlaceholder({ className, placeholder, colorClass, onchange, index, before, forceRaise, ...props }: RaisedInputPlaceholderProps) {
    const [value, setValue] = React.useState<string>("");
    const [key, setKey] = React.useState<number>(0);
    const ctx = React.useContext(AccountSettingsContext);

    const onChangeHandler = (e: React.ChangeEvent<HTMLInputElement>) => {
        const curValue = e.target.value;

        setValue(curValue);
        const validity = e.target.checkValidity();

        if (onchange) {
            onchange(index, validity, e.target.name, e);
        }
    };

    if (key !== ctx) {
        setKey(ctx);
        setValue("");
    }

    const isRaised = forceRaise || value.length > 0;

    return (
        <label className={`group relative cursor-text ${className}`}>
            <span
                className={`absolute my-auto origin-left cursor-text px-2 align-bottom transition-transform duration-300 group-focus-within:-translate-y-4 group-focus-within:scale-90 ${colorClass} ${isRaised && "-translate-y-4 scale-90"}`}>
                {placeholder}
            </span>
            <div className={`h-full w-full cursor-text ${colorClass} flex px-2`}>
                {before}
                <input className={`${colorClass} grow`} {...props} onChange={onChangeHandler} key={ctx} />
            </div>
        </label>
    );
}
