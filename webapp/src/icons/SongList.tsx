type SongListProps = {
    isOpen?: boolean;
    className?: string;
    onClick?: () => void;
};

export default function SongList({ isOpen, className, onClick }: SongListProps) {
    return (
        <svg
            version="1.1"
            id="_x32_"
            xmlns="http://www.w3.org/2000/svg"
            xmlnsXlink="http://www.w3.org/1999/xlink"
            className={`h-full w-full transition-colors duration-1000 ${isOpen ? "fill-[#ffc421]" : "fill-white"} ${className}`}
            onClick={onClick}
            viewBox="0 0 512 512"
            xmlSpace="preserve">
            <g>
                <path
                    className="st0"
                    d="M443.643,0v326.061c0,23.864-26.356-3.965-78.048,1.992c-66.323,8.226-113.17,60.232-112.999,109.071
		c0.17,48.874,54.068,81.851,120.338,73.616c78.156-3.75,128.035-54.562,127.855-103.445V0H443.643z"
                />
                <rect x="11.211" y="6.109" className="st0" width="347.942" height="59.254" />
                <rect x="11.211" y="132.826" className="st0" width="347.942" height="59.253" />
                <path
                    className="st0"
                    d="M11.211,386.257v59.246h187.8c-0.342-3.32-0.566-6.684-0.574-10.101c-0.054-17.019,3.57-33.607,10.19-49.144
		H11.211z"
                />
                <path
                    className="st0"
                    d="M342.809,290.698c5.589-0.637,10.998-0.951,16.345-1.05v-30.106H11.211v59.244h255.136
		C288.381,304.234,314.505,294.205,342.809,290.698z"
                />
            </g>
        </svg>
    );
}
