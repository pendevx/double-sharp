import React from "react";
import { KeyedForm } from "../../..";
import { LoginForm, RegistrationForm } from ".";
import { AccountsContext } from "../../../../contexts/AccountsContext";

export default function Unauthenticated() {
    const [action, setAction] = React.useState<"login" | "register">("register");
    const [formValid, setFormValid] = React.useState<boolean>(false);
    const { login, register } = React.useContext(AccountsContext);

    const switchAction = () => {
        setAction(action === "login" ? "register" : "login");
    };

    const actionName = action.charAt(0).toUpperCase() + action.slice(1);

    const onSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const form = e.currentTarget as HTMLFormElement;
        const formData = new FormData(form);

        const username = formData.get("username") as string;
        const password = formData.get("password") as string;
        const displayName = formData.get("displayName") as string;
        const email = formData.get("email") as string;

        if (action === "login") {
            await login(username, password);
        } else {
            await register(username, password, { displayName: displayName });
        }
    };

    return (
        <div>
            <h4>Log in or register an account below to unlock more functionality!</h4>

            <div className="relative mb-8 mt-4 w-full">
                <h5 className="mb-2 text-center text-base font-bold">{actionName}</h5>
                <button className="absolute bottom-0 right-0 w-fit cursor-pointer text-sm text-white" onClick={switchAction}>
                    Switch to {action === "login" ? "Register" : "Login"}
                </button>
            </div>

            <KeyedForm className="flex flex-col items-start gap-4" onKeyDown={e => e.stopPropagation()} onSubmit={onSubmit}>
                {action === "login" ? <LoginForm reportValidity={setFormValid} /> : <RegistrationForm reportValidity={setFormValid} />}

                <div className="grid w-full grid-cols-2 gap-4">
                    <button type="reset" className="w-full border-[1px] border-solid border-white bg-[#333] p-2 text-center text-white">
                        Reset
                    </button>
                    <button type="submit" className="w-full border-[1px] border-solid border-white bg-[#004317] p-2 text-center text-white disabled:bg-[#355c42]" disabled={!formValid}>
                        {actionName}
                    </button>
                </div>
            </KeyedForm>
        </div>
    );
}
