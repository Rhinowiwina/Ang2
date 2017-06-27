import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import { LoggedInUser, EditUserView, UserView } from '../BindingModels/userBindingModels';
import { message } from '../BindingModels/messageBindingModels';
import { Constants, Global } from '../Shared/global'
@Injectable()
export class BaseService {
    baseUrl: string
    constructor(private _http: Http, private _constants:Constants) {
	//_http is sent from inherited class
	}
	
	//base calls
	get(url: string): Observable<any> {
	
		return this._http.get(url)
	.map((response: Response) => <any>response.json())
	//.do(data => console.log("All: " + JSON.stringify(data)))
	.catch(this.handleError);
	}

    post(url: string, model: any): Observable<any> {
  
		let body = JSON.stringify(model);
		let headers = new Headers({ 'Content-Type': 'application/json' });
		let options = new RequestOptions({ headers: headers });
		return this._http.post(url, body, options)
			.map((response: Response) => <any>response.json())
			.catch(this.handleError);
	}

	put(url: string, id: number, model: any): Observable<any> {
		let body = JSON.stringify(model);
		let headers = new Headers({ 'Content-Type': 'application/json' });
		let options = new RequestOptions({ headers: headers });
		return this._http.put(url + id, body, options)
			.map((response: Response) => <any>response.json())
			.catch(this.handleError);
	}

	delete(url: string): Observable<any> {
		let headers = new Headers({ 'Content-Type': 'application/json' });
		let options = new RequestOptions({ headers: headers });
		return this._http.delete(url, options)
			.map((response: Response) => <any>response.json())
			.catch(this.handleError);
	}

	private handleError(error: Response) {
		
		return Observable.throw(error.json().error || 'Server error');
	}



}
@Injectable()
export class CompanyDataService extends BaseService {
	baseUrl: string="api/company/";

    constructor(private vhttp: Http, private constants: Constants) {
        super(vhttp, constants);
    }
  getCompany(companyId: string) {
	 
	  return super.get(this.baseUrl+"getCompany?companyId="+companyId)

	}
}
@Injectable()
export class AppUserDataService extends BaseService{
	baseUrl: string = 'api/appUser/';
	
