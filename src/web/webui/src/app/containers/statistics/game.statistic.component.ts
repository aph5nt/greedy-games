import { Component, OnInit, ChangeDetectorRef, AfterViewInit, EventEmitter, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GameStatistic, GameStatisticResult } from '../../models/game.statistic.model';
import { Network } from '../../models/network';
import { Observable } from 'rxjs/Observable';
import { UiService } from '../../services/ui.service';
import { GameStatisticService } from '../../services/game.statistic.service';
import { CentBit } from '../../components/currency/currency.converter.pipe';
import { GameLog, UserState } from '../../models/game.model';
import { GameLogService } from '../../services/gamelog.service';
import { Helpers } from '../../models/functions';

@Component({
  selector: 'app-statistics',
  templateUrl: './game.statistic.component.html',
  styleUrls: ['./game.statistic.component.scss']
})
export class GameStatisticComponent implements OnInit {
  network: Network;
  result: GameStatisticResult;
  pageRange: Array<number>;
  maxPagePaging = 10;
  showAll: boolean;

  constructor(public helpers: Helpers,
    private route: ActivatedRoute,
    private router: Router,
    private uiService: UiService,
    private gameStatisticService: GameStatisticService,
    private gameLogService: GameLogService,
    private change: ChangeDetectorRef) { }

  ngOnInit() {

    this.route.queryParams.subscribe(params => {
      this.network = <Network>this.route.snapshot.params.network;
      let page = params.page || 1;
      let pageSize = params.pageSize || 15;
      this.showAll = String(true) === params.showAll;
      const data = this.gameStatisticService.get(this.network, this.showAll, page, pageSize).subscribe(result => {
        this.result = result;
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

  showProfit(item) {
    if (item.loss > 0) {
      return new CentBit().transform(-item.loss);
    } else {
      return " " + new CentBit().transform(item.win - item.bet);
    }
  }
 
  navigateToGameLog(data: GameStatistic) {
   this.router.navigate([`${data.network}/gamelog/${data.userName}/${data.gameId}`]);
  }

  onShowAllCheck() {
    this.router.navigate([`/${this.network}/statistics`], { queryParams: { showAll: this.showAll } });
  }
}
