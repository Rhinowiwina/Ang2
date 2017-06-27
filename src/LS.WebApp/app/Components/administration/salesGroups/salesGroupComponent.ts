import { Component, ViewChild, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, Inject, NgModule } from '@angular/core';
import { Routes, Router, RouterModule, ActivatedRoute } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser'
import { Observable } from 'rxjs/Rx';
import {SalesGroupDataService } from '../../../Service/Services';
import 'rxjs/add/operator/mergeMap';
import { Global,Constants } from '../../../Shared/global';
import { ModalModule, ModalDirective } from 'ngx-bootstrap';
import { ToasterModule, ToasterService, ToasterConfig, BodyOutputType } from 'angular2-toaster';
import { Accordion, AccordionGroup,AccordionHead } from '../../../Shared/accordion';

//https://www.npmjs.com/package/ng2-accordion
@Component({
	selector: 'sales-groups',
	templateUrl: 'app/Components/administration/salesGroups/salesGroups.html',

	styleUrls: ['../../Content/sass/siteAngular.css']
})
export class SalesGroupComponent implements OnInit {
    msg: string;
    hasLoaded: boolean = false;
    showLevel1Manager: [{}];
    showLevel2Manager: [{}];
    showLevel3Manager: [{}];
    showLevel3Team: [{}];
    unassingedManager: boolean = false;
    salesGroups: [{}];
    constructor(private router: Router, private _global: Global,private _salesGroupDataService: SalesGroupDataService,private toasterService: ToasterService, private _constants: Constants) {
	
	}

    ngOnInit(): void {
 
        this.getAndFormatSalesGroups();
    }
    getAndFormatSalesGroups() {
        this._salesGroupDataService.getCompanySalesGroupAdminTreeWhereManagerInTree().subscribe(response => {
            var response = response;
            if (!response.isSuccessful) {
                this.toasterService.pop('error', 'Error retrieving groups.', response.error.userHelp);
                this.hasLoaded = true;
            }
            this.salesGroups = response.data;
            if (this.salesGroups.length < 1) {
                this.unassingedManager = true;
            }
            this.hasLoaded = true;
               }, error => this.msg = <any>error);
    }

}