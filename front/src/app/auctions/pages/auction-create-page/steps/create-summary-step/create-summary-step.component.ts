import { Component, OnInit, Input } from '@angular/core';
import { CreateAuctionCommandArgs } from '../../../../../core/commands/CreateAuctionCommand';
import { AuctionCreateStep } from '../../../../auctionCreateStep';

@Component({
  selector: 'app-create-summary-step',
  templateUrl: './create-summary-step.component.html',
  styleUrls: ['./create-summary-step.component.scss']
})
export class CreateSummaryStepComponent extends AuctionCreateStep<void> implements OnInit {


  @Input("commandArgs")
  commandArgs: CreateAuctionCommandArgs;

  constructor() { super(); }

  ngOnInit() {
  }

  onOkClick() {
    this.outputOnStepComplete.emit();
  }
  checkIsReady() {
    this.onStepReady.emit();
  }

}
