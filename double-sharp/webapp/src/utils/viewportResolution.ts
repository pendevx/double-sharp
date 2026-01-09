export enum ViewportResolution {
    Mobile,
    Tablet,
    Laptop,
    Desktop,
}

const isMobile = () => window.matchMedia("(max-width: 640px)").matches;
const isTablet = () => window.matchMedia("(min-width: 641px) and (max-width: 768px)").matches;
const isLaptop = () => window.matchMedia("(min-width: 769px) and (max-width: 1280px)").matches;

export default (): ViewportResolution => {
    if (isMobile()) return ViewportResolution.Mobile;
    if (isTablet()) return ViewportResolution.Tablet;
    if (isLaptop()) return ViewportResolution.Laptop;
    return ViewportResolution.Desktop;
};
