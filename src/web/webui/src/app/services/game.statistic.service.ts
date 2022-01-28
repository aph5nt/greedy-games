import { Injectable } from '@angular/core';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { ErrorObservable } from "rxjs/observable/ErrorObservable";
import { Network } from '../models/network';
import { UserState, ClientSetting, Position } from '../models/game.model';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { GameStatistic, GameStatisticResult, GameDailyStatisticResult } from '../models/game.statistic.model'
import { getHeaders, handleError } from './common';

declare var jquery: any;
declare var $: any;

@Injectable()
export class GameStatisticService {

    constructor(private http: Http, private authService: AuthService) { }

    get(network: Network, showAll: boolean, page: number, pageSize: number): Observable<GameStatisticResult> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/statistics/${network}?showAll=${showAll}&page=${page}&pageSize=${pageSize}`;

        return this.http.get(apiUrl, options)
            .map((res: Response) => {
                return res.json();
            })
            .catch((error: any) => handleError(error));
    }

    getDaily(network: Network, showAll: boolean, page: number, pageSize: number): Observable<GameDailyStatisticResult> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/statistics/${network}/daily/?showAll=${showAll}&page=${page}&pageSize=${pageSize}`;

        return this.http.get(apiUrl, options)
            .map((res: Response) => {
                return res.json();
            })
            .catch((error: any) => handleError(error));
    }

    getSingle(network: Network, userName: string, gameId): Observable<GameStatistic> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/statistics/${network}/${userName}/${gameId}`;

        return this.http.get(apiUrl, options)
            .map((res: Response) => {
                return res.json();
            })
            .catch((error: any) => handleError(error));
    }
}