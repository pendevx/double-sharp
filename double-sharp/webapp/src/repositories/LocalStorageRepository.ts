import singleton from "../utils/singleton";

class LocalStorageRepository {
    private cache = new Map<Key, any>();
    private deleteSym = Symbol("delete");

    get(key: Key) {
        const value = localStorage.getItem(key);
        this.cache.set(key, value);
        return value;
    }

    set(key: Key, value: any) {
        if (!this.cache.has(key) || this.cache.get(key) !== value) {
            this.cache.set(key, value);
        }
    }

    unset(key: Key) {
        this.cache.set(key, this.deleteSym);
    }

    commit() {
        for (const [k, v] of this.cache.entries()) {
            if (v === this.deleteSym) {
                localStorage.removeItem(k);
            } else {
                localStorage.setItem(k, v);
            }
        }
    }
}

export type Key = "playBehaviour" | "seed" | "lastSongId";

const repository = new (singleton(LocalStorageRepository))();

window.addEventListener("beforeunload", () => repository.commit());

export default repository;
