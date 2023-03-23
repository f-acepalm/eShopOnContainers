import { NgModule }             from '@angular/core';
import { BrowserModule  }       from '@angular/platform-browser';

import { SharedModule }         from '../shared/shared.module';
import { LoyaltyComponent } from './loyalty.component';
import { LoyaltyService } from './loyalty.service';

@NgModule({
    imports: [BrowserModule, SharedModule],
    declarations: [LoyaltyComponent],
    providers: [LoyaltyService]
})
export class LoyaltyModule { }
