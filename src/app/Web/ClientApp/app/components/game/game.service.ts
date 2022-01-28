import { Injectable } from '@angular/core';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import * as model from './game.model';
import { ErrorObservable } from "rxjs/observable/ErrorObservable";
import { environment } from '../environments/environment';
declare var jquery: any;
declare var $: any;

////https://www.codeproject.com/Articles/1172349/SPA-using-ASP-Net-Core-plus-Angular-part

@Injectable()
export class GameService {

    public errorMessageSubject = new BehaviorSubject("");
    public errorMessage = this.errorMessageSubject.asObservable().distinctUntilChanged();

    constructor(private http: Http) {

    }

    get(network: model.Network): Observable<model.UserState> {
        const headers = this.getHeaders(new model.UserToken());
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/game/minefield/${network}`;
        
        return this.http.get(apiUrl, options)
            .map((res: Response) => res.json())
            .catch((error: any) => this.handleError(error));
    }

    start(network: model.Network, clientSetting: model.ClientSetting): Observable<model.UserState> {

        const headers = this.getHeaders(new model.UserToken());
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/game/minefield/${network}`;
        const body = clientSetting;
        return this.http.post(apiUrl, body, options)
            .map((res: Response) => res.json())
            .catch((error: any) => this.handleError(error));
    }

    move(network: model.Network, gameId: string, position: model.Position): Observable<model.UserState> {
        const headers = this.getHeaders(new model.UserToken());
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/game/minefield/${network}/move/${gameId}`;
        const body = position;

        return this.http.put(apiUrl, body, options)
            .map((res: Response) => res.json())
            .catch((error: any) => this.handleError(error));
    }

    takeAway(network: model.Network, gameId: string): Observable<model.UserState> {
        const headers = this.getHeaders(new model.UserToken());
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/game/minefield/${network}/takeaway/${gameId}`;

        return this.http.put(apiUrl, {}, options)
            .map((res: Response) => res.json())
            .catch((error: any) => this.handleError(error));
    }

    getHeaders(userToken: model.UserToken): Headers {
        let token: any = $("input[name=__RequestVerificationToken]").val();
        const headers = new Headers();
        headers.append('Accept', 'application/json');
        headers.append('content-type', 'application/json; charset=utf-8');
        headers.append('RequestVerificationToken', token);
        //headers.append('Authorization', `Bearer ${userToken.token}`);
        return headers;
    }
     
    handleError(error: any): ErrorObservable {
        const message = error._body || 'Server error';
        this.errorMessageSubject.next(message);
        return Observable.throw(message);
    }
}