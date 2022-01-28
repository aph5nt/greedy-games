import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Network } from '../../../models/network';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  network: Network;
  constructor( private route: ActivatedRoute,
    private router: Router,
    private change: ChangeDetectorRef) { }

  ngOnInit() {
    this.route.data.subscribe(
      (data) => {
        this.network = data.item.network;
      }
    );

    console.log("home.component oninit");
  }
   
}
