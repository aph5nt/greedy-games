import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GameService } from '../../services/game.service';
import { Network } from '../../models/network';
import { Guid } from '../../models/guid';
import * as model from '../../models/game.model'
import { Observable } from 'rxjs';


@Component({
  selector: "app-game",
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: "./game.component.html",
  styleUrls: ["./game.component.scss"]
})
export class GameComponent implements OnInit {

  errorMessage$: Observable<string>;
  network: Network;
  fieldSize: model.FieldSize;
  userState: model.UserState;
  settings: model.Settings;

  constructor(
      private route: ActivatedRoute,
      private router: Router,
      private change: ChangeDetectorRef,
      private gameService: GameService
  ) {
      this.errorMessage$ = gameService.errorMessage;
  }

  ngOnInit(): void {

      this.route.data.subscribe((data) => {
          this.userState = data.item.userState;
          this.network = data.item.network;
          this.fieldSize = data.item.fieldSize;
        
          // tslint:disable-next-line:max-line-length
          if (this.userState.status === model.Status.Alive && (this.userState.network !== this.network || this.userState.fieldSize.size !== this.fieldSize.size)) {
              const url = `/game/minefield/${this.userState.network}?size=${this.userState.fieldSize.size}`;
              this.router.navigate([url]);
          }

          this.settings = model.Settings.createFrom(this.userState);
          this.change.detectChanges();

          console.log("game.component onInit")
      });
  }

  changeSize($event: any) {
      this.fieldSize = $event;
      this.userState = model.UserState.empty(this.network, this.fieldSize);
      this.change.detectChanges();
  }

  startGame($event: any): void {
      const body = new model.ClientSetting();
      body.bet = $event;
      body.seed = Guid.newGuid();
      body.x = this.fieldSize.columns;
      body.y = this.fieldSize.rows;

      this.gameService.start(this.network, body).subscribe(userState => {
        this.userState = userState;
        this.settings = model.Settings.createFrom(userState);
        this.change.detectChanges();
      });
  }

  move($event: any): void {
      const position = new model.Position();
      position.x = $event.columnIndex;
      position.y = $event.rowIndex;
      this.gameService.move(this.network, this.userState.gameId, position).subscribe(userState => {
        this.userState = userState;
        this.settings = model.Settings.createFrom(userState);
        this.change.detectChanges();
      });
  }

  takeAway($event: any): void {
      // read network from url?
        this.gameService.takeAway(this.network, this.userState.gameId).subscribe(userState => {
        this.userState = userState;
        this.settings = model.Settings.createFrom(userState);
        this.change.detectChanges();
      });
  }
}
