import { Component, OnInit, Input, Output, EventEmitter, ViewChild, AfterViewInit } from '@angular/core';
import { FieldSize, UserState, Status } from '../../models/game.model'
 
@Component({
    selector: 'app-gamepreview',
    styleUrls: ['./game.preview.component.scss'],
    templateUrl: './game.preview.component.html',
})
export class GamePreviewComponent {
    @Input() fieldSize: FieldSize;
    @Input() userState: UserState;

    showWin(): boolean {
        return this.userState && this.userState.status === Status.Escaped;
    }

    showLose(): boolean {
        return this.userState && this.userState.status === Status.Dead;
    }
}