import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ErrorObservable } from 'rxjs/observable/ErrorObservable';

export function getHeaders(token): Headers {
   
    const headers = new Headers();
    headers.append('Accept', 'application/json');
    headers.append('Content-Type', 'application/json; charset=utf-8');
    if(token) headers.append('Authorization', `Bearer ${token}`);
    return headers;
}
 
export function handleError(error: any): ErrorObservable {
    const message = error._body || 'Server error';
    return Observable.throw(message);
}