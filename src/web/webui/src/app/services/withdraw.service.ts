import { Injectable } from '@angular/core';
import { getHeaders, handleError } from './common';
import { environment } from '../../environments/environment';
import { Network } from '../models/network';
import { UserAccount } from '../models/userAccount';
import { Observable } from 'rxjs/Observable';
import { AuthService } from './auth.service';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { TranStatus } from './deposit.service';

export class UserWithdraw {
    public network: Network 
    public toAddress : string
    public amount: number;
    public createdAt: Date
    public updatedAt : Date
    public transactionId : string;
    public tranStatus: TranStatus;
}

export class WithdrawResult {
    public data: UserWithdraw[];
    public totalPages: number;
    public pageIndex: number;
    public hasNextPage: boolean;
    public hasPreviousPage: boolean;
  }

@Injectable()
export class WithdrawService {

    constructor(private http: Http, private authService: AuthService) { }


    getHistory(network: Network, page: number, pageSize: number): Observable<WithdrawResult> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/withdraw/history/${network}?page=${page}&pageSize=${pageSize}`;
    
        return this.http.get(apiUrl, options)
        .map((res: Response) => res.json())
        .catch((error: any) => handleError(error));
      }

      getBankrollHistory(network: Network, page: number, pageSize: number): Observable<WithdrawResult> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/withdraw/bankroll/history/${network}?page=${page}&pageSize=${pageSize}`;
    
        return this.http.get(apiUrl, options)
        .map((res: Response) => res.json())
        .catch((error: any) => handleError(error));
      }
    
}