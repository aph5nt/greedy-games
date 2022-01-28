import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import { Http, Headers, RequestOptions, Response } from '@angular/http';

import { environment } from '../../environments/environment';
import { UserAccount, GameAccount } from '../models/userAccount';
import { AuthService } from './auth.service';
import { Network } from '../models/network';
import { getHeaders, handleError } from './common';
 

export class CreateAccount {
  userName: string;
  password: string;
}

export class UserCount {
  count: number;
}

export class RegisterModel {
  constructor(public token: string) {
  }
}

export class GeneratedPassword {
  password: string;
}

@Injectable()
export class AccountService {

  constructor(private http: Http, private authService: AuthService) { }

  createAccount(recaptchaToken: string): Observable<CreateAccount> {
    const headers = getHeaders(null);
    const options = new RequestOptions({ headers: headers });
    const body = new RegisterModel(recaptchaToken);
    const apiUrl = `${environment.apiBaseUrl}/api/account`;

    return this.http.post(apiUrl, body, options)
      .map((res: Response) => res.json())
      .catch((error: any) => handleError(error));
  }

  getUserCount(): Observable<UserCount> {
    const headers = getHeaders(null);
    const options = new RequestOptions({ headers: headers });
    const apiUrl = `${environment.apiBaseUrl}/api/account/count`;

    return this.http.get(apiUrl, options)
    .map((res: Response) => res.json())
    .catch((error: any) => handleError(error));
  }

  generatePassword(): Observable<GeneratedPassword> {
    const headers = getHeaders(null);
    const options = new RequestOptions({ headers: headers });
    const apiUrl = `${environment.apiBaseUrl}/api/account/generatepassword`;

    return this.http.get(apiUrl, options)
    .map((res: Response) => res.json())
    .catch((error: any) => handleError(error));
  }

  getAccount(network: Network): Observable<UserAccount> {
    const headers = getHeaders(this.authService.getAuth().token);
    const options = new RequestOptions({ headers: headers });
    const apiUrl = `${environment.apiBaseUrl}/api/account/${network}`;

    return this.http.get(apiUrl, options)
      .map((res: Response) => res.json())
      .catch((error: any) => handleError(error));
  }

  getBankrollAccount(network: Network): Observable<GameAccount> {
    const headers = getHeaders(this.authService.getAuth().token);
    const options = new RequestOptions({ headers: headers });
    const apiUrl = `${environment.apiBaseUrl}/api/account/bankroll/${network}`;

    return this.http.get(apiUrl, options)
      .map((res: Response) => res.json())
      .catch((error: any) => handleError(error));
  }
  

  activateAccount(recaptchaToken: string, network: Network): Observable<Response> {
    const headers = getHeaders(this.authService.getAuth().token);
    const options = new RequestOptions({ headers: headers });
    const body = {};
    const apiUrl = `${environment.apiBaseUrl}/api/deposit/activate/${network}`;

    return this.http.put(apiUrl, body, options)
    .map((res: Response) => res.json())
    .catch((error: any) => handleError(error));
  }
 

}
