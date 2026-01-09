export const twoSpacePadding = (val: number) => Math.floor(val).toString().padStart(2, "0");
export const formatTime = (seconds: number) => twoSpacePadding(seconds / 60) + ":" + twoSpacePadding(seconds % 60);
