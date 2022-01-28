import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import * as model from "../../../models/game.model";
 
@Component({
    selector: 'game-board-settings',
    templateUrl: './settings.component.html',
    styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

    @Input() model: model.Settings;
    @Output() startGame = new EventEmitter<number>();
    @Output() sizeChanged = new EventEmitter<model.FieldSize>();

    fieldSizes: model.FieldSize[];
    bet: number;

    constructor() {
        this.fieldSizes = model.FieldSizes;
    }

    ngOnInit(): void {
       this.bet = this.satoshiToCent(this.model.bet);
    }

    centToSatoshi(value: number) {
        return value * model.cent;
    }

    satoshiToCent(value: number) {
        return value / model.cent;
    }

    canStart(): boolean {
        if (this.model === null) {
            return false;
        }

        return this.model.status !== model.Status.Alive;
    }
 
    sizeChangeToggle(fieldSize: model.FieldSize) {
        this.model.fieldSize = fieldSize;
        this.sizeChanged.emit(fieldSize);
    }

    startGameToggle(): void {
        this.model.bet = this.centToSatoshi(this.bet);
        this.startGame.emit(this.model.bet);
    }

    x12(): void {
        const newVal = this.centToSatoshi(this.bet / 2);
        if (this.model.min < newVal) {
            this.bet = this.satoshiToCent(newVal);
        } else {
            this.bet = this.satoshiToCent(this.model.min);
        }
    }

    x2(): void {
        const newVal = this.centToSatoshi(this.bet * 2);
        if (this.model.max > newVal) {
            this.bet = this.satoshiToCent(newVal);
        } else {
            this.bet = this.satoshiToCent(this.model.max);
        }
    }

    max(): void {
        this.bet = this.satoshiToCent(this.model.max);
    }
}