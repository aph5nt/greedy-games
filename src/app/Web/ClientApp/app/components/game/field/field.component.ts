import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import * as model from "../game.model";
import { NgForOf } from '@angular/common';

@Component({
    selector: 'game-field',
    styleUrls: ['./field.component.scss'],
    templateUrl: './field.component.html',
})
export class FieldComponent implements OnInit {

    @Input() field: model.Field;
    @Input() fieldSize: model.FieldSize;
    @Output() move = new EventEmitter<model.Field>();

    stateStr: string;

    ngOnInit(): void {
        this.stateStr = this.field.state.toString();
    }

    moveToggle(columnIndex: number, rowIndex: number): void {
       if (this.field.canStepOn){
           this.move.emit(this.field);
       }
    }
}