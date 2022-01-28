import { EventEmitter, Inject, Injectable, PLATFORM_ID, Injector } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders, HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';


import { Observable } from 'rxjs/Observable';
import { ErrorObservable } from 'rxjs/observable/ErrorObservable';
import { Subject } from 'rxjs/Subject';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { environment } from '../../environments/environment';


export interface TokenResponse {
    token: string;
    userName: string;
    chatToken: string;
}

export class AuthState {
    public isLogged: boolean;
    constructor(public userName: string) {
        if (userName) {
            this.isLogged = true;
        } else {
            this.isLogged = false;
        }
    }
    public static Empty() {
        const state = new AuthState(null);
        return state;
    }
}

@Injectable()
export class AuthService {
    authKey = 'auth';

    private authStateSubject: BehaviorSubject<AuthState>;

    get AuthState(): Observable<AuthState> {
        return this.authStateSubject.asObservable();
    }

    constructor(private http: HttpClient,
        @Inject(PLATFORM_ID) private platformId: any) {

        this.authStateSubject = new BehaviorSubject<AuthState>(AuthState.Empty());
    }

    login(username: string, password: string, twoFactorAuthCode: number): Observable<boolean> {
        const url = `${environment.apiBaseUrl}/api/token`;
        return this.getAuthFromServer(url, {
            userName: username,
            password: password,
            twoFactorAuthCode: twoFactorAuthCode
        });
    }

    getAuthFromServer(url: string, data: any): Observable<boolean> {
        return this.http.post<TokenResponse>(url, data)
            .map((res) => {
                const token = res && res.token;
                if (token) {
                    this.setAuth(res);
                    return true;
                }

                return Observable.throw('Unauthorized');
            })
            .catch(error => {
                const x = ErrorObservable.of<boolean | ErrorObservable>(false);
                return new Observable<any>(error);
            });
    }

    // performs the logout
    logout(): boolean {
        this.setAuth(null);
        return true;
    }

    // Persist auth into localStorage or removes it if a NULL argument is given
    setAuth(auth: TokenResponse | null): boolean {
        if (isPlatformBrowser(this.platformId)) {
            if (auth) {
                localStorage.setItem(this.authKey, JSON.stringify(auth));
                this.authStateSubject.next(new AuthState(auth.userName));
            } else {
                localStorage.removeItem(this.authKey);
                this.authStateSubject.next(AuthState.Empty());
            }
        }
        return true;
    }

    // Retrieves the auth JSON object (or NULL if none)
    getAuth(): (TokenResponse | null) {
        if (isPlatformBrowser(this.platformId)) {
            const i = localStorage.getItem(this.authKey);
            if (i) {
                const auth = JSON.parse(i);
                if (this.authStateSubject.value.userName !== auth.userName) {
                    this.authStateSubject.next(new AuthState(auth.userName));
                }

                return auth;
            }
        }

        return null;
    }

    // Returns TRUE if the user is logged in, FALSE otherwise.
    isLoggedIn(): boolean {
        if (isPlatformBrowser(this.platformId)) {
            return localStorage.getItem(this.authKey) != null;
        }
        return false;
    }
}

 // try to refresh token
    /*
    refreshToken(): Observable<boolean> {
        const url = `${environment.apiBaseUrl}/api/token`;
        const data = {
            client_id: this.clientId,
            // required when signing up with username/password
            grant_type: 'refresh_token',
            refresh_token: this.getAuth()!.refresh_token,
            // space-separated list of scopes for which the token is issued
            scope: 'offline_access profile email'
        };

        return this.getAuthFromServer(url, data);
    }
    */