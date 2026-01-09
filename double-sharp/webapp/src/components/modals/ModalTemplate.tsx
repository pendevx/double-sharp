type ModalTemplateProps = {
    children?: React.ReactNode;
};

export default function ModalTemplate({ children }: ModalTemplateProps) {
    return (
        <div className="m-auto h-[80vh] w-3/4 overflow-hidden rounded-lg border-[1px] border-solid border-[#111] bg-[#080808]">
            <div className="mb-4 ml-8 mr-12 mt-4">{children}</div>
        </div>
    );
}
