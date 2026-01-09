type CircularSvgProps = {
    width?: number;
    height?: number;
    children?: React.ReactNode;
    className?: string;
    onClick?: () => void;
};

export default function CircularSvg({ width, height, children, className, onClick }: CircularSvgProps) {
    return (
        <div className={`overflow-hidden rounded-[50%] fill-[#000000] p-3 ${className ? className : ""}`} onClick={onClick} style={{ width, height }}>
            {children}
        </div>
    );
}
