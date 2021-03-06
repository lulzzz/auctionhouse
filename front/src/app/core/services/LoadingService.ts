import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private loadingCount = 0;
  isLoading = false;

  constructor() { }

  setLoading(loading: boolean) {
    if (!loading) {
      setTimeout(() => {
        this.isLoading = !(--this.loadingCount <= 0);
        if (this.loadingCount < 0) { this.loadingCount = 0; }
      });
    } else {
      setTimeout(() => this.isLoading = ++this.loadingCount > 0);
    }
  }

  resetLoading(){
    this.isLoading = false;
    this.loadingCount = 0;
  }
}
