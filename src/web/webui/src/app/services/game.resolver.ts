import { Injectable, } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot, ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/combineLatest';

import { UiService } from './ui.service';
import { Network } from '../models/network';
import { GameService } from './game.service';
import { DefaultFieldSize, FieldSizes, UserState, Status } from '../models/game.model';
 
@Injectable()
export class GameResolver implements Resolve<any> {
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private gameService: GameService,
        private uiService: UiService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {

        const network = <Network>route.params.network;
        const fieldSize = FieldSizes.find(q => q.size === route.params.size) || DefaultFieldSize;   

        this.uiService.Page = 'minefield';
        this.uiService.Network = network;
        this.uiService.Fieldsize = fieldSize;

        const userState = this.gameService.get(network).map(userState => {
            if (userState.isEmpty) {
                return UserState.empty(network, fieldSize);
            } else {
                if (userState.status !== Status.Alive) {
                    return UserState.empty(network, fieldSize);
                } else {
                    return userState;
                }
            }
        });

        return Observable.combineLatest(userState,
            Observable.of(network),
            Observable.of(fieldSize),
            (userState, network, fieldSize) => {
                return {
                    userState: userState,
                    network: network,
                    fieldSize: fieldSize
                };
            });
    }
}


 