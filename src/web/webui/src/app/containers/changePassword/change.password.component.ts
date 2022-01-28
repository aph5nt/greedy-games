import { Component, OnInit, ElementRef, ViewChild, ChangeDetectorRef } from '@angular/core';
import * as jQuery from 'jquery';
import { AuthService } from '../../services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { ErrorObservable } from 'rxjs/observable/ErrorObservable';
import { AccountService } from '../../services/account.service';

export interface LoginModel {
  userName?: string;
  password?: string;
}

export class PasswordValidation {

  static MatchPassword(AC: AbstractControl) {
     const formGroup = AC.parent;
     if (formGroup) {
          const passwordControl = formGroup.get('newPassword'); // to get value in input tag
          const confirmPasswordControl = formGroup.get('newPasswordConfirmation'); // to get value in input tag

          if (passwordControl && confirmPasswordControl) {
              const password = passwordControl.value;
              const confirmPassword = confirmPasswordControl.value;
              if (password !== confirmPassword) { 
                  return { matchPassword: true };
              } else {
                  return null;
              }
          }
     }

     return null;
  }
}

@Component({
  selector: 'app-change-password',
  templateUrl: './change.password.component.html',
  styleUrls: ['./change.password.component.scss']
})
export class ChangePasswordComponent implements OnInit {

  @ViewChild('loginModal') loginModal: ElementRef;
  changePasswordForm: FormGroup;
  newPassword: string;
  changingPasswordFailed = false;

  constructor(
    private authService: AuthService,
    private accountService: AccountService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private change: ChangeDetectorRef) {

    this.changePasswordForm = fb.group({
      'newPassword': ['', Validators.compose([Validators.required, Validators.pattern("^(?=.*[A-Za-z])(?=.*\d){8,}$")])],
      'newPasswordConfirmation': ['', Validators.compose([Validators.required, PasswordValidation.MatchPassword])],
      'oldPassword': [null, Validators.compose([Validators.required])],
      'twoFactorAuthCode': [null, Validators.compose([Validators.pattern("^[0-9]+")])]
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.accountService.generatePassword().subscribe(result => {
        this.newPassword = result.password;
        this.change.detectChanges();
      });
    });
  }

  submit(formValue: any) {
    if (this.changePasswordForm.valid) {
      // this.withdraw.emit(this.withdrawForm.value);
      this.changePasswordForm.reset();
    }
  }

   


}
