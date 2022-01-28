import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { GameComponent } from './containers/game/game.component';
import { FieldComponent } from './containers/game/field/field.component';
import { MultiplierComponent } from './containers/game/multiplier/multiplier.component';
import { SettingsComponent } from './containers/game/settings/settings.component';
import { TakeAwayComponent } from './containers/game/takeaway/takeaway.component';
import { ResultComponent } from './containers/game/result/result.component';
import { MessageComponent } from './containers/game/message/message.component';
import { GameService } from './services/game.service';
import { GameResolver } from './services/game.resolver';
import { NetworkGuard } from './network.guard';
import { CurrencyModule } from './components/currency/currency.module'
import { MessageModule } from './components/message/message.module'
import { FieldSizeGuard } from './fieldSize.guard';
import { AuthGuard } from './auth.guard';

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
        NetworkGuard,
        FieldSizeGuard,
        AuthGuard
    ],
    imports: [
        CommonModule,
        FormsModule,
        HttpModule,
        CurrencyModule,
        MessageModule,
        RouterModule.forRoot([
            {
                path: ':network/minefield/:size',
                component: GameComponent,
                canActivate: [NetworkGuard, FieldSizeGuard, AuthGuard],
                resolve: {
                    item: GameResolver
                }
            }
        ],
            { enableTracing: true }
        )
    ]
})
export class GameModule { }
