import { Component, ViewChild, ViewEncapsulation, Input, Attribute, OnChanges, OnInit, Inject, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser'
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe, Location } from '@angular/common';
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
    sendingPassword: boolean = false;
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
      
    constructor(private router: Router,private _location: Location,private route: ActivatedRoute, private _userDataService: AppUserDataService, private _salesGroupDataService: SalesGroupDataService, private _salesTeamDataService: SalesTeamDataService, private _global: Global, toasterService: ToasterService, private _constants: Constants) {
        this.toasterService = toasterService;
     
		}

    ngOnInit(): void {
     
        console.log(this._global)
        this.minToChangeTeam = this._global.minToChangeTeam;
        this.sub = this.route.params.subscribe(params => {
            this.userId = params['userId']
            if (this.userId) {
                this.createOrModify = this._constants.modify
                this.getUserToEdit(this.userId);
            } else {
                this.createOrModify = this._constants.create
                this.createUser();
            }
        })
    }
    createUser() {
        this.user = new EditUserView();
        this.hasLoaded = false;
        this.user.id="";
        this.user.companyId= "";
        this.user.userName= "";
        this.user.originalEmail= "";
        this.user.firstName= "";
        this.user.lastName= "";
        this.user.externalUserID= "";
        this.user.isExternalUserIDActive=false;
        this.user.email= "";
        this.user.payPalEmail= "";
        this.user.isActive= true;
        this.user.originalActive= true;
        this.user.additionalDataNeeded= true;
        this.user.permissionsAllowTpivBypass= true;
        this.user.permissionsLifelineNlad= true;
        this.user.permissionsLifelineCA= true;
        this.user.permissionsLifelineTX= true;
        this.user.permissionsAccountOrder= true;
        this.user.userCommission=0;
        this.user.originalRoleName = "";
        this.user.team = {id:"",name:""};
         this.user.rowVersion= "";
         this.user.roleId = "";
         this.user.role={id:null,name:null,rank:null}
         this.getTeams();
        this.setRoleFilter(); 
        this.hasLoaded = true;
         }
    getUserToEdit(userId:string) {
        this.user = new EditUserView();
        this.hasLoaded = false;
        this._userDataService.getUserToEdit(this.userId).subscribe(response =>{
            var response = response;
            if (!response.isSuccessful) {            
                this.toasterService.pop('error', 'Error getting user to edit.', response.errror.userHelp);
                this.hasLoaded = true;
            }
            let vuser = response.data;
     
            this.user.id = vuser.id;
            this.user.companyId = vuser.companyId;
            this.user.userName = vuser.userName;
            this.user.originalEmail = vuser.email;
            this.user.firstName = vuser.firstName;
            this.user.lastName = vuser.lastName;
            this.user.externalUserID = vuser.externalUserID;
            this.user.isExternalUserIDActive = vuser.isExternalUserIDActive;
            this.user.email = vuser.email;
            this.user.payPalEmail = vuser.payPalEmail;
            this.user.originalActive = vuser.isActive;
            this.user.isActive = vuser.isActive;
            this.user.additionalDataNeeded = vuser.additionalDataNeeded;
            this.user.permissionsAllowTpivBypass = vuser.permissionsAllowTpivBypass;
            this.user.permissionsLifelineNlad = vuser.permissionsLifelineNlad;
            this.user.permissionsLifelineCA = vuser.permissionsLifelineCA;
            this.user.permissionsLifelineTX = vuser.permissionsLifelineTX;
            this.user.permissionsAccountOrder = vuser.permissionsAccountOrder;
            this.user.userCommission = vuser.userCommission;
            this.user.role = vuser.role;
            this.user.originalRoleName= vuser.role.name;
            this.user.team = vuser.salesTeam;
            this.user.team.id = vuser.salesTeam.id == null ? "" : vuser.salesTeam.id;
            this.user.rowVersion= vuser.rowVersion;
            this.user.selectedGroupId="";
            this.hasLoaded = true;
         
            if (vuser.role.rank <= this._global.loggedInUser.role.rank) {
                this.toasterService.pop('error', 'Permission Error', "You do not have permission to access this user's information.");
                return;
            }
            
       
         
           
            this.canBeDeleted = vuser.canBeDeleted && this._global.loggedInUser.role.rank <= this._constants.administratorRoleRank;
          
            this.showResetPasswordButton = true;
            this.getTeams();
            this.setRoleFilter();  
                }, error => this.msg = <any>error);
    }
    deleteUser(userId: string) {
        this._userDataService.deleteUser(userId).subscribe(response => {
            var response = response;
            if (!response.isSuccessful) {
                this.toasterService.pop('error', 'Error deleting user.', response.error.userHelp);
                this.sendingPassword = false;
            } 
            this.toasterService.pop('success', 'Successfully deleted user.');
            this.router.navigate(["user"])
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
                editedTeam.push({id:"",name:"",externalDisplayName:"",isActive:null,displayText: "-Select Team-"})
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
                var lev1Groups = response.data;
                
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
                    if (team.displayText == "-Select Team-") {
                        continue
                    }
                    team.displayText = team.externalDisplayName + " / " + team.name;
                    if (!team.isActive) {
                        team.displayText += " (Inactive)";
                    }
                }
              
                  this.setTeam()   
            }, error => this.msg = <any>error)
    }
    setTeam() {
        
       
        var teamsLength = this.teams.length;
        for (var teamsIndex = 0; teamsIndex < teamsLength; teamsIndex++) {
            
            if (this.user.team.id == this.teams[teamsIndex].id) {

                this.user.team.id = this.teams[teamsIndex].id;
                this.user.team.name = this.teams[teamsIndex].name;
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

             //console.log(this.roles)
                //filter roles the user does not have access to.
               this.roles = this.roles.filter(role => role.rank > this._global.loggedInUser.role.rank)
               
                this.hasLoaded = true;
            }, error => this.msg = <any>error)
    }
    setGroupFilter() {
        let vgroupLevel: string;
        this.setRole();
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
      filteredGroup.push({
            name: '-Select Group-',
            id: "",
            level: ""
        })
        vGroups.forEach(function (group: GroupView) {
            if (group.level == vgroupLevel) {
                filteredGroup.push(group)
            }

        })
        this.groups = filteredGroup;
        console.log(this._global)

    }
    setRole() {
        var rolesLength = this.roles.length;
    
        for (var rolesIndex = 0; rolesIndex < rolesLength; rolesIndex++){
          
            if (this.user.role.id == this.roles[rolesIndex].id) {
              
                this.user.role.id = this.roles[rolesIndex].id;
                this.user.role.name = this.roles[rolesIndex].name;
                this.user.role.rank = this.roles[rolesIndex].rank;

                break;
            }

        }
       
    }
    resetUsersPassword(userId: string, email: string) {
        this.sendingPassword = true;
        this._userDataService.resetUsersPassword(userId,email).subscribe(response => {
            var response = response;
            if (!response.isSuccessful) {
                this.toasterService.pop('error', 'Error resetting password.', response.error.userHelp);
                this.sendingPassword = false;
            } else if (response.isSuccessful) {
                if (response.error == null) {
                    this.toasterService.pop('success', "An email has been sent to the user's account.");
                    this.sendingPassword = false;
                }else {
                this.toasterService.pop('info',response.error.userHelp);
                this.sendingPassword = false;
            }
            } 

          
        }, error => this.msg = <any>error);

    }
    submitUser( userForm: any) {  
    
        if (userForm.valid) {
            if (this.createOrModify == this._constants.modify && this._global.loggedInUser.id == this.user.id) {
                this._userDataService.editLoggedInUser(this.user).subscribe(response => {
              
                    var response = response;
                    if (!response.isSuccessful) {
                        this.toasterService.pop('error', 'Error editing logged in user.', response.error.userHelp);
                        this.sendingPassword = false;
                    }
                    this.toasterService.pop('success', 'Successfully edited logged in user.');
                    this.router.navigate(["user"])
                }, error => this.msg = <any>error);


            } else {
                
                if (this.user.selectedGroupId ==null || this.user.selectedGroupId == "") {
                    var groupId = "0";
                } else {
                    var groupId = this.user.selectedGroupId;
                }
             
                this._userDataService.submitUserForAddOrEdit(this.user, this.createOrModify, groupId).subscribe(response => {
                 
                    var response = response;
                    if (!response.isSuccessful) {
                        this.toasterService.pop('error', 'Error editing user.', response.error.userHelp);
                        this.sendingPassword = false;
                    }
                    this.toasterService.pop('success', 'Successfully edited user.');
                    this.router.navigate(["user"])
                }, error => this.msg = <any>error);

            }


        }

    }
}