import React from "react";
import { AccountsContext, UserInformation } from "../../../contexts/AccountsContext";
import CommonSettingsTab from "./CommonSettingsTab";
import { UserDetails, Unauthenticated } from "./Accounts";

export default function AccountTab() {
    const { userInformation } = React.useContext(AccountsContext);

    return <CommonSettingsTab title="Account">{userInformation == null ? <Unauthenticated /> : <UserDetails />}</CommonSettingsTab>;
}
