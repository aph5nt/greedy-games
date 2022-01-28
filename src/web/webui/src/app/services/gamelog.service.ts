import { Injectable } from '@angular/core';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { ErrorObservable } from "rxjs/observable/ErrorObservable";
import { Network } from '../models/network';
import { UserState, ClientSetting, Position, GameLog } from '../models/game.model';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { getHeaders, handleError } from './common';

declare var jquery: any;
declare var $: any;

@Injectable()
export class GameLogService {
    constructor(private http: Http, private authService: AuthService) { }

    get(network: Network, userName: string, gameId: string): Observable<GameLog> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/gamelogs/${network}/${userName}/${gameId}`;
     
        return this.http.get(apiUrl, options)
            .map((res: Response) => res.json())
            .catch((error: any) => handleError(error));
    }
}