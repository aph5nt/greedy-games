import { Injectable } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
 
@Injectable()
export class NetworkGuard implements CanActivate {

    constructor(private router: Router, private location : Location) { }

    canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
        const network = route.url[0].path.toLowerCase();

        // use defaults here
        if (network === 'free' || network === 'waves' || network === 'wavestest') {
            return Observable.of(true);
        }

        //this.location.replace('/');
        this.router.navigate(['/free']);

        return Observable.of(false);
    }
}

