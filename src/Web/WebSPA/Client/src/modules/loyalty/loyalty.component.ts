import { Component, OnInit }    from '@angular/core';
import { LoyaltyService }        from './loyalty.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ILoyaltyDetails } from '../shared/models/loyaltyDetails.model';
import { IMemberTierDetails } from '../shared/models/memberTier.model';

@Component({
    selector: 'esh-loyalty',
    styleUrls: ['./loyalty.component.scss'],
    templateUrl: './loyalty.component.html'
})
export class LoyaltyComponent implements OnInit {
    errorReceived: boolean;
    loyaltyDetails: ILoyaltyDetails;
    memberTier: IMemberTierDetails;

    constructor(private service: LoyaltyService,
        private configurationService: ConfigurationService) { }

    ngOnInit() {
        if (this.configurationService.isReady) {
            this.getLoyaltyDetails();
        } else {
            this.configurationService.settingsLoaded$.subscribe(x => {
                this.getLoyaltyDetails();
            });
        }
    }

    getLoyaltyDetails() {
        this.errorReceived = false;
        this.service.getLoyaltyDetails()
            .pipe(catchError((err) => this.handleError(err)))
            .subscribe(loyaltyDetails => {
                this.loyaltyDetails = loyaltyDetails;
                console.log('loyalty details retrieved');
            });

        this.service.getMemberTier()
            .pipe(catchError((err) => this.handleError(err)))
            .subscribe(memberTier => {
                this.memberTier = memberTier;
                console.log('member tier details retrieved');
            });
    }

    private handleError(error: any) {
        this.errorReceived = true;
        return Observable.throw(error);
    }  
}

