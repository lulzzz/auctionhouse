import { Component, OnInit, Input } from '@angular/core';
import { CategoryTreeNode } from 'src/app/core/models/CategoryTreeNode';
import { CategoriesQuery } from 'src/app/core/queries/CategoriesQuery';
import { CategorySelectStep } from './categorySelectStep';
import { AuctionCreateStep } from '../auctionCreateStep';
import { Category } from '../../../core/models/Category';



@Component({
  selector: 'app-category-select-step',
  templateUrl: './category-select-step.component.html',
  styleUrls: ['./category-select-step.component.scss']
})
export class CategorySelectStepComponent extends AuctionCreateStep<CategorySelectStep> implements OnInit {

  @Input()
  defaults: Category;

  mainCategories: CategoryTreeNode[] = [];
  subCategories: CategoryTreeNode[] = [];
  subCategories2: CategoryTreeNode[] = [];
  selectedMainCategory: CategoryTreeNode;
  selectedSubCategory: CategoryTreeNode;
  selectedSubCategory2: CategoryTreeNode;

  titleMsg = 'Select categories';

  constructor(private categoriesQuery: CategoriesQuery) {
    super();
    this.categoriesQuery.execute().subscribe((v) => {
      this.mainCategories = v.subCategories;
      if (this.defaults) {
        this.selectMainCategory(this.defaults.name);
        this.selectSubCategory(this.defaults.subCategory.name);
        this.selectSubCategory2(this.defaults.subCategory.subCategory.name);
      }
    });
  }

  ngOnInit() {
  }

  selectMainCategory(selectedCategoryName: string) {
    this.selectedMainCategory =
      this.mainCategories.filter(c => c.categoryName === selectedCategoryName)[0] || null;
    this.subCategories = this.selectedMainCategory ? this.selectedMainCategory.subCategories : null;
    this.selectedSubCategory = null;
    this.selectedSubCategory2 = null;
    this.subCategories2 = [];
    this.ready = false;
  }

  selectSubCategory(selectedCategoryName: string) {
    this.selectedSubCategory
      = this.subCategories.filter(c => c.categoryName === selectedCategoryName)[0] || null;
    this.subCategories2 = this.selectedSubCategory ? this.selectedSubCategory.subCategories : null;
    this.ready = false;
  }

  selectSubCategory2(selectedCategoryName: string) {
    if (!selectedCategoryName) {
      this.ready = false;
      return;
    }
    this.selectedSubCategory2 = this.subCategories2.filter(c => c.categoryName === selectedCategoryName)[0];
    this.ready = true;
  }

  onOkClick() {
    console.log("OK click");

    if (this.selectedMainCategory && this.selectedSubCategory && this.selectedSubCategory2) {
      let step: CategorySelectStep = {
        selectedMainCategory: this.selectedMainCategory,
        selectedSubCategory: this.selectedSubCategory,
        selectedSubCategory2: this.selectedSubCategory2
      };
      this.completeStep(step);
    }
  }

}
