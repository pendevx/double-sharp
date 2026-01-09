import { MusicLoopSvg, MusicShuffleSvg, MusicIconSvg } from ".";
import { PlayBehaviour } from "../types/playBehaviour";

type PlayBehaviourIconProps = {
    playBehaviour: PlayBehaviour;
    className?: string;
    fill?: string;
};

export default function PlayBehaviourIcon({ playBehaviour, className, fill }: PlayBehaviourIconProps) {
    let icon;

    switch (playBehaviour) {
        case "loop":
            icon = <MusicLoopSvg className={className} fill={fill} />;
            break;

        case "shuffle":
            icon = <MusicShuffleSvg className={className} fill={fill} />;
            break;

        default:
            icon = <MusicIconSvg className={className} fill={fill} />;
            break;
    }

    return <div className="h-full">{icon}</div>;
}
