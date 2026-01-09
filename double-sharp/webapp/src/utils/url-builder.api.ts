export function downloadSong(songId: number) {
    return `/api/music/download/${encodeURIComponent(songId)}`;
}

export function listSongs() {
    return `/api/music/list`;
}

export function downloadLyrics(songId: number) {
    return `/api/lyrics/download/${encodeURIComponent(songId)}`;
}

export function registerAccount() {
    return `/api/accounts/register`;
}

export function loginAccount() {
    return `/api/accounts/login`;
}

export function logoutAccount() {
    return `/api/accounts/logout`;
}

export function userDetails() {
    return `/api/accounts/user`;
}
