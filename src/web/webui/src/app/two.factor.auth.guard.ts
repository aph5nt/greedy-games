import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';
import { TwoFactorAuthService } from './services/two.factor.auth.service';
import { Network } from './models/network';

@Injectable()
export class TwoFactorAuthGuard implements CanActivate {

  constructor(public service: TwoFactorAuthService, public router: Router) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot, ): Observable<boolean> | Promise<boolean> | boolean {
    if (next.params.network === <string>Network.Free) {
      return true;
    }
    return this.service.get().map(result => {
      if (!result.enabled) { // 2fa is not enabled for waves network
        this.router.navigate(['/2fasetup']);
      }
      return result.enabled;
    });
  }
}