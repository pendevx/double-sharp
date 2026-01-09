import { PlayBehaviour } from "../types/playBehaviour";

export const playBehaviours = ["loop", "shuffle", null];

export function nextPlayBehaviour(currentPlayBehaviour: PlayBehaviour): PlayBehaviour {
    let nextBehaviour = playBehaviours[(playBehaviours.indexOf(currentPlayBehaviour) + 1) % playBehaviours.length];
    return nextBehaviour as PlayBehaviour;
}

export function lastPlayBehaviour(currentPlayBehaviour: PlayBehaviour): PlayBehaviour {
    let lastBehaviour = playBehaviours[(playBehaviours.indexOf(currentPlayBehaviour) - 1) % playBehaviours.length];
    return lastBehaviour as PlayBehaviour;
}
