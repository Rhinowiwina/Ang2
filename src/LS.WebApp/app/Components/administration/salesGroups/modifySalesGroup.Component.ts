import { Component, ViewChild, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, Inject, NgModule } from '@angular/core';
import { Routes, Router, RouterModule, ActivatedRoute } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser'
import { Observable } from 'rxjs/Rx';
import { SalesGroupDataService } from '../../../Service/Services';
import 'rxjs/add/operator/mergeMap';
import { Global, Constants } from '../../../Shared/global';

import { ModalModule, ModalDirective } from 'ngx-bootstrap';
import { ToasterModule, ToasterService, ToasterConfig, BodyOutputType } from 'angular2-toaster';
import { Level1SalesGroup, Level2SalesGroup, Level3SalesGroup, SalesGroup } from '../../../BindingModels/salesGroupBindingModels';
import { SalesTeam } from '../../../BindingModels/salesTeamBindingModels';
import { UserView, GroupManager, GroupsOfManagers } from '../../../BindingModels/userBindingModels';
import { AppUserDataService } from '../../../Service/Services';
//https://www.npmjs.com/package/ng2-accordion
@Component({
    selector: 'modifySales-groups',
    templateUrl: 'app/Components/administration/salesGroups/modifySalesGroups.html',

    styleUrls: ['../../Content/sass/siteAngular.css']
})
export class ModifySalesGroupComponent implements OnInit {
    msg: string;
    hasLoaded: boolean = false;
    sub: any;
    salesGroupId: string;
    level1SalesGroups: Array<Level1SalesGroup>;
    salesGroup: { parentGroupOptions: Array<SalesGroup>, level: number, parentSalesGroupLabel: string, managerOptions: GroupManager[], managers: Array<GroupManager>,isDeleted:boolean };
    allSalesGroups: Array<SalesGroup>=[];
    canBeDeleted: boolean;
    allManagers: GroupsOfManagers;
    createOrModify: number;
    paramLevel: number;
    currentLevel: number;
    submitButtonDisabled: boolean;
    constructor(private router: Router, private _appUserDataService:AppUserDataService, private route: ActivatedRoute, private _global: Global, private _salesGroupDataService: SalesGroupDataService, private toasterService: ToasterService, private _constants: Constants) {

    }

    ngOnInit(): void {
        this.allManagers = new GroupsOfManagers();
      
        this.sub = this.route.queryParams.subscribe(params => {
        
            this.salesGroupId = params['groupid'];
            this.paramLevel = params['level']
            this.getManagers()
            if (this.salesGroupId) {
             
                this.createOrModify = this._constants.modify
                this.currentLevel = this.paramLevel;
                this.getSalesGroupToEdit(this.salesGroupId);
            } else {
                this.createOrModify = this._constants.create
                this.salesGroupCreate();
            }
        })
       
    }
    ngOnDestroy() {
        if (this.sub) this.sub.unsubscribe();
    }
    getSalesGroups(){
        let self = this;
     
        this._salesGroupDataService.getCompanySalesGroupAdminTreeWhereManagerInTree().subscribe(response => {
            var response = response;
            if (!response.isSuccessful) {
                this.toasterService.pop('error', 'Error retrieving groups.', response.error.userHelp);
                this.hasLoaded = true;
            }
            this.level1SalesGroups = response.data;
            this.level1SalesGroups.forEach(function (level1SalesGroup) {
        
                self.allSalesGroups.push({ id: level1SalesGroup.id, name: level1SalesGroup.name, level: 1, parentGroupName: '' });
                if (level1SalesGroup.childSalesGroups.length > 0){
                    var level2SalesGroups = level1SalesGroup.childSalesGroups;
                    level2SalesGroups.forEach(function (level2SalesGroup) {
                        self.allSalesGroups.push({ id: level2SalesGroup.id, name: level2SalesGroup.name, level: 2, parentGroupName: level1SalesGroup.name });
                    })
                }
             
       
                self.salesGroup.parentGroupOptions = self.allSalesGroups.filter((group: { level: number }) => group.level == self.currentLevel)
            })
         
       
        }, error => this.msg = <any>error);
       
    }
    getManagers() {
        let self = this;
        this._appUserDataService.getAllSalesGroupManagers().subscribe(response => {
            var response = response;
            if (!response.isSuccessful) {
                this.toasterService.pop('error', 'Error retrieving managers.', response.error.userHelp);
                this.hasLoaded = true;
            }
         
            this.allManagers.level1 = [];
            self.allManagers.level2 = [];
            self.allManagers.level3 = [];
           self.allManagers.level1 = response.data.level1Managers;
           self.allManagers.level2 = response.data.level1Managers;
           self.allManagers.level3 = response.data.level1Managers;
         
         
        }, error => this.msg = <any>error);

    }
    getSalesGroupToEdit(saleGroupId: string) {
        var self = this;
        this._salesGroupDataService.getSalesGroup(this.salesGroupId, this.paramLevel).subscribe(response => {
            var response = response;
            if (!response.isSuccessful) {
                this.toasterService.pop('error', 'Error retrieving group.', response.error.userHelp);
                this.hasLoaded = true;
            }         
            var salesGroup = response.data
             this.checkDeletability(salesGroup);
            this.salesGroup = salesGroup;
            this.salesGroup.level = this.paramLevel;
            this.salesGroup.parentSalesGroupLabel = "Level " + (this.salesGroup.level - 1);
            this.getSalesGroups()
            if (this.currentLevel == 1) {
                this.salesGroup.managerOptions = this.allManagers.level1;
            } else if (this.currentLevel == 2){
                this.salesGroup.managerOptions = this.allManagers.level2;
        } else if (this.currentLevel == 3){
                this.salesGroup.managerOptions = this.allManagers.level3;
            }
               
            console.log(this.salesGroup)
            this.hasLoaded = true;
        }, error => this.msg = <any>error);
    }
 

    salesGroupCreate() {


    }
    checkDeletability(salesGroup: any) {
        if (this.currentLevel == this._constants.salesGroupLevel3) {
            this.canBeDeleted = salesGroup.salesTeams.length == 0 && salesGroup.managers.length == 0;
            return;
        }

        this.canBeDeleted = salesGroup.childSalesGroups.length == 0 && salesGroup.managers.length == 0;

    }

    submitSalesGroup(groupForm: any, createOrModify: number, salesGroupLevel: number) {
        this.submitButtonDisabled = true;


        this.salesGroup.managers = this.checkForManagers();
        this.salesGroup.isDeleted = false;


        if (groupForm.valid) {
            this._appUserDataService.submitSalesGroupForAddOrEdit().subscribe(response => {
                var response = response;
                if (!response.isSuccessful) {
                    this.toasterService.pop('error', 'Error saving group.', response.error.userHelp);
                    this.hasLoaded = true;
                }





            }, error => this.msg = <any>error);
        }
    }
   checkForManagers() {
       var selectedManagers: Array<GroupManager> = []
       this.salesGroup.managerOptions.forEach(function (manager) {
           if (manager.val = true) {
            selectedManagers.push(manager);
           }
       });
       return selectedManagers;
   }
}