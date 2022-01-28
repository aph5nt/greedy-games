import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ViewChild, ElementRef } from '@angular/core';
import * as jQuery from 'jquery';
declare var jQuery: any;

import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { RegisteredComponent } from '../registered/registered.component';
import { AuthService } from '../../../services/auth.service';
import { AccountService, UserCount } from '../../../services/account.service';
import { LoginComponent } from '../login/login.component';

export interface RegisterModel {
  captcha?: string;
}

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  @ViewChild('registerModal') registerModal: ElementRef;
  @ViewChild('registeredModal') registeredModal: RegisteredComponent;
  @ViewChild('loginModal') loginModal: LoginComponent;

  totalPlayers = 0;
  beginRegistration = false;
  registerModel: RegisterModel = {};

  constructor(private accountService: AccountService) {

  }

  ngOnInit(): void {
    this.accountService.getUserCount().subscribe(userCount => {
      this.totalPlayers = userCount.count;
    });
  }

  show() {
    this.clear();
    jQuery(this.registerModal.nativeElement).modal('show');
  }

  close() {
    this.clear();
    jQuery(this.registerModal.nativeElement).modal('hide');
  }

  join() {
    this.beginRegistration = true;
  }

  private clear() {
    this.registerModel = {};
    this.beginRegistration = false;
  }

  resolved(captchaResponse: string) {
    this.accountService.createAccount(captchaResponse).subscribe(result => {
      this.registeredModal.show(result.userName, result.password);
      this.close();
    });
  }

  login() {
    this.close();
    this.loginModal.show();
  }
}
