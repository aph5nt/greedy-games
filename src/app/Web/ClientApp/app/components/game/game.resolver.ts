import { Injectable, } from "@angular/core";
import { Location } from '@angular/common';
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot, ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs/Rx";
import { Network, networks, fieldSizes, defaultFieldSize, Status, UserState } from "./game.model";
import { GameService } from "./game.service";


@Injectable()
export class GameResolver implements Resolve<any> {
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private location: Location,
        private gameService: GameService) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {
        const pathArray = this.location.path().split('/');
        const networkResult = networks.find(q => q === pathArray[1]);
        const network = networkResult ? networkResult : Network.Free;
        const fieldSize = fieldSizes.find(q => q.size === route.queryParams["size"]) || defaultFieldSize;   

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