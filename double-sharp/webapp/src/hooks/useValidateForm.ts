import React from "react";

export default function useValidateForm(length: number, key: any) {
    const [valid, setValid] = React.useState<boolean[]>(Array(length).fill(false));
    const [formValid, setFormValid] = React.useState(false);
    const [currentKey, setCurrentKey] = React.useState(key);

    if (currentKey !== key) {
        setValid(Array(length).fill(false));
        setFormValid(false);
        setCurrentKey(key);
    }

    const reportValidity = (id: number, v: boolean) => {
        const newValid = [...valid];
        newValid[id] = v;
        setValid(newValid);

        setFormValid(newValid.every(x => x));
    };

    return { formValid, reportValidity };
}
