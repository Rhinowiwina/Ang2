import { Component, ViewChild, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, Inject, NgModule } from '@angular/core';
import { Routes, Router, RouterModule, ActivatedRoute } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser'
import { Observable } from 'rxjs/Rx';
import { SalesGroupDataService } from '../../../Service/Services';
import 'rxjs/add/operator/mergeMap';
import { Global,Constants } from '../../../Shared/global';
import { ModalModule, ModalDirective } from 'ngx-bootstrap';
import { ToasterModule, ToasterService, ToasterConfig, BodyOutputType } from 'angular2-toaster';
import { Level1SalesGroup, Level2SalesGroup, Level3SalesGroup } from '../../../BindingModels/salesGroupBindingModels';
import { AccordionGroup } from '../../../Shared/Accordion/AccordionGroup';

import { SalesTeam } from '../../../BindingModels/salesTeamBindingModels';
//https://www.npmjs.com/package/ng2-accordion
@Component({
    selector: 'sales-groups',
    templateUrl: 'app/Components/administration/salesGroups/salesGroups.html',

    styleUrls: ['../../Content/sass/siteAngular.css']
})
export class SalesGroupComponent implements OnInit {
    msg: string;
    hasLoaded: boolean = false;
    showLevel1Manager: boolean[] = [];
    showLevel2Manager: boolean[]=[];
    showLevel3Manager: boolean[] = [];
    showLevel3Team: boolean[] = [];
    unassingedManager: boolean = false;
    salesGroups:Array<Level1SalesGroup>;
    @ViewChild(AccordionGroup) accordion: AccordionGroup;
    constructor(private router: Router, private _global: Global, private _salesGroupDataService: SalesGroupDataService, private toasterService: ToasterService, private _constants: Constants) {

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
   
    showManagers(index: number, groupId: string, level: number, event: any, isOpen: any) {
      
        if (isOpen && event) {
            event.preventDefault();
            event.stopPropagation();
        }
      
        if (level == this._constants.salesGroupLevel1) {
          
            this.showMgrsInLevel1Group(groupId);
            this.showLevel1Manager[index] = !this.showLevel1Manager[index];

        } else if (level == this._constants.salesGroupLevel2) {
            this.showMgrsInLevel2Group(groupId);
           this.showLevel2Manager[index] = !this.showLevel2Manager[index];

        } else if (level == this._constants.salesGroupLevel3) {
            this.showLevel3Manager[index] = this.showLevel3Manager[index];
        }

        return false;

    }
    showMgrsInLevel1Group(salesGroupLevel1Id: string) {
        if (typeof this.showLevel1Manager[salesGroupLevel1Id] == 'undefined') {
          
            this._salesGroupDataService.getGroupManagers(salesGroupLevel1Id, 1).subscribe(response => {
                var response = response;
                if (!response.isSuccessful) {
                    this.toasterService.pop('error', 'Error retrieving level1 managers.', response.error.userHelp);
                    this.hasLoaded = true;
                }
                this.salesGroups.forEach(function (Lev1Group: Level1SalesGroup) {
                        if (Lev1Group.id == salesGroupLevel1Id) {
                            Lev1Group.managers = response.data
                          
                        }
                   
                })
             
            }, error => this.msg = <any>error);
 
        }
        this.showLevel1Manager[salesGroupLevel1Id] = !this.showLevel1Manager[salesGroupLevel1Id];
     
    }
    showMgrsInLevel2Group(salesGroupLevel2Id: string) {
      
        if (typeof this.showLevel2Manager[salesGroupLevel2Id] == 'undefined') {
            this._salesGroupDataService.getGroupManagers(salesGroupLevel2Id, 2).subscribe(response => {
                var response = response;
                if (!response.isSuccessful) {
                    this.toasterService.pop('error', 'Error retrieving level2 managers.', response.error.userHelp);
                    this.hasLoaded = true;
                }
                this.salesGroups.forEach(function (Lev1Group: Level1SalesGroup) {
                    Lev1Group.childSalesGroups.forEach(function (Lev2Group: Level2SalesGroup) {
                        if (Lev2Group.id == salesGroupLevel2Id) {
                            Lev2Group.managers = response.data
                        
                        }
                    })
                })
            }, error => this.msg = <any>error);
        }
        this.showLevel2Manager[salesGroupLevel2Id] = !this.showLevel2Manager[salesGroupLevel2Id];
 
    }
    showMgrsInLevel3Group(salesGroupLevel3Id:string) {
         if (typeof this.showLevel2Manager[salesGroupLevel3Id] == 'undefined') {
        
            this._salesGroupDataService.getGroupManagers(salesGroupLevel3Id, 3).subscribe(response => {
                var response = response;
                if (!response.isSuccessful) {
                    this.toasterService.pop('error', 'Error retrieving level3 managers.', response.error.userHelp);
                    this.hasLoaded = true;
                }
                this.salesGroups.forEach(function (Lev1Group: Level1SalesGroup) {
                    Lev1Group.childSalesGroups.forEach(function (Lev2Group: Level2SalesGroup) {
                        Lev2Group.childSalesGroups.forEach(function (Lev3Group: Level3SalesGroup) {
                                if (Lev3Group.id == salesGroupLevel3Id) {
                                    Lev3Group.managers = response.data
                                }
                        })                       
                    })
                })
            }, error => this.msg = <any>error);
        }
         this.showLevel3Manager[salesGroupLevel3Id] = !this.showLevel3Manager[salesGroupLevel3Id];
    
    }
    showTeamsInLevel3Group(salesGroupLevel3Id:string){
        if (typeof this.showLevel3Team[salesGroupLevel3Id] == 'undefined') {
            this._salesGroupDataService.getLevel3GroupTeams(salesGroupLevel3Id).subscribe(response => {
                var response = response;
                if (!response.isSuccessful) {
                    this.toasterService.pop('error', 'Error retrieving sales teams.', response.error.userHelp);
                    this.hasLoaded = true;
                }
                this.salesGroups.forEach(function (Lev1Group: Level1SalesGroup) {
                    Lev1Group.childSalesGroups.forEach(function (Lev2Group: Level2SalesGroup) {
                        Lev2Group.childSalesGroups.forEach(function (Lev3Group: Level3SalesGroup) {
                            if (Lev3Group.id == salesGroupLevel3Id) {
                                Lev3Group.salesTeams = response.data
                            }
                        })
                    })
                })
            }, error => this.msg = <any>error); 
        }
        this.showLevel3Team[salesGroupLevel3Id] = !this.showLevel3Team[salesGroupLevel3Id];
    }
    modifySalesGroup(level: number, groupid: string, event: any) {
     
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }
        this.router.navigate(["modifySalesgroup"], {queryParams:{groupid:groupid,level:level}});
        return false;


    }
}