import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { NgForm, FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Network, Balance } from '../../models/network';
import { Observable } from 'rxjs/Observable';
import { WithdrawService, WithdrawResult } from '../../services/withdraw.service';
import { AccountService } from '../../services/account.service';
import { UserAccount, GameAccount } from '../../models/userAccount';
import { DepositResult, DepositService } from '../../services/deposit.service';
import { BalanceService } from '../../services/balance.service';

@Component({
    selector: 'app-bankroll',
    templateUrl: './bankroll.component.html',
    styleUrls: ['./bankroll.component.scss']
})
export class BankrollComponent implements OnInit {

    gameAccount: GameAccount = null;
    withdrawResult: WithdrawResult = null;
    depositResult: DepositResult = null;
    balance: Balance;

    network: Network = Network.Waves;

    pageRange: Array<number>;
    maxPagePaging = 10;

    constructor(
        private withdrawService: WithdrawService,
        private depositService: DepositService,
        private balanceService: BalanceService,
        private accountService: AccountService,
        private route: ActivatedRoute, fb: FormBuilder,
        private change: ChangeDetectorRef) {




    }

    ngOnInit() {
        this.route.queryParams.subscribe(params => {
            this.network = <Network>this.route.snapshot.params.network;
            let page = params.page || 1;
            let pageSize = params.pageSize || 10;


            const balance = this.balanceService.getBankrollBalance(this.network);

            if (this.network != Network.Free) {
                const gameAccount = this.accountService.getBankrollAccount(this.network);
                const withdrawHistory = this.withdrawService.getBankrollHistory(this.network, page, pageSize);
                const depositHistory = this.depositService.getBankrollHistory(this.network, page, pageSize);

                Observable.combineLatest(gameAccount, withdrawHistory, depositHistory, balance,
                    (gameAccount, withdrawHistory, depositHistory, balance) => {
                        return {
                            gameAccount,
                            withdrawHistory,
                            depositHistory,
                            balance
                        };

                    }).subscribe(data => {
                        this.gameAccount = data.gameAccount;
                        this.depositResult = data.depositHistory;
                        this.withdrawResult = data.withdrawHistory;
                        this.balance = data.balance;

                        this.renderPagination(this.depositResult);
                        this.renderPagination(this.withdrawResult);
                        this.change.detectChanges();
                    });
            }
            else {
                balance.subscribe(data => {
                    this.balance = data;
                });
            }
        });
    }

    renderPagination(result: DepositResult | WithdrawResult) {
        this.pageRange = new Array<number>();
        for (var i = result.pageIndex; i <= result.totalPages; i++) {
            this.pageRange.push(i);
        }
    }

    togglePreview($event, item) {

    }

}
