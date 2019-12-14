import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { CreateAuctionCommandArgs, CreateAuctionCommand } from '../../../core/commands/CreateAuctionCommand';
import { CategorySelectStep } from '../../../shared/forms/category-select-step/categorySelectStep';
import { ProductFormResult } from '../../../shared/forms/product-step/productStep';
import { Router } from '@angular/router';
import { StartAuctionCreateSessionCommand } from '../../../core/commands/StartAuctionCreateSessionCommand';
import { AuctionDataStep } from '../../../shared/forms/auction-data-step/auctionDataStep';
import { AddImageFormResult } from '../../../shared/forms/add-image-step/add-image-step.component';
import { AddAuctionImageCommand } from 'src/app/core/commands/AddAuctionImageCommand';
import { RemoveAuctionImageCommand } from 'src/app/core/commands/RemoveAuctionImageCommand';
import { ImgSelectedEvent } from 'src/app/shared/forms/img-upload-input/img-upload-input.component';
import { AuctionImageQuery } from '../../../core/queries/AuctionImageQuery';

@Component({
  selector: 'app-auction-create-page',
  templateUrl: './auction-create-page.component.html',
  styleUrls: ['./auction-create-page.component.scss']
})
export class AuctionCreatePageComponent implements OnInit {
  private totalSteps = 4;
  private categorySelectStep: CategorySelectStep;
  private productStep: ProductFormResult;
  private auctionDataStep: AuctionDataStep;

  @ViewChild('categoryStep', { static: true })
  categoryStepComponent;
  @ViewChild('productStep', { static: true })
  productStepComponent;
  @ViewChild('imageStep', { static: true })
  imageStepComponent;
  @ViewChild('summaryStep', { static: true })
  summaryStepComponent;
  @ViewChild('auctionDataStep', { static: true })
  auctionDataStepComponent;

  createAuctionArgs: CreateAuctionCommandArgs;
  error: string = null;
  showCreateForm = false;

  step = 0;
  stepComponents = [];

  constructor(private startAuctionCreateSessionCommand: StartAuctionCreateSessionCommand,
    private createAuctionCommand: CreateAuctionCommand,
    private addAuctionImageCommand: AddAuctionImageCommand,
    private removeAuctionImageCommand: RemoveAuctionImageCommand,
    private auctionImageQuery: AuctionImageQuery,
    private router: Router,
    public location: Location) {
  }

  ngOnInit() {
    this.startAuctionCreateSession();
    this.stepComponents = [
      this.categoryStepComponent,
      this.auctionDataStepComponent,
      this.productStepComponent,
      this.imageStepComponent,
      this.summaryStepComponent
    ];
  }

  private nextStep() {
    this.step++;
  }

  private previousStep() {
    this.step--;
  }

  private startAuctionCreateSession() {
    console.log('starting session');
    this.startAuctionCreateSessionCommand.execute().subscribe((msg) => {
      if (msg.status === 'COMPLETED') {
        console.log('session started');
        this.showCreateForm = true;
      } else {
        console.log('Cannot start session');
        this.router.navigate(['/error']);
      }
    }, (err) => {
      console.log('Cannot start session');
      this.router.navigate(['/error']);
    });
  }

  private createCommandArgs() {
    const categories = [this.categorySelectStep.selectedMainCategory.categoryName,
    this.categorySelectStep.selectedSubCategory.categoryName,
    this.categorySelectStep.selectedSubCategory2.categoryName];
    this.createAuctionArgs = {
      buyNowPrice: this.auctionDataStep.buyNowPrice,
      startDate: this.auctionDataStep.startDate,
      endDate: this.auctionDataStep.endDate,
      category: categories,
      product: this.productStep.product,
      tags: this.productStep.tags,
      name: this.auctionDataStep.name,
      buyNowOnly: this.auctionDataStep.buyNow && !this.auctionDataStep.auction
    };
  }

  onCategorySelectedStep(stepResult: CategorySelectStep) {
    this.categorySelectStep = stepResult;
    this.nextStep();
  }

  onAuctionDataStep(stepResult: AuctionDataStep) {
    this.auctionDataStep = stepResult;
    this.nextStep();
  }

  onProductStep(stepResult: ProductFormResult) {
    this.productStep = stepResult;
    this.nextStep();
  }

  onImagesStep(stepResult: AddImageFormResult[]) {
    console.log('img step = ');
    console.log(stepResult);
    this.createCommandArgs();
    console.log(this.createAuctionArgs);

    this.nextStep();
  }

  onImgSelect(event: ImgSelectedEvent) {
    this.addAuctionImageCommand.execute(event.files, event.imgnum).subscribe((msg) => {
      if (msg.status === 'COMPLETED') {
        console.log('add image completed');
        console.log(msg);
        this.imageStepComponent.setImgPreview(event.imgnum, this.auctionImageQuery.execute(msg.extraData.imgSz1));
      } else {
        console.log('Cannot add image');
      }
    }, (err) => {
      console.log('Cannot add image');
    });
  }

  onImgCancel(imgnum: number) {
    this.removeAuctionImageCommand.execute(imgnum).subscribe((v) => {
      console.log("img removed " + imgnum);
      this.imageStepComponent.setImgPreview(imgnum, null);
    }, (err) => {
      console.log("remove image error ");
      console.log(err);
    });
  }

  onForwardClick() {
    if (this.step + 1 < this.totalSteps) {
      this.nextStep();
    }
  }

  onBackClick() {
    if (this.step - 1 >= 0) {
      this.previousStep();
    }
  }

  onOkClick() {
    this.stepComponents[this.step].onOkClick();
  }

  onSummaryStep() {
    this.createAuctionCommand.execute(this.createAuctionArgs).subscribe((msg) => {
      if (msg.status === 'COMPLETED') {
        this.router.navigate(['/user']);
      } else {
        this.error = 'Cannot create an auction';
      }
    }, (err) => {
      this.error = 'Cannot create an auction';
    });
  }
}
