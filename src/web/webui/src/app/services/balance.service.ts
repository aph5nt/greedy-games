import { Injectable } from '@angular/core';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { ErrorObservable } from "rxjs/observable/ErrorObservable";
import { Network, Balance } from '../models/network';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { getHeaders, handleError } from './common';

@Injectable()
export class BalanceService {
     
    constructor(private http: Http, private authService: AuthService) { }

    getUserBalance(network: Network): Observable<Balance> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/balance/${network}`;
       
        return this.http.get(apiUrl, options)
            .map((res: Response) => {
                return res.json();
            })
            .catch((error: any) => handleError(error));
    }

    getBankrollBalance(network: Network): Observable<Balance> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/balance/bankroll/${network}`;
       
        return this.http.get(apiUrl, options)
            .map((res: Response) => {
                return res.json();
            })
            .catch((error: any) => handleError(error));
    }
}