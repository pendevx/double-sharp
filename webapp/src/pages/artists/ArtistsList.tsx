import React from "react";
import { Scrollable } from "../../components";
import useFetch from "../../hooks/useFetch";
import UserIcon from "../../icons/UserIcon";
import { AuthContext, Role } from "../../contexts/AuthContext";
import { useNavigate } from "react-router";

function Card({ children, ...props }: { children: React.ReactNode } & React.HTMLAttributes<HTMLButtonElement>) {
    return (
        <button className="h-20 rounded-xl bg-gray-700 px-8 py-4" {...props}>
            {children}
        </button>
    );
}

function ArtistCard({ imageUrl, name }: { imageUrl?: string; name: string }) {
    return (
        <Card>
            <div className="flex h-full gap-4">
                {imageUrl ? (
                    <img src={imageUrl} alt={name} className="aspect-square h-full rounded-full" />
                ) : (
                    <i className="aspect-square h-full rounded-full p-2">
                        <UserIcon />
                    </i>
                )}
                <div>
                    <h3 className="mt-2 text-lg font-semibold">{name}</h3>
                    <p className="mt-1 text-sm text-gray-300">Details, such as Chinese Singer and Actor</p>
                    <p className="mt-1 text-sm text-gray-300">Number of songs: 42</p>
                </div>
            </div>
        </Card>
    );
}

function SuggestArtistCard() {
    const navigate = useNavigate();

    return (
        <Card onClick={() => navigate("request-artist")}>
            <div className="flex h-full w-full items-center gap-6">
                <i className="relative -top-2 text-6xl">+</i>
                <span className="text-xl">Suggest a new artist</span>
            </div>
        </Card>
    );
}

type Artist = {
    id: number;
    name: string;
    imageUrl?: string;
    dateOfBirth?: string;
};

export default function ArtistsList() {
    const { data: artists, refreshData } = useFetch<Artist[]>([], "/api/artists");
    const authContext = React.useContext(AuthContext);

    React.useEffect(() => {
        refreshData();
        authContext.checkRoleAsync(Role.User);
    }, []);

    return (
        <Scrollable>
            <ul className="flex flex-col gap-4">
                {authContext.hasRole(Role.User) && (
                    <li className="sticky top-0">
                        <SuggestArtistCard />
                    </li>
                )}

                {artists.map(artist => (
                    <li key={artist.id}>
                        <ArtistCard imageUrl={artist.imageUrl} name={artist.name} />
                    </li>
                ))}
            </ul>
        </Scrollable>
    );
}
