import { Network } from "./network";

export class UserAccount {
    public isActive: boolean;
    public depositAddress: string;
    public updatedAt: Date;
    public network: Network;
    public userName: string;
    public id: number;
  }


  export class GameAccount {
    public treshold: number;
    public depositAddress: string;
    public updatedAt: Date;
    public network: Network;
    public userName: string;
    public id: number;
  }
