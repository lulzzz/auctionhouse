import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuctionsPageComponent } from './pages/auctions-page/auctions-page.component';
import { AuctionCreatePageComponent } from './pages/auction-create-page/auction-create-page.component';
import { AuthGuard } from '../core/auth/AuthGuard';

const routes: Routes = [
  { path: 'auctions/:mainCategory/:subCategory', component: AuctionsPageComponent },
  { path: 'auctions/:mainCategory/:subCategory/:subCategory2', component: AuctionsPageComponent },
  { path: 'auctions/create', canActivate: [AuthGuard], component: AuctionCreatePageComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuctionsRoutingModule { }
