export default function ChangeTrack({ className, onClick }) {
    return (
        <svg viewBox="0 0 36 36" xmlns="http://www.w3.org/2000/svg" xmlnsXlink="http://www.w3.org/1999/xlink" role="img" className={`${className} cursor-pointer`} onClick={onClick}>
            <path className="fill-transparent" d="M36 32a4 4 0 0 1-4 4H4a4 4 0 0 1-4-4V4a4 4 0 0 1 4-4h28a4 4 0 0 1 4 4v28z"></path>
            <path className="fill-white" d="M27 18L15 7v9.166L5 7v22l10-9.167V29zm0-11h4v22h-4z"></path>
        </svg>
    );
}
