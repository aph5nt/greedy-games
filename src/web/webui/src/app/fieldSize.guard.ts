import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { FieldSizes } from './models/game.model'

@Injectable()
export class FieldSizeGuard implements CanActivate {
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
        if (FieldSizes.find(q => q.size === route.params.size)) {
            return true;
        }
        return false;
    }
}