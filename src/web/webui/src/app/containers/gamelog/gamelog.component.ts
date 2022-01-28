import { Component, OnInit, ViewChild, Input, ChangeDetectorRef } from '@angular/core';
import { UserState, GameLog } from '../../models/game.model';
import { ModalDirective } from 'ngx-bootstrap';
import { GameStatistic } from '../../models/game.statistic.model';
import { ActivatedRoute, Router } from '@angular/router';
import { Network } from '../../models/network';
import { Helpers } from '../../models/functions';

@Component({
  selector: 'app-gamelog',
  templateUrl: './gamelog.component.html',
  styleUrls: ['./gamelog.component.scss']
})
export class GameLogComponent implements OnInit {

  network: Network;
  log: GameLog;
  statistic: GameStatistic;
  userState: UserState;

  constructor(
    public helpers: Helpers,
    private route: ActivatedRoute,
    private router: Router,
    private change: ChangeDetectorRef) { }

  ngOnInit() {
    this.route.data.subscribe(
      (data) => {
        this.network = data.item.network;
        this.log = data.item.gameLog;
        this.userState = data.item.gameLog.userState;
        this.statistic = data.item.statistic;
        this.change.detectChanges();
      }
    );
  }


  
}
 