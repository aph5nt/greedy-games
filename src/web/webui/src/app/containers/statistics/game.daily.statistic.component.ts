import { Component, OnInit, ChangeDetectorRef, AfterViewInit, EventEmitter, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GameDailyStatisticResult, GameDailyStatistic } from '../../models/game.statistic.model';
import { Network } from '../../models/network';
import { Observable } from 'rxjs/Observable';
import { UiService } from '../../services/ui.service';
import { GameStatisticService } from '../../services/game.statistic.service';
import { CentBit } from '../../components/currency/currency.converter.pipe';
import { GameLog, UserState } from '../../models/game.model';
import { GameLogService } from '../../services/gamelog.service';
import { Helpers } from '../../models/functions';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-daily-statistics',
  templateUrl: './game.daily.statistic.component.html',
  styleUrls: ['./game.daily.statistic.component.scss']
})
export class GameDailyStatisticComponent implements OnInit {
  network: Network;
  result: GameDailyStatisticResult;
  pageRange: Array<number>;
  maxPagePaging = 10;

  showAll: boolean;

  constructor(public helpers: Helpers,
    private route: ActivatedRoute,
    private router: Router,
    private uiService: UiService,
    private gameStatisticService: GameStatisticService,
    private change: ChangeDetectorRef) {
  }

  ngOnInit() {

    this.route.queryParams.subscribe(params => {
      this.network = <Network>this.route.snapshot.params.network;
      let page = params.page || 1;
      let pageSize = params.pageSize || 15;

      this.showAll = String(true) === params.showAll;
       
      const data = this.gameStatisticService.getDaily(this.network, this.showAll, page, pageSize).subscribe(result => {
        this.result = result;
        this.renderPagination();
        this.change.detectChanges();
      });
    });

  }

  onShowAllCheck() {
    this.router.navigate([`/${this.network}/statistics/daily`], { queryParams: { showAll: this.showAll } });
  }

  renderPagination() {
    this.pageRange = new Array<number>();
    for (var i = this.result.pageIndex; i <= this.result.totalPages; i++) {
      this.pageRange.push(i);
    }
  }
}
