import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './components/app/app.component';
import { GameModule } from './components/game/game.module';

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        GameModule,
        RouterModule.forRoot([
            { path: '', redirectTo: '/', pathMatch: 'full' },
           // { path: 'game', component: AppComponent },
            { path: '**', redirectTo: '/' }
        ])
       
    ]
})
export class AppModuleShared {
}