    constructor(private vhttp: Http, private constants: Constants, private global:Global) {
        super(vhttp, constants);
    }
    resetUsersPassword(userId:string,email:string) {
        return this.post(this.baseUrl + 'resetUsersPassword?userId='+userId +'&email=' + email,null);

    }
    editLoggedInUser(user:EditUserView) {
        var putData = {
            userName: user.userName,
            firstName: user.firstName,
            lastName: user.lastName,
            email: user.email,
            rowVersion: user.rowVersion
        }
        return this.post(this.baseUrl + 'editLoggedInUser',putData);

    }
    submitUserForAddOrEdit(user: EditUserView, createOrModify: number, groupId: string) {
     
        if (createOrModify == this.constants.modify) {
         
           var result = this.editUser(user, groupId);
        }
        if (createOrModify == this.constants.create) {
            var result = this.addUser(user, groupId);
        }
       return result;
    }
    editUser(user: EditUserView, groupId: string) {
      
        var postData = {
            userId: user.id,
            groupId: groupId,
            companyId: user.companyId,
            userName: user.userName,
            roleId: user.role.id,
            firstName: user.firstName,
            lastName: user.lastName,
            externalUserID: user.externalUserID,
            externalUserIDActive: user.isExternalUserIDActive,
            email: user.email,
            originalEmail: user.originalEmail,
            paypalemail: user.payPalEmail,
            isActive: user.isActive,
            additionalDataNeeded: user.additionalDataNeeded,
            permissionsAllowTpivBypass: user.permissionsAllowTpivBypass,
            permissionsLifelineNlad: user.permissionsLifelineNlad,
            permissionsLifelineCA: user.permissionsLifelineCA,
            permissionsLifelineTX: user.permissionsLifelineTX,
            permissionsAccountOrder: user.permissionsAccountOrder,
            salesTeamId:"",
            rowVersion:user.rowVersion
        }
        if (user.role.rank >= this.constants.salesTeamManagerRoleRank && user.team != null) {
            postData.salesTeamId = user.team.id;
        } else { postData.salesTeamId=null }
       
        
        return this.post(this.baseUrl + 'editUser', postData);
        }
    addUser(user: EditUserView, groupId: string) {
        var postData = {
            roleId: user.role.id,
            groupId: groupId,
            firstName: user.firstName,
            lastName: user.lastName,
            externalUserID: user.externalUserID,
            externalUserIDActive: user.isExternalUserIDActive,
            email: user.email,
            paypalemail: user.payPalEmail,
            permissionsAllowTpivBypass: user.permissionsAllowTpivBypass || false,
            permissionsLifelineNlad: user.permissionsLifelineNlad || false,
            permissionsLifelineCA: user.permissionsLifelineCA || false,
            permissionsLifelineTX: user.permissionsLifelineTX || false,
            permissionsAccountOrder: user.permissionsAccountOrder || false,
            isActive: user.isActive,
            salesTeamId: "",
            additionalDataNeeded: user.additionalDataNeeded
        }
        if ((user.role.rank == this.constants.salesRepRoleRank || user.role.rank == this.constants.salesTeamManagerRoleRank) && typeof user.team != "undefined") {
            postData.salesTeamId = user.team.id;
        }
        return this.post(this.baseUrl + 'createUser', postData);
    }
    deleteUser(userId:string) {
        return this.delete(this.baseUrl +'?userId=' + userId);

    }
	getLoggedInUser () {
		
		return this.get(this.baseUrl + 'getLoggedInUser');

	}
	getAllUsersUnderLoggedInUserInTree(UserId: string, rank: number) {

	return this.get(this.baseUrl + 'getAllUsersUnderLoggedInUserInTree?UserID=' + UserId + '&Rank=' + rank );

	}
	getAllRoles() {
		return this.get(this.baseUrl + "getAllRoles");
    }
    getUserToEdit(userId: string) {
      
        return this.get(this.baseUrl + 'getUserForEdit?userId='+ userId);
    }
}
@Injectable()
export class MessageDataService extends BaseService {
	baseUrl: string = 'api/loginMsg/'

    constructor(private vhttp: Http,private constants:Constants) {
        super(vhttp,constants);
     
    }
    deleteMessage(messageId:string) {
        return this.delete(this.baseUrl + 'deleteMessage?messageId=' + messageId);

    }
	getActiveMessages() {
	
		return this.get(this.baseUrl +  'getActiveMessages');

	}
	getAllMessages() {
		return this.get(this.baseUrl + 'getAllMessages');
	}
	getMsgToEdit(messageId: string) {
	
		return this.get(this.baseUrl + 'getMsgToEdit?messageId=' + messageId);

    }
    submittMessageForAddOrEdit(createOrModify: number, message: message) {
    
        var postData = {
            id: message.id,
            title: message.title,
            msg: message.msg,
            beginDate: message.beginDate,

            expirationDate: message.expirationDate,
            msgLevel: message.msgLevel,
            active: message.active
        }
        if (createOrModify == this.constants.modify) {
            return this.post(this.baseUrl + 'editMessage', postData);
        }
        if (createOrModify == this.constants.create) {
            return this.post(this.baseUrl + 'createMessage', postData);
        }
      }

}
@Injectable()
export class SalesTeamDataService extends BaseService {
    baseUrl: string = 'api/salesTeam/'

    constructor(private vhttp: Http, private constants: Constants) {
        super(vhttp, constants);

    }

    getSalesTeamsForSelection() {

        return this.get(this.baseUrl + 'getSalesTeamsForSelection');

    }

}
@Injectable()
export class SalesGroupDataService extends BaseService {
   
    salesGroupApiPrefixes: string[];
     constructor(private vhttp: Http, private constants: Constants) {
        super(vhttp, constants);
        this.salesGroupApiPrefixes = ['api/level1SalesGroup/', 'api/level2SalesGroup/', 'api/level3SalesGroup/'];
    }

     getCompanySalesGroupAdminTreeWhereManagerInTree() {
      
        return this.get(this.salesGroupApiPrefixes[0] + 'getCompanySalesGroupAdminTreeWhereManagerInTree');

    }

}

