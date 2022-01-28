export const satoshi = 100000000;
export const cent = 1000000;

export enum Network {
    Free = "free",
    Waves = "waves",
    WavesTest = "wavestest"
}
 
export class FieldSize {

    static maxBet: { [key: string]: number } = {
        [Network.Free]: 10 * satoshi,
        [Network.Waves]: 10 * satoshi,
        [Network.WavesTest]: 10 * satoshi,
    };

    static minBet: { [key: string]: number }= {
        [Network.Free]: 0.01 * satoshi,
        [Network.Waves]: 0.01 * satoshi,
        [Network.WavesTest]: 0.01 * satoshi,
    };

    static defaultBet: { [key: string]: number } = {
        [Network.Free]: 0.1 * satoshi,
        [Network.Waves]: 0.1 * satoshi,
        [Network.WavesTest]: 0.1 * satoshi,
    };

    constructor(public size: string, public display: string, public columns: number, public rows: number, public multipiers: number[]) {
        
    }
}

export class Field {
    public canStepOn = false;
    public steppedOn = false;

    constructor(
        public rowIndex: number,
        public columnIndex: number,
        public state: FieldState = FieldState.Unknown) {
    }
}

export enum FieldState {
    Unknown = "unknown",
    Safe = "safe",
    Mined = "mined"
}

export enum Status {
    None = "none",
    Alive = "alive",
    Dead = "dead",
    Escaped = "escaped",
    TakeAway  ="takeAway"
}

export class Position {
    x: number;
    y: number;
}

export class UserState {
    isEmpty: boolean = false;
    board: Field[];
    moves: Position[];
    position: Position;
    status: Status;
    bet: number;
    win: number;
    loss: number;
    gameId: string;
    fieldSize: FieldSize;
    network: Network;

    static canStart(userState: UserState): boolean {
        return userState.status !== Status.Alive;
    }

    static flat(input: any): Field[] {
        const emptyBoard = new Array<Field>();

        // first index 4 rows - y
        // second index 9 columns - x

        const rows = input.length;
        const columns = input[0].length;

        for (let y = 0; y < rows; y++) {
            for (let x = 0; x < columns; x++) {
                emptyBoard.push(new Field(y, x, input[y][x].state));
            }
        }

        return emptyBoard;
    }

    static empty(network: Network, fieldSize: FieldSize): UserState {

        const emptyBoard = new Array<Field>();
        for (let r = 0; r < fieldSize.rows; r++) {
            for (let c = 0; c < fieldSize.columns; c++) {
                emptyBoard.push(new Field(r, c, FieldState.Unknown));
            }
        }

        const userState = new UserState();
        userState.board = emptyBoard;
        userState.moves = [];
        userState.position = { x: -1, y: 0 };
        userState.status = Status.None;
        userState.bet = FieldSize.defaultBet[network];
        userState.win = 0;
        userState.loss = 0;
        userState.gameId = '';
        userState.fieldSize = fieldSize;
        userState.network = network;

        return userState;

    }
}

export class ClientSetting {
    seed: string;
    bet: number;
    x: number;
    y: number;
}

export class Guid {

    public static newGuid(): string {
        return this.generateGuid();
    }

    private static generateGuid(): string {
        return this.generatePart() + this.generatePart() + '-' + this.generatePart() + '-' + this.generatePart() + '-' +
            this.generatePart() + '-' + this.generatePart() + this.generatePart() + this.generatePart();
    }

    private static generatePart(): string {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
}

export class Settings {
    bet: number;
    min: number;
    max: number;
    fieldSize: FieldSize;
    status: Status;
    network: Network;

    static createFrom(userState: UserState): Settings {
        const settings = new Settings();
        settings.bet = userState.bet;
        settings.min = FieldSize.minBet[userState.network];
        settings.max = FieldSize.maxBet[userState.network];
        settings.status = userState.status;
        settings.fieldSize = userState.fieldSize;
        settings.network = userState.network;
        return settings;
    }
}

export class UserMessage {
    channelType: string;
    channelUrl: string;
    createdAt: number;
    customType: string;
    data: string;
    message: string;
    messageId: number;
    messageType: string;
    reqId: string;
}

export class UserToken {
    userName: string;
    token: string;
}

export const defaultNetwork = Network.Free;
export const networks = [defaultNetwork, Network.Waves, Network.WavesTest];

const multipiers3X2 = [1.92, 3.69, 7.08];
const multipiers6X3 = [1.44, 2.07, 2.99, 4.3, 6.19, 8.92];
const multipiers9X4 = [1.28, 1.64, 2.1, 2.68, 3.44, 4.4, 5.63, 7.21, 9.22];

export const defaultFieldSize = new FieldSize('6x3', '6 x 3', 6, 3, multipiers6X3);
 
export const fieldSizes = [
    new FieldSize('3x2', '3 x 2', 3, 2, multipiers3X2),
    defaultFieldSize,
    new FieldSize('9x4', '9 x 4', 9, 4, multipiers9X4)];