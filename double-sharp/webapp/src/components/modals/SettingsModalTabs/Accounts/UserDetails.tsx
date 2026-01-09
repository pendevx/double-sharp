import { AccountsContext } from "../../../../contexts/AccountsContext";
import React from "react";

export default function UserDetails() {
    const { userInformation, logout } = React.useContext(AccountsContext);

    return (
        <div>
            <div className="relative">
                <h4>Currently signed in as: {userInformation!.displayName}</h4>
                <button type="button" className="absolute bottom-0 right-0 w-fit cursor-pointer text-white" onClick={logout}>
                    Log out
                </button>
            </div>
        </div>
    );
}
