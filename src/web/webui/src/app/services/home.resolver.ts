import { Injectable, } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/combineLatest';

import { UiService } from './ui.service';
import { Network } from '../models/network';


@Injectable()
export class HomeResolver implements Resolve<any> {
    constructor(
        private uiService: UiService,
        private router: Router) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {

        const network = <Network>route.params['network'];
        this.uiService.Page = 'home';
        this.uiService.Network = network;

        return Observable.combineLatest(Observable.of(network),
            // tslint:disable-next-line:no-shadowed-variable
            (network, model) => {
                return {
                    network
                };
            });
    }
}
