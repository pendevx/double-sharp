import React from "react";
import { Scrollable } from "../";
import { AccountTab, AppearanceTab, GeneralTab } from "./SettingsModalTabs";
import ModalTemplate from "./ModalTemplate";

enum SettingsTabs {
    General,
    Appearance,
    Account,
}

export default function SettingsModal() {
    const [currentTab, setCurrentTab] = React.useState<SettingsTabs>(SettingsTabs.General);

    return (
        <ModalTemplate>
            <div className="flex">
                <div className="hidden grow basis-1 desktop:block">
                    <div className="relative">
                        <SelectableTab selected={currentTab === SettingsTabs.General} onClick={() => setCurrentTab(SettingsTabs.General)}>
                            <span className="relative z-10">General</span>
                        </SelectableTab>
                        <SelectableTab selected={currentTab === SettingsTabs.Appearance} onClick={() => setCurrentTab(SettingsTabs.Appearance)}>
                            <span className="relative z-10">Appearance</span>
                        </SelectableTab>
                        <SelectableTab selected={currentTab === SettingsTabs.Account} onClick={() => setCurrentTab(SettingsTabs.Account)}>
                            <span className="relative z-10">Account</span>
                        </SelectableTab>

                        <i className="absolute bottom-0 left-0 top-0 w-[1px] bg-[#333]" />
                    </div>
                </div>

                <Scrollable showScroller={false} className="grow-[3] basis-1 pl-12">
                    <GeneralTab />
                    <AppearanceTab />
                    <AccountTab />
                </Scrollable>
            </div>
        </ModalTemplate>
    );
}

type SelectableTabProps = {
    children: React.ReactNode | string;
    selected: boolean;
    onClick: () => void;
};

function SelectableTab({ children, selected, onClick }: SelectableTabProps) {
    return (
        <button
            className={`relative w-full cursor-pointer p-2 pl-6 text-left transition-colors duration-150 before:absolute before:left-0 before:top-0 before:h-full before:bg-[#222] before:transition-all before:duration-300 before:content-[''] hover:text-[#ffc421] before:hover:w-full ${selected ? "text-[#ffc421] before:w-full" : "text-white before:w-0"}`}
            onClick={onClick}>
            {children}
        </button>
    );
}
