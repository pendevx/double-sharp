import React from "react";
import localStorageRepository, { Key } from "../repositories/LocalStorageRepository";

/**
 *
 * @param key The key to read from the local storage
 * @param converter A runtime type converter
 * @returns A tuple containing the value and a setter function for the state
 */
export function useStoreState<T>(key: Key, converter: (value: any) => T = (value: any) => value): [T, React.Dispatch<React.SetStateAction<T>>] {
    const [data, setData] = React.useState<T>(converter(localStorageRepository.get(key)));
    localStorageRepository.set(key, data);
    return [data, setData];
}

export function useStoreRef<T>(key: Key, converter: (value: any) => T): React.MutableRefObject<T> {
    const ref = React.useRef<T>(converter(localStorageRepository.get(key)));

    const proxy = new Proxy(ref, {
        set(obj, prop, value) {
            localStorageRepository.set(key, value);
            return Reflect.set(obj, prop, value);
        },
    });

    return proxy;
}
