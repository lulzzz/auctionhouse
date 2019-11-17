import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl, ValidatorFn, ValidationErrors, AbstractControl, FormGroupDirective, NgForm } from '@angular/forms';
import { Product, Condition } from 'src/app/core/models/Product';
import { Auction } from '../../../../../core/models/Auctions';
import { AuctionCreateStep } from '../../../../auctionCreateStep';
import { ProductStep } from 'src/app/auctions/productStep';
import { ErrorStateMatcher } from '@angular/material';


function tagsValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const tagsString: string = control.value;
    const tags = tagsString.split(' ').filter((s) => s.length > 0);
    console.log(tags);

    if (tags.length === 0) { return null; }
    if (tags.filter((t) => t.length > 30).length > 0) { return { tagsMaxLength: { value: 'Tag value exceeds max length' } }; }

    return null;
  };

}

export class InstantErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null,
    form: FormGroupDirective | NgForm | null): boolean {
    return control && control.invalid && (control.dirty || control.touched);
  }
}

@Component({
  selector: 'app-product-step',
  templateUrl: './product-step.component.html',
  styleUrls: ['./product-step.component.scss']
})
export class ProductStepComponent extends AuctionCreateStep<ProductStep> implements OnInit {



  titleMsg = 'Product info';

  defaultStartDate: Date;
  defaultEndDate: Date;
  form = new FormGroup({
    productName: new FormControl('', [Validators.required]),
    productDescription: new FormControl('', [Validators.required]),
    tags: new FormControl('', [Validators.required, tagsValidator()]),
    condition: new FormControl(0, [Validators.required]),
  });
  tags: string[] = [];
  tagsErrorStateMatcher = new InstantErrorStateMatcher();
  showTagsHelp = false;

  constructor() {
    super();
    this.defaultStartDate = new Date();
    const nextMonth = new Date();
    nextMonth.setMonth(nextMonth.getMonth() + 1);
    this.defaultEndDate = nextMonth;
    console.log(this.form);
  }

  ngOnInit() {
  }

  onTagsChange() {
    if (this.form.controls.tags.valid) {
      this.tags = this.form.value.tags.split(' ').filter((s) => s.length > 0);
    } else {
      this.tags = [];
    }
    this.showTagsHelp = this.form.value.tags.split(' ').filter((s) => s.length === 0).length > 1;
  }

  onChange() {
    this.ready = this.form.valid;
    console.log(this.form.value);

  }


  onOkClick() {
    console.log(this.form.value);
    console.log(this.form.valid);
    console.log(this.form.errors);


    if (this.form.valid) {
      const product: Product = {
        name: this.form.value.productName,
        description: this.form.value.productDescription,
        condition: this.form.value.condition
      };
      const productStep = new ProductStep(product, this.tags);
      this.completeStep(productStep);
    }
  }


}


