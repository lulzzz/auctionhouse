import { HttpClient } from '@angular/common/http';
import { UserIdentity } from '../../models/UserIdentity';
import { Observable } from 'rxjs';
import * as jwtDecode from 'jwt-decode';
import { map, filter } from 'rxjs/operators';
import { WSCommandStatusService, RequestStatus } from '../../services/WSCommandStatusService';
import { Injectable } from '@angular/core';
import { WSCommandHelper } from '../WSCommandHelper';
import { CommandHelper, ResponseOptions } from '../ComandHelper';

@Injectable({
  providedIn: 'root'
})
export class CancelBidCommand{
  constructor(private httpClient: HttpClient, private commandHelper: CommandHelper) {
  }

  execute(auctionId: string, bidId: string): Observable<RequestStatus> {
    const url = '/api/cancelBid';
    const req = this.httpClient.post(url, { auctionId, bidId });
    return this.commandHelper.getResponseStatusHandler(req, true, ResponseOptions.WSQueuedCommand);
  }
}