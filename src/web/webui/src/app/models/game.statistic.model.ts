import { Network } from "./network";

export class GameStatistic {
    public id: string;
    public gameId: string;
    public size: string;
    public network: Network;
    public userName: string;
    public type: string;
    public createdAt: Date;
    public turn: number;
    public bet: string;
    public win: string;
    public loss: string;
}
 
export class GameStatisticResult {
    public data: GameStatistic[];
    public totalPages: number;
    public pageIndex: number;
    public hasNextPage: boolean;
    public hasPreviousPage: boolean;
}


export class GameDailyStatistic {
    public network: Network;
    public userName: string;
    public type: string;
    public profit: number;
    public date: Date;
}

export class GameDailyStatisticResult {
    public data: GameDailyStatistic[];
    public totalPages: number;
    public pageIndex: number;
    public hasNextPage: boolean;
    public hasPreviousPage: boolean;
}