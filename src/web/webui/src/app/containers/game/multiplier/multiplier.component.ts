import { Component, Input } from '@angular/core';
import * as model from "../../../models/game.model";
 
@Component({
    selector: 'game-multipier',
    templateUrl: './multiplier.component.html',
    styleUrls: ['./multiplier.component.scss']
})
export class MultiplierComponent {
    @Input() fieldSize: model.FieldSize;
}