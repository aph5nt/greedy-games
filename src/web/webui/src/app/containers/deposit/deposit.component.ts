import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DepositService, Deposit, DepositResult } from '../../services/deposit.service';
import { Network } from '../../models/network';
import { UserAccount } from '../../models/userAccount';
import { Observable } from 'rxjs/Observable';
import { AccountService } from '../../services/account.service';

export interface ActivateModel {
  captcha?: string;
}

@Component({
  selector: 'app-deposit',
  templateUrl: './deposit.component.html',
  styleUrls: ['./deposit.component.scss']
})
export class DepositComponent implements OnInit {

  userAccount: UserAccount = null;
  depositResult: DepositResult;
  network: Network = Network.Waves;
  activateModel: ActivateModel = {};

  pageRange: Array<number>;
  maxPagePaging = 10;

  constructor(
    public depositService: DepositService,
    public accountService: AccountService,
    private route: ActivatedRoute,
    private router: Router,
    private change: ChangeDetectorRef) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.network = <Network>this.route.snapshot.params.network;
      let page = params.page || 1;
      let pageSize = params.pageSize || 10;

      const userAccount = this.accountService.getAccount(this.network);
      const depositHistory = this.depositService.getHistory(this.network, page, pageSize);

      Observable.combineLatest(userAccount, depositHistory,
        (userAccount, depositHistory) => {
          return {
            userAccount,
            depositHistory
          };
        }).subscribe(data => {
          this.userAccount = data.userAccount;
          this.depositResult = data.depositHistory
          this.renderPagination();
          this.change.detectChanges();
        });
    });
  }

  resolved(captchaResponse: string) {
    this.accountService.activateAccount(captchaResponse, this.network).subscribe(result => {
      this.router.navigate([`${this.network}/deposit`]);
    });
  }

  renderPagination() {
    this.pageRange = new Array<number>();
    for (var i = this.depositResult.pageIndex; i <= this.depositResult.totalPages; i++) {
      this.pageRange.push(i);
    }
  }

  togglePreview($event, item): void {
  }
}

