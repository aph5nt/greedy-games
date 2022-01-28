import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { RECAPTCHA_SETTINGS, RecaptchaModule, RecaptchaSettings } from 'ng-recaptcha';
import { RecaptchaFormsModule } from 'ng-recaptcha/forms';
import { environment } from '../environments/environment';

import { AppComponent } from './app.component';
import { HomeComponent } from './containers/main/home/home.component';
import { NavComponent } from './containers/main/nav/nav.component';
import { ChatComponent } from './containers/main/chat/chat.component';
import { RegisterComponent } from './containers/main/register/register.component';
import { RegisteredComponent } from './containers/main/registered/registered.component';
import { DepositComponent } from './containers/deposit/deposit.component';
import { BankrollComponent } from './containers/bankroll/bankroll.component';


import { WithdrawComponent } from './containers/withdraw/withdraw.component';
import { QRCodeModule } from './components/qrcode/qrcode';
import { AuthService } from './services/auth.service';
import { AccountService } from './services/account.service';
import { LoginComponent } from './containers/main/login/login.component';
import { NetworkGuard } from './network.guard';
import { AuthGuard } from './auth.guard';
import { UiService } from './services/ui.service';
import { FundsNetworkGuard } from './funds.network.guard';
import { TwoFactorAuthGuard } from './two.factor.auth.guard'
import { DepositService } from './services/deposit.service';
import { BalanceService } from './services/balance.service';
import { TwoFactorAuthService } from './services/two.factor.auth.service';
import { WithdrawService } from './services/withdraw.service';
import { HomeResolver } from './services/home.resolver';
import { GameModule } from './game.module';

import { GameStatisticComponent } from './containers/statistics/game.statistic.component';
import { GameDailyStatisticComponent } from './containers/statistics/game.daily.statistic.component';

import { GameStatisticService } from './services/game.statistic.service';
import { GameLogService } from './services/gamelog.service';
import { GameLogResolver } from './services/gamelog.resolver';

import { CurrencyModule } from './components/currency/currency.module';
import { GameLogComponent } from './containers/gamelog/gamelog.component';
import { GamePreviewComponent } from './containers/gamepreview/game.preview.component';
import { TFASetupComponent } from './containers/2FASetup/2fa-setup.component';
import { ChangePasswordComponent } from './containers/changePassword/change.password.component';


import { Helpers } from './models/functions';
import { BackButtonComponent } from './components/backbutton/backbutton';

const appRoutes: Routes = [
  {
    path: ':network/home',
    component: HomeComponent,
    canActivate: [NetworkGuard],
    resolve: {
      item: HomeResolver
    }
  },
  {
    path: '2fasetup',
    component: TFASetupComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'changepassword',
    component: ChangePasswordComponent,
    canActivate: [AuthGuard],
  },
  {
    path: ':network/withdraw',
    component: WithdrawComponent,
    canActivate: [FundsNetworkGuard, AuthGuard, TwoFactorAuthGuard],
  },
  {
    path: ':network/deposit',
    component: DepositComponent,
    canActivate: [FundsNetworkGuard, AuthGuard, TwoFactorAuthGuard]
  },
  {
    path: ':network/bankroll',
    component: BankrollComponent,
    canActivate: [AuthGuard]
  },
  {
    path: ':network/statistics',
    component: GameStatisticComponent,
    canActivate: [NetworkGuard]

  },

  {
    path: ':network/statistics/daily',
    component: GameDailyStatisticComponent,
    canActivate: [NetworkGuard]

  },

  {
    path: ':network/gamelog/:userName/:gameId',
    component: GameLogComponent,
    canActivate: [NetworkGuard, TwoFactorAuthGuard],
    resolve: {
      item: GameLogResolver
    }

  },



  
  // { path: '', redirectTo: '/free', pathMatch: 'full' },
  { path: '**', redirectTo: '/free/home' }
];

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    ChatComponent,
    DepositComponent,
    RegisterComponent,
    WithdrawComponent,
    RegisteredComponent,
    HomeComponent,
    LoginComponent,
    GameStatisticComponent,
    GameDailyStatisticComponent,
    GameLogComponent,
    GamePreviewComponent,
    BackButtonComponent,
    TFASetupComponent,
    ChangePasswordComponent,
    BankrollComponent
  ],
  imports: [
    CurrencyModule,
    RecaptchaFormsModule,
    RecaptchaModule.forRoot(),
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    QRCodeModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    GameModule,
    RouterModule.forRoot(appRoutes),
  ],
  providers: [
    Helpers,
    HomeResolver,
    BalanceService,
    DepositService,
    WithdrawService,
    UiService,
    NetworkGuard,
    FundsNetworkGuard,
    TwoFactorAuthGuard,
    AuthGuard,
    GameStatisticService,
    GameLogService,
    GameLogResolver,
    AccountService,
    TwoFactorAuthService,
    AuthService,
    {
      provide: RECAPTCHA_SETTINGS,
      useValue: { siteKey: environment.siteKey } as RecaptchaSettings,
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
