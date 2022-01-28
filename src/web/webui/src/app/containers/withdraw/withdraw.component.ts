import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { NgForm, FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Network } from '../../models/network';
import { Observable } from 'rxjs/Observable';
import { WithdrawService, WithdrawResult } from '../../services/withdraw.service';
import { AccountService } from '../../services/account.service';
import { UserAccount } from '../../models/userAccount';

@Component({
  selector: 'app-withdraw',
  templateUrl: './withdraw.component.html',
  styleUrls: ['./withdraw.component.scss']
})
export class WithdrawComponent implements OnInit {
      withdrawForm: FormGroup;
      userAccount: UserAccount = null;
      result: WithdrawResult;
      network: Network = Network.Waves;
      networkFee = 0.0001;
      pageRange: Array<number>;
      maxPagePaging = 10;

      constructor(
        private withdrawService: WithdrawService, 
        private accountService: AccountService,
        private route: ActivatedRoute, fb: FormBuilder,
        private change: ChangeDetectorRef) {

          const pattern = '^3[MNP][1-9A-HJ-NP-Za-km-z]{33}$';

          this.withdrawForm = fb.group({
              'destinationAddress': [null, Validators.compose([Validators.required, Validators.pattern(pattern)])],
              'amount': [0.0, Validators.compose([Validators.required, this.amountMin(), this.amountMax()])],
              'password': [null, Validators.compose([Validators.required, Validators.minLength(5)])],
              'confirmationCode': [null, Validators.compose([Validators.required, Validators.pattern("^[0-9]+")])]
          });
      }

      ngOnInit() {
        this.route.queryParams.subscribe(params => {
            this.network = <Network>this.route.snapshot.params.network;
            let page = params.page || 1;
            let pageSize = params.pageSize || 10;
      
            const userAccount = this.accountService.getAccount(this.network);
            const withdrawHistory = this.withdrawService.getHistory(this.network, page, pageSize);
      
            Observable.combineLatest(userAccount, withdrawHistory,
              (userAccount, withdrawHistory) => {
                return {
                  userAccount,
                  withdrawHistory
                };
              }).subscribe(data => {
                this.userAccount = data.userAccount;
                this.result = data.withdrawHistory
                this.renderPagination();
                this.change.detectChanges();
              });
          });
      }

      renderPagination() {
        this.pageRange = new Array<number>();
        for (var i = this.result.pageIndex; i <= this.result.totalPages; i++) {
          this.pageRange.push(i);
        }
      }
      willReceive(): string {
          const spendable = this.withdrawForm.get('amount').value - this.networkFee;
          if (spendable <= 0) {
              return '0';
          }
          return spendable.toFixed(8);
      }

      max(): number {
          return 1; // this.stateService.getBalance();
      }

      amountMax(): ValidatorFn {
          return (c: AbstractControl): { [key: string]: boolean } | null => {
              // if (this.stateService.getBalance() < parseFloat(c.value)) {
              //    return { 'max': true };
              // }
              return null;
          };
      }

      amountMin(): ValidatorFn {
          return (c: AbstractControl): { [key: string]: boolean } | null => {
              if (this.networkFee >= parseFloat(c.value)) {
                  return { 'min': true };
              }
              return null;
          };
      }
 
      togglePreview($event, item){

      }

      toggleWithdraw(formValue: any): void {
          if (this.withdrawForm.valid) {
              // this.withdraw.emit(this.withdrawForm.value);
              this.withdrawForm.reset();
          }
      }

}
