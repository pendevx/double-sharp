import ValidatableForm, { ReportValidity } from "../../../ValidatableForm";

export default function LoginForm({ reportValidity }: { reportValidity: ReportValidity }) {
    return (
        <ValidatableForm
            className="grid w-full grid-cols-2 gap-4"
            fields={[
                { placeholder: "Username", type: "text", required: true, name: "username" },
                { placeholder: "Password", type: "password", required: true, name: "password" },
            ]}
            reportValidity={reportValidity}
        />
    );
}
