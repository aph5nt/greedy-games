import { Injectable, } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/combineLatest';
import { UiService } from './ui.service';
import { Network } from '../models/network';
import { GameStatisticService } from './game.statistic.service'
import { GameLogService } from './gamelog.service'

@Injectable()
export class GameLogResolver implements Resolve<any> {

    constructor(
        private gameLogService: GameLogService,
        private gameStatisticService: GameStatisticService,
        private uiService: UiService,
        private router: Router) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {

        const network = <Network>route.params.network;
        const userName = route.params.userName;
        const gameId = route.params.gameId;

        this.uiService.Page = 'gamedetail';
        this.uiService.Network = network;

        const gameLog = this.gameLogService.get(network, userName, gameId);
        const statistic = this.gameStatisticService.getSingle(network, userName, gameId);

        return Observable.combineLatest(Observable.of(network), gameLog, statistic,
            // tslint:disable-next-line:no-shadowed-variable
            (network, gameLog, statistic) => {
                return {
                    network, gameLog, statistic
                };
            });
    }
}
