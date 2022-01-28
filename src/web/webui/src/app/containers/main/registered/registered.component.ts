import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import * as jQuery from 'jquery';
declare var jQuery: any;

export interface UserCreatedModel {
  userName?: string;
  password?: string;
}

@Component({
  selector: 'app-registered',
  templateUrl: './registered.component.html',
  styleUrls: ['./registered.component.scss']
})
export class RegisteredComponent {

  @ViewChild('userCreatedModal') userCreatedModal: ElementRef;
  userCreatedModel: UserCreatedModel = {};

  constructor(private authService: AuthService) { }

  show(userName, password) {
    this.userCreatedModel = {
      userName: userName,
      password: password
    };

    jQuery(this.userCreatedModal.nativeElement).modal('show');
  }

  close() {
    const userName = this.userCreatedModel.userName;
    const password = this.userCreatedModel.password;

    this.authService.login(userName, password, null).subscribe(result => {

      jQuery(this.userCreatedModal.nativeElement).modal('hide');
      this.userCreatedModel = {};

    }, error => {
      console.log(error);
    });
  }
}
