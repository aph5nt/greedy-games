import { Injectable } from '@angular/core';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { ErrorObservable } from "rxjs/observable/ErrorObservable";
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { getHeaders, handleError } from './common';

declare var jquery: any;
declare var $: any;

export class TwoFactorAuth {
    public enabled: boolean;
}

export class TwoFactorAuthSetup {
    public manualSetupKey: string;
    public qrCodeImage: string;
}

export class TwoFactorAuthCode {
    constructor(public value: number){

    }
}

@Injectable()
export class TwoFactorAuthService {

    constructor(private http: Http, private authService: AuthService) {

    }

    get(): Observable<TwoFactorAuth> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/2fa`;

        return this.http.get(apiUrl, options)
            .map((res: Response) => res.json())
            .catch((error: any) => handleError(error));
    }

    generate(): Observable<TwoFactorAuthSetup> {
        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/2fa/generate`;

        return this.http.get(apiUrl, options)
            .map((res: Response) => res.json())
            .catch((error: any) => handleError(error));
    }

    enable(body: TwoFactorAuthCode): Observable<Response> {

        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/2fa/enable`;
        return this.http.put(apiUrl, body, options)
            .map((res: Response) => res)
            .catch((error: any) => handleError(error));
    }

    disable(body: TwoFactorAuthCode): Observable<Response> {

        const headers = getHeaders(this.authService.getAuth().token);
        const options = new RequestOptions({ headers: headers });
        const apiUrl = `${environment.apiBaseUrl}/api/2fa/disable`;
        return this.http.put(apiUrl, body, options)
            .map((res: Response) => res)
            .catch((error: any) => handleError(error));
    }
}