/**
 * Creates a pseudo-rng, based on the [Linear Congruential Generator](https://en.wikipedia.org/wiki/Linear_congruential_generator).
 * The default value for the parameters A, C, and MOD use the same values as `glibc`.
 * @param {number} seed The seed to use for the pseudo-rng.
 * @param {number} A  `0 < A < M`
 * @param {number} C  `0 <= C < M`
 * @param {number} M  `0 < M`
 * @yields {number} A pseudo-random number in the range `[0, MOD)`
 */
export default function* createPRNG(seed: number = 0, A: number = 1103515245, C: number = 12345, MOD = 2 ** 31): Generator<number> {
    while (true) {
        seed = (A * seed + C) % MOD;
        yield seed;
    }
}
