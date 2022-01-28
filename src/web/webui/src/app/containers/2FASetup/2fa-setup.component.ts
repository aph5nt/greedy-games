import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Network, } from '../../models/network';
import { UserAccount, } from '../../models/userAccount';
import { Observable } from 'rxjs/Observable';
import { TwoFactorAuthService, TwoFactorAuthCode } from '../../services/two.factor.auth.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';

export interface VerificationModel {
  twoFactorAuthCode?: number;
}

@Component({
  selector: 'app-2fa-setup',
  templateUrl: './2fa-setup.component.html',
  styleUrls: ['./2fa-setup.component.scss']
})
export class TFASetupComponent implements OnInit {

  showConfirmationForm: boolean = false;
  showResetForm: boolean = false;

  manualSetupKey: string = null;
  qrCodeImage: string = null;
  confirmationForm: FormGroup;
  disableForm: FormGroup;

  constructor(
    public twoFactorAuthService: TwoFactorAuthService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private change: ChangeDetectorRef) {
    this.confirmationForm = fb.group({
      'confirmationCode': [null, Validators.compose([Validators.required, Validators.pattern("^[0-9]+")])],
    });

    this.disableForm = fb.group({
      'confirmationCode': [null, Validators.compose([Validators.required, Validators.pattern("^[0-9]+")])],
    });
  }

  private load() {
    this.twoFactorAuthService.get().subscribe(getResult => {
      this.showResetForm = getResult.enabled;

      if (!getResult.enabled) {

        this.showConfirmationForm = true;
        this.showResetForm = false;

        var twoFactorAuthSetup = this.twoFactorAuthService.generate().subscribe(generateResult => {
          this.manualSetupKey = generateResult.manualSetupKey;
          this.qrCodeImage = generateResult.qrCodeImage;
          this.change.detectChanges();
        });

      } else {
        this.showResetForm = true;
        this.showConfirmationForm = false;
        this.change.detectChanges();
      }
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.load();
    });
  }

  toggleEnable(formValue: any): void {

    if (this.confirmationForm.valid) {
      const code = formValue.confirmationCode;

      this.twoFactorAuthService.enable(new TwoFactorAuthCode(code)).subscribe(result => {
        if (result.status === 204) {
          this.confirmationForm.reset();
          this.load();
        }
      });
    }
  }

  toggleDisable(formValue: any): void {
    if (this.disableForm.valid) {
      const code = formValue.confirmationCode;

      this.twoFactorAuthService.disable(new TwoFactorAuthCode(code)).subscribe(result => {
        if (result.status === 204) {
          this.disableForm.reset();
          this.load();
        }
      });
    }
  }


}