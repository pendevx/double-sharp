export default function debounce<R, Args extends any[]>(func: (...args: Args) => R, delay: number): (...args: Args) => void {
    let timeout: number;
    return function (this: any, ...args: Args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), delay);
    };
}
