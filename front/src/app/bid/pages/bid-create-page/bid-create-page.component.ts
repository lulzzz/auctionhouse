import { Component, OnInit, OnDestroy } from '@angular/core';
import { Auction } from '../../../core/models/Auctions';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { BidCommand } from '../../../core/commands/BidCommand';
import { ServerMessage, ServerMessageService } from 'src/app/core/services/ServerMessageService';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-bid-create-page',
  templateUrl: './bid-create-page.component.html',
  styleUrls: ['./bid-create-page.component.scss']
})
export class BidCreatePageComponent implements OnInit, OnDestroy {
  private connectionStartedSub: Subscription;

  showForm = false;
  auction: Auction;
  form = new FormGroup({
    price: new FormControl('', [Validators.required])
  });

  constructor(private activatedRoute: ActivatedRoute, private bidCommand: BidCommand, private router: Router,
              private serverMessageService: ServerMessageService) {
    this.activatedRoute.data.subscribe((data) => {
      this.auction = data.auction;
    });
  }

  ngOnInit() {
    this.connectionStartedSub = this.serverMessageService.connectionStarted.subscribe((connected) => {
      if (connected) {
        this.showForm = true;
      } else {
        this.router.navigate(['/home']);
      }
    });
    this.serverMessageService.ensureConnected();
  }

  ngOnDestroy(): void {
    this.connectionStartedSub.unsubscribe();
  }

  onBidSubmit() {
    if (this.form.valid) {

      this.bidCommand
        .execute(this.auction.auctionId, this.form.value.price, '1234')
        .subscribe((msg: ServerMessage) => {
          if (msg.result === 'completed') {
            this.router.navigate(['/auction'], { queryParams: { auctionId: this.auction.auctionId } });
          } else {
            console.log('error ' + msg);
          }
        });
    }
  }

}
