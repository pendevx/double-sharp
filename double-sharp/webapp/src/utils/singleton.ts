type TConstructor<T> = new (...args: any[]) => T;

export default function singleton<T>(type: TConstructor<T>): TConstructor<T> {
    let instance: T | null = null;

    const proxy = new Proxy(type.constructor, {
        construct(_, args) {
            if (instance == null) {
                instance = new type(...args);
            }

            return instance as object;
        },
    });

    type.constructor = proxy;

    return proxy as TConstructor<T>;
}
