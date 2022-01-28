import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class FieldSizeGuard implements CanActivate {

    constructor(private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
        const fieldSize = route.url[2].path.toLowerCase();

        // use defaults here
        if (fieldSize === '3x2' || fieldSize === '6x3' || fieldSize === '9x4') {
            return Observable.of(true);
        }

        return Observable.of(false);
    }
}