import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import {GameService} from "../game.service";

@Component({
    selector: 'game-message',
    changeDetection: ChangeDetectionStrategy.Default,
    styleUrls: ['./message.component.scss'],
    template: `
    <div id="message" [ngClass]="{'show' : isVisible}">
        <div *ngFor="let msg of message | async | messageFmt">
            {{msg}} <br/>
        </div>
    </div>`
})
export class MessageComponent implements OnInit {
    @Input() message: Observable<string>;
    isVisible = false;

    constructor(private cdRef: ChangeDetectorRef, private gameService: GameService)
    {
    }

    ngOnInit(): void {
        this.message.subscribe(text => {
            if (text.length > 0) {
                console.log(text);
                this.isVisible = true;
                setTimeout(() => {
                    this.isVisible = false;
                    this.gameService.errorMessageSubject.next('');
                }, 3000);
            }
        });
    }
}