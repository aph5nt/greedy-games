import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Network, NetworksStr } from './models/network';

@Injectable()
export class NetworkGuard implements CanActivate {
  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    if (NetworksStr.indexOf(next.params.network) >= 0) {
      return true;
    }
    return false;
  }
}


