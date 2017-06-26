import { Component, ViewChild, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, Inject, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser'
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/mergeMap';
import { Global,Constants  } from '../../../Shared/global';
import { ModalModule, ModalDirective } from 'ngx-bootstrap';
import { ToasterModule, ToasterService, ToasterConfig, BodyOutputType } from 'angular2-toaster';
import { SalesTeam, DisplaySalesTeam } from '../../../BindingModels/salesTeamBindingModels';
import { ApplicationRole } from '../../../BindingModels/applicationRole';
import { AppUserDataService, SalesTeamDataService,SalesGroupDataService } from '../../../Service/Services';
import { YesNo, OrderBy } from '../../../Shared/filters';
import { Level1SalesGroup, Level2SalesGroup, Level3SalesGroup, GroupView} from '../../../BindingModels/groupBindingModels';
import { LoggedInUser, EditUserView, UserView } from '../../../BindingModels/userBindingModels';
//https://www.npmjs.com/package/ng2-accordion
@Component({
	selector: 'modify-user',
    templateUrl: 'app/Components/administration/users/modifyUsers.html',
    styleUrls: ['../../Content/sass/siteAngular.css'],
   
})
export class modifyUsersComponent implements OnInit {
    msg: string;
	hasLoaded: boolean = false;
	minToChangeTeam: number;
	private toasterService: ToasterService;
    roles: Array<ApplicationRole>=[];
   // groups: Array<{group}>=[]
    teams: Array<DisplaySalesTeam> = [];
    sendingpassword: boolean = false;
    groups: Array<GroupView>=[];
    managerLevel: string="";
    sub: any;
    userId:string;
    createOrModify: number;
    user: EditUserView;
    filteredRoles: Array<{}>;
    filteredGroups: any;
    originalEmail: string;
    canBeDeleted: boolean=false;
    showResetPasswordButton: boolean;
    testRole: Array<ApplicationRole> = [{ id:"f1c85c97-2c50-4165-9bb8-248518a109f2", name:"Level 1 Manager",rank:2}];
    constructor(private route: ActivatedRoute, private _userDataService: AppUserDataService, private _salesGroupDataService: SalesGroupDataService, private _salesTeamDataService: SalesTeamDataService, private _global: Global, toasterService: ToasterService, private _constants: Constants) {
        this.toasterService = toasterService;
		}

    ngOnInit(): void {

        this.minToChangeTeam = this._global.minToChangeTeam;
        this.sub = this.route.params.subscribe(params => {
            this.userId = params['userId']
            if (this.userId) {
                this.createOrModify = this._constants.modify
                this.getUserToEdit(this.userId);
            } else {
                this.createOrModify = this._constants.create
                //this.createUserModel();
            }
        })
		}
    getUserToEdit(userId: string) {
        this.user = new EditUserView();
        this.hasLoaded = false;
        this._userDataService.getUserToEdit(this.userId).subscribe(response =>{
            var response = response;
            if (!response.isSuccessful) {            
                this.toasterService.pop('error', 'Error getting user to edit.', response.errror.userHelp);
                this.hasLoaded = true;
            }
            let vuser = response.data;
           // console.log(vuser)
            this.originalEmail = vuser.email;
            this.user.id = vuser.id,
            this.user.comapnyId = vuser.companyId,
            this.user.userName = vuser.userName,
            this.user.originalEmail = vuser.email,
            this.user.firstName = vuser.firstName,
            this.user.lastName = vuser.lastName,
            this.user.externalUserID = vuser.externalUserID,
            this.user.isExternalUserIDActive = vuser.isExternalUserIDActive,
            this.user.email = vuser.email,
            this.user.payPalEmail = vuser.payPalEmail,
            this.user.originalActive = vuser.isActive,
            this.user.isActive = vuser.isActive,
            this.user.additionalDataNeeded = vuser.additionalDataNeeded,
            this.user.permissionsAllowTpivBypass = vuser.permissionsAllowTpivBypass,
            this.user.permissionsLifelineNlad = vuser.permissionsLifelineNlad,
            this.user.permissionsLifelineCA = vuser.permissionsLifelineCA,
            this.user.permissionsLifelineTX = vuser.permissionsLifelineTX,
            this.user.permissionsAccountOrder = vuser.permissionsAccountOrder,
            this.user.userCommission = vuser.userCommission,
            this.user.role = vuser.role;
            this.user.originalRoleName= vuser.role.name,
            this.user.team = vuser.salesTeam;
            this.user.rowVersion= vuser.rowVersion
         
            this.hasLoaded = true;
            if (vuser.role.rank <= this._global.loggedInUser.role.rank) {
                this.toasterService.pop('error', 'Permission Error', "You do not have permission to access this user's information.");
                return;
            }
            
       console.log(this.user)
         
           
            this.canBeDeleted = vuser.canBeDeleted && this._global.loggedInUser.role.rank <= this._constants.administratorRoleRank;
       
            this.showResetPasswordButton = true;
            this.getTeams();
            this.setRoleFilter();  
                }, error => this.msg = <any>error);
    }

    getTeams() {      
        this._salesTeamDataService.getSalesTeamsForSelection().subscribe
            (response => {
                
                var response = response;
                if (!response.isSuccessful) {
                    this.toasterService.pop('error', 'Error Sales Teams', response.errror.userHelp);
                    return
                }
                
                var vTeam = response.data;
                let editedTeam: Array<DisplaySalesTeam> = [];
                vTeam.forEach(function (team: DisplaySalesTeam) {
                 
                   editedTeam.push({
                        id: team.id,
                        name: team.name,
                        externalDisplayName: team.externalDisplayName,
                        isActive: team.isActive,
                        displayText:""
                  
                 })
                 
                });
             this.teams = editedTeam;
               
            }, error => this.msg = <any>error)

        this._salesGroupDataService.getCompanySalesGroupAdminTreeWhereManagerInTree().subscribe
            (response => {
                var response = response;
                if (!response.isSuccessful) {
                    console.log(response)
                    this.toasterService.pop('error', 'Error Groups', response.error.userHelp);
                    return
                }
             
                let self = this
                 var  lev1Groups = response.data;
                lev1Groups.forEach(function (lev1group: Level1SalesGroup) {
                    self.groups.push({
                        name: lev1group.name,
                        id: lev1group.id,
                        level: "1"
                    })
                 
                    var lev2groups = lev1group.childSalesGroups;
                       
                            lev2groups.forEach(function (lev2: Level2SalesGroup){
                                self.groups.push({
                                    name: lev2.name,
                                    id: lev2.id,
                                    level: "2"
                                })
                                var lev3groups = lev2.childSalesGroups;
                        lev3groups.forEach(function (lev3: Level3SalesGroup) {
                            self.groups.push({
                                name: lev3.name,
                                id: lev3.id,
                                level: "3"
                            })
                        })
                    })
                 })

                var teamsLength = this.teams.length;
                for (var teamIndex = 0; teamIndex < teamsLength; teamIndex++) {
                    var team = this.teams[teamIndex];
                    team.displayText = team.externalDisplayName + " / " + team.name;
                    if (!team.isActive) {
                        team.displayText += " (Inactive)";
                    }
                }
                console.log(this.user)
                  this.setTeam()   
            }, error => this.msg = <any>error)
    }
    setTeam() {
        
       
        var teamsLength = this.teams.length;
        for (var teamsIndex = 0; teamsIndex < teamsLength; teamsIndex++) {
            
            if (this.user.team.id == this.teams[teamsIndex].id) {
                this.user.team = this.teams[teamsIndex];
            }
        }

    }
    setRoleFilter() {
        this._userDataService.getAllRoles().subscribe
            (response => {
                var response = response;
                if (!response.isSuccessful) {
                    this.toasterService.pop('error', 'Error Getting Roles', response.errror.userHelp);
                    return
                }
                var self = this;
                var vroles = response.data;
                vroles.forEach(function (role:any) {
                 self.roles.push({id:role.id,name:role.name,rank:role.rank})

                });

             console.log(this.roles)
                //filter roles the user does not have access to.
               this.roles = this.roles.filter(role => role.rank > this._global.loggedInUser.role.rank)
               
                this.hasLoaded = true;
            }, error => this.msg = <any>error)
    }
    setGroupFilter() {
        let vgroupLevel: string;
        if (this.user.role.name == "Level 1 Manager") {
            this.managerLevel = "Level1"
            vgroupLevel = "1"
        }
        else if (this.user.role.name == "Level 2 Manager") {
            this.managerLevel = "Level2"
            vgroupLevel = "2"
        }
        else if (this.user.role.name == 'Level 3 Manager') {
            this.managerLevel = "Level3"
            vgroupLevel = "3"
        }
        else {
            this.managerLevel = ""
        }
        var vGroups = this.groups;
        let filteredGroup: GroupView[] = []
        vGroups.forEach(function (group: GroupView) {
            if (group.level == vgroupLevel) {
                filteredGroup.push(group)
            }

        })
        this.groups = filteredGroup;

    }
    setRole() {

    }

}