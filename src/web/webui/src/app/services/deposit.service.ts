import { Injectable } from '@angular/core';
import { getHeaders, handleError } from './common';
import { environment } from '../../environments/environment';
import { Network } from '../models/network';
import { UserAccount } from '../models/userAccount';
import { Observable } from 'rxjs/Observable';
import { AuthService } from './auth.service';
import { Http, Headers, RequestOptions, Response } from '@angular/http';



export class Deposit {
  public isGameAccount: boolean;
  public amount: number;
  public transStatus: TranStatus;
  public transactionSignature: string;
  public TransactionHeight: string;
  public updatedAt: Date;
  public createdAt: Date;
  public network: Network;
  public userName: string;
  public id: number;
}

export class DepositResult {
  public data: Deposit[];
  public totalPages: number;
  public pageIndex: number;
  public hasNextPage: boolean;
  public hasPreviousPage: boolean;
}

export enum TranStatus {
  Pending = 0,
  Confirmed = 1,
  Failed = 2
}

export class DepositViewModel {
  public userAccount: UserAccount;
  public deposits: Deposit[];
}

@Injectable()
export class DepositService {

  constructor(private http: Http, private authService: AuthService) { }


  
  getHistory(network: Network, page: number, pageSize: number): Observable<DepositResult> {
    const headers = getHeaders(this.authService.getAuth().token);
    const options = new RequestOptions({ headers: headers });
    const apiUrl = `${environment.apiBaseUrl}/api/deposit/history/${network}?page=${page}&pageSize=${pageSize}`;

    return this.http.get(apiUrl, options)
    .map((res: Response) => res.json())
    .catch((error: any) => handleError(error));
  }

  getBankrollHistory(network: Network, page: number, pageSize: number): Observable<DepositResult> {
    const headers = getHeaders(this.authService.getAuth().token);
    const options = new RequestOptions({ headers: headers });
    const apiUrl = `${environment.apiBaseUrl}/api/deposit/bankroll/history/${network}?page=${page}&pageSize=${pageSize}`;

    return this.http.get(apiUrl, options)
    .map((res: Response) => res.json())
    .catch((error: any) => handleError(error));
  }

  
}
