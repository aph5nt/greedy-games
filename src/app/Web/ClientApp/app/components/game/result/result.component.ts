import { Component, OnChanges, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { trigger, state, style, transition, animate } from '@angular/animations'
import * as model from "../game.model";
 
@Component({
    selector: 'game-result',
    animations: [

        trigger('showWin', [
            state('0', style({ 'z-index': '100', opacity: 0 })),
            state('1', style({ 'z-index': '100', opacity: 1 })),
            transition('0 => 1', animate('600ms ease-in'))
        ]),

        trigger('showLose', [
            state('0', style({ 'z-index': '100', opacity: 0, transform: 'scale(0.0)' })),
            state('1', style({ 'z-index': '100', opacity: 1, transform: 'scale(1.0)' })),
            transition('0 => 1', animate('600ms ease-in'))
        ])

    ],
    styleUrls: ['./result.component.scss'],
    template: `
    <div [hidden]="!showWin()" [@showWin]="showWin()">
         <img  src="/images/win.svg" class="result"/>
    </div>

     <div [hidden]="!showLose()" [@showLose]="showLose()">
         <img src="/images/lose.svg" class="result"/>
    </div>`

})
export class ResultComponent {

    @Input() userState: model.UserState;

    showWin(): boolean {
        return this.userState && (this.userState.status === model.Status.Escaped || this.userState.status === model.Status.TakeAway);
    }

    showLose(): boolean {
        return this.userState && this.userState.status === model.Status.Dead;
    }

}
