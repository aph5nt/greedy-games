import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import * as model from '../game.model'

@Component({
    selector: 'game-take-away',
    styles: [`
        .btn-takeaway {
            color: white;
            border-color: #195FB4;
            background-color: #195FB4;
            width: 220px;
        }
    `],
    template: `
    <div class="col-md-4 col-md-offset-4">
    <br />
        <div *ngIf="canTakeAway()">
            <form novalidate>
                <div class="form-group">
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

    network(): model.Network {
        if (this.userState === null) {
            return model.defaultNetwork;
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
