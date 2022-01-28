import { Component, OnInit, ElementRef, ViewChild, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService, TokenResponse, AuthState } from '../../../services/auth.service';
import { RegisterComponent } from '../register/register.component';
import { LoginComponent } from '../login/login.component';
import { UiService, UiState } from '../../../services/ui.service';
import { Networks, Network, Balance } from '../../../models/network';
import { HubConnection } from '@aspnet/signalr-client';
import { environment } from '../../../../environments/environment'
import { BalanceService } from '../../../services/balance.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {

  @Input() uiState: UiState;
  @Input() authState: AuthState;

  @ViewChild('registerModal') registerModal: RegisterComponent;
  @ViewChild('loginModal') loginModal: LoginComponent;

  Networks = Networks;

  constructor(
    public authService: AuthService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit() {
    
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  register() {
    this.registerModal.show();
  }

  login() {
    this.loginModal.show();
  }
 
}
