import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { GameComponent } from './game.component';
import { GameService } from './game.service';
import { GameResolver } from './game.resolver';

import { FieldComponent } from './field/field.component';
import { MultiplierComponent } from './multiplier/multiplier.component';
import { SettingsComponent } from './settings/settings.component';
import { TakeAwayComponent } from './takeaway/takeaway.component';
import { ResultComponent } from './result/result.component';
import { MessageComponent } from "./message/message.component";
import {CurrencyModule} from "../app/currency.module";
import {NetworkGuard} from "./game.guard.network";
import {MessageModule} from "../app/message.module";

@NgModule({
    declarations: [
        GameComponent,
        FieldComponent,
        MultiplierComponent,
        SettingsComponent,
        TakeAwayComponent,
        ResultComponent,
        MessageComponent
    ],
    providers: [
        GameService,
        GameResolver,
        NetworkGuard
    ],
    imports: [
        CommonModule,
        FormsModule,
        HttpModule,
        CurrencyModule,
        MessageModule,
        RouterModule.forRoot([
           // { path: '', redirectTo: ':network?size=6x3', pathMatch: 'full' },
            {
                path: ':network',
                component: GameComponent,
                canActivate: [NetworkGuard],//, FieldSizeGuard, AuthGuard],
                resolve: {
                    item: GameResolver
                }
            },
            { path: '**', redirectTo: '/free?size=6x3' }
        ],
            { enableTracing: true }
        )
    ]
})
export class GameModule { }
