type CommonSettingsTabProps = {
    title: string;
    children?: React.ReactNode;
};

export default function CommonSettingsTab({ title, children }: CommonSettingsTabProps) {
    return (
        <div className="mb-8">
            <h3 className="mb-2 text-xl font-semibold">{title}</h3>
            {children}
        </div>
    );
}
