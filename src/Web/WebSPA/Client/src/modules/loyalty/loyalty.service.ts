import { Injectable } from '@angular/core';

import { DataService } from '../shared/services/data.service';
import { IOrderDetail } from "../shared/models/order-detail.model";
import { SecurityService } from '../shared/services/security.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { BasketWrapperService } from '../shared/services/basket.wrapper.service';

import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ICoupon } from '../shared/models/coupon.model';
import { ILoyaltyDetails } from '../shared/models/loyaltyDetails.model';
import { IMemberTierDetails } from '../shared/models/memberTier.model';

@Injectable()
export class LoyaltyService {
    private loyaltyUrl: string = '';

    constructor(private service: DataService, private configurationService: ConfigurationService) {
        if (this.configurationService.isReady)
            this.loyaltyUrl = this.configurationService.serverSettings.purchaseUrl;
        else
            this.configurationService.settingsLoaded$.subscribe(x => this.loyaltyUrl = this.configurationService.serverSettings.purchaseUrl);
    }

    getLoyaltyDetails(): Observable<ILoyaltyDetails> {
        let url = this.loyaltyUrl + '/l/api/v1/loyalty';

        return this.service.get(url).pipe<ILoyaltyDetails>(tap((response: any) => {
            return response;
        }));
    }

    getMemberTier(): Observable<IMemberTierDetails> {
        let url = this.loyaltyUrl + '/l/api/v1/MemberTier/current';

        return this.service.get(url).pipe<IMemberTierDetails>(tap((response: any) => {
            return response;
        }));
    }
}

