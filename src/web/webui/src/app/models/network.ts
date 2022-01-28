export enum Network {
    Free = 'free',
    Waves = 'waves',
}

export const DefaultNetwork = Network.Free;
export const Networks = [DefaultNetwork, Network.Waves];
export const NetworksStr = [<string>DefaultNetwork, <string>Network.Waves];


export class Balance {
    network: Network;
    amount: number;
}