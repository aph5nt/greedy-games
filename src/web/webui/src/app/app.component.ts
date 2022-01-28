import { Component } from '@angular/core';
import { ViewChild } from '@angular/core';
import { OnInit } from '@angular/core/src/metadata/lifecycle_hooks';
import { RegisterComponent } from './containers/main/register/register.component';
import { AuthService, AuthState } from './services/auth.service';
import { Route } from '@angular/compiler/src/core';
import { ActivatedRouteSnapshot, ActivatedRoute } from '@angular/router';
import { UiService, UiState } from './services/ui.service';
import { Network, Balance } from './models/network';
import { RouterStateSnapshot } from '@angular/router/src/router_state';
import { HubConnection } from '@aspnet/signalr-client';
import { environment } from '../environments/environment';
import { BalanceService } from './services/balance.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  @ViewChild('registerModal') registerModal: RegisterComponent;
  title = 'app';

  uiState: UiState;
  authState: AuthState;

   
  hubConnection: HubConnection;

  constructor(public authService: AuthService, public uiService: UiService, public balanceService: BalanceService) {
  }

  ToggleChat() {
    this.uiService.ChatOpened = !this.uiService.ChatOpened;
  }

  ngOnInit(): void {


    this.authService.AuthState.subscribe(state => {
      this.authState = state;

      if (state.isLogged) {

        const auth = this.authService.getAuth();

        this.hubConnection = new HubConnection(`${environment.apiBaseUrl}/balances?hubToken=${auth.token}`);

        this.hubConnection.on('onBalanceUpdated', (data: Balance) => {
          this.uiService.Balance = data;
        });

        this.hubConnection
          .start()
          .then(() => console.log('Connection started!'))
          .catch(err => console.log('Error while establishing connection :('));

        this.balanceService.getUserBalance(this.uiService.GetNetwork).subscribe(data => {
          this.uiService.Balance = data;
        });



      } else {

        if(this.hubConnection){
          this.hubConnection.stop();
        }
        

      }
    });

    this.uiService.State.subscribe(state => {

     
this.uiState = state;
    
      
    });



    if (this.authService.isLoggedIn() === false) {

      this.registerModal.show();
      

    } else {
      this.authService.getAuth();
    }

    this.uiService.NetworkObservable.subscribe(network => {
      if (this.authService.isLoggedIn()) {
        this.balanceService.getUserBalance(network).subscribe(data => {
          this.uiService.Balance = data;
        })
      }
    });

  }
}
