import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import * as model from '../../../models/game.model'
import { Network, DefaultNetwork } from '../../../models/network';

@Component({
    selector: 'game-take-away',
    styles: [`
         
        .btn-takeaway {
            color: white;
            border-color: #195FB4;
            background-color: #195FB4;
            width: 300px;
        }
    `],
    template: `
    <div class="">
    <br />
        <div *ngIf="canTakeAway()">
            <form novalidate>
                <div class="form-group text-center">
                    <button type="button" (click)="takeAwayToggle()" class="btn btn-takeaway">
                        Take Away {{amount() | currency }} 
                        <span innerHTML='{{network() | uppercase | currencyPrefix}}'></span>
                    </button>
                </div>
            </form>
        </div>
    </div>
    `
})
export class TakeAwayComponent {

    @Input() userState: model.UserState;
    @Output() takeAway = new EventEmitter<number>();

    amount(): number {
        if (this.userState === null) {
            return 0;
        }

        const position = this.userState.position.x;
        return this.userState.fieldSize.multipiers[position] * this.userState.bet;
    }

    network(): Network {
        if (this.userState === null) {
            return DefaultNetwork;
        }

        return this.userState.network;
    }

    canTakeAway(): boolean {
        if (this.userState === null) {
            return false;
        }

        return this.userState.position.x >= 0 && this.userState.status === model.Status.Alive;
    }

    takeAwayToggle(): void {
        this.takeAway.emit();
    }
}
