import { Component } from '@angular/core';

@Component({
    selector: 'app-backbutton',
    template: `
    <a class="float-right" (click)="back()">
        <i class="fa fa-xs fa-arrow-circle-left" aria-hidden="true"></i>
    </a>
    `
})
export class BackButtonComponent {
     
   back(){
    window.history.back();
   }
}