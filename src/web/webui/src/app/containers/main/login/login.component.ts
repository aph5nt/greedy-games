import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import * as jQuery from 'jquery';
import { AuthService } from '../../../services/auth.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ErrorObservable } from 'rxjs/observable/ErrorObservable';
declare var jQuery: any;

export interface LoginModel {
  userName?: string;
  password?: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  @ViewChild('loginModal') loginModal: ElementRef;
  loginForm: FormGroup;
  failedToLogin = false;

  constructor(private authService: AuthService, fb: FormBuilder, private router: Router) {
    this.loginForm = fb.group({
      'userName': [null, Validators.compose([Validators.required])],
      'password': [null, Validators.compose([Validators.required])],
      'twoFactorAuthCode': [null, Validators.compose([Validators.pattern("^[0-9]+")])]
    });
  }

  ngOnInit() {

  }

  show() {
    jQuery(this.loginModal.nativeElement).modal('show');
  }

  login(loginForm) {
    if (this.loginForm.valid) {

      this.authService.login(this.loginForm.value.userName, this.loginForm.value.password, this.loginForm.value.twoFactorAuthCode)
      .subscribe(result => {
        this.failedToLogin = false;
        this.router.navigate(['/free']);
        this.close();
      }, error => {
        this.failedToLogin = true;
        console.log(error);
      });
    }
  }

  close() {
    jQuery(this.loginModal.nativeElement).modal('hide');
  }
}
