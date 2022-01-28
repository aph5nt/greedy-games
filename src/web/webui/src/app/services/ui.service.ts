import { Injectable } from '@angular/core';
import { Network, DefaultNetwork, Networks, Balance } from '../models/network';
import { Observable } from 'rxjs/Observable';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/distinctUntilChanged';
import { distinctUntilChanged } from 'rxjs/operators';
import { FieldSize, DefaultFieldSize } from '../models/game.model';

export class UiState {
  constructor(public network: Network, public balances: { [network: string]: number }, public chatOpened: boolean, public page: string, public fieldSize: FieldSize) { }

  public static Empty() {
    var balances = {
      ["free"]: 0,
      ["waves"]: 0
    }
    return new UiState(DefaultNetwork, balances, false, '', DefaultFieldSize);
  }
}

@Injectable()
export class UiService {

  storageKey = 'uistate';

  private uiStateSubject = new BehaviorSubject<UiState>(UiState.Empty());
  public State = this.uiStateSubject.distinctUntilChanged();

  public NetworkObservable = this.uiStateSubject.map(x => x.network).distinctUntilChanged();
 
  constructor() {
    const state = localStorage.getItem(this.storageKey);
    if (state) {
      this.uiStateSubject.next(JSON.parse(state));
    } else {

      localStorage.setItem(
        this.storageKey,
        JSON.stringify(UiState.Empty()));
    }
  }

  private update(state: UiState) {
    this.uiStateSubject.next(state);
    localStorage.setItem(
      this.storageKey,
      JSON.stringify(state));
  }

  set Network(network: Network) {
    var state = this.uiStateSubject.value;
    state.network = network;
    this.update(state);
  }

  get GetNetwork() {
    var state = this.uiStateSubject.value;
    return state.network;
  }

  set Page(page: string) {
    var state = this.uiStateSubject.value;
    state.page = page;
    this.update(state);
  }

  set Fieldsize(fieldSize: FieldSize) {
    var state = this.uiStateSubject.value;
    state.fieldSize = fieldSize;
    this.update(state);
  }

  set Balance(balance: Balance) {
    var state = this.uiStateSubject.value;
    state.balances[balance.network] = balance.amount;
    this.update(state);
  }

  set ChatOpened(opened: boolean) {
    var state = this.uiStateSubject.value;
    state.chatOpened = opened;
    this.update(state);
  }

  get ChatOpened() : boolean {
    var state = this.uiStateSubject.value;
    return state.chatOpened;
  }
}
