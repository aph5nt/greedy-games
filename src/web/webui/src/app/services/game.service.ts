import { Injectable } from '@angular/core';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { ErrorObservable } from "rxjs/observable/ErrorObservable";
import { Network } from '../models/network';
import { UserState, ClientSetting, Position } from '../models/game.model';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { getHeaders, handleError } from './common';

declare var jquery: any;
declare var $: any;

@Injectable()
export class GameService {

    public errorMessageSubject = new BehaviorSubject("");
    public errorMessage = this.errorMessageSubject.asObservable().distinctUntilChanged();

    constructor(private http: Http, private authService: AuthService) {
    }

    get(network: Network): Observable<UserState> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/game/${network}`;

        return this.http.get(apiUrl, options)
            .map((res: Response) => res.json())
            .catch((error: any) => handleError(error));
    }
 
    start(network: Network, clientSetting: ClientSetting): Observable<UserState> {

        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/game/${network}`;
        const body = clientSetting;
        return this.http.post(apiUrl, body, options)
            .map((res: Response) => res.json())
            .catch((error: any) => handleError(error));
    }

    move(network: Network, gameId: string, position: Position): Observable<UserState> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/game/${network}/move/${gameId}`;
        const body = position;

        return this.http.put(apiUrl, body, options)
            .map((res: Response) => res.json())
            .catch((error: any) => handleError(error));
    }

    takeAway(network: Network, gameId: string): Observable<UserState> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/game/${network}/takeaway/${gameId}`;

        return this.http.put(apiUrl, {}, options)
            .map((res: Response) => res.json())
            .catch((error: any) => handleError(error));
    }
}