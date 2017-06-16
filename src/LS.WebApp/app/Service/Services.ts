import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import { message } from '../BindingModels/messageBindingModels';
import { Constants } from '../Shared/global'
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

	delete(url: string, id: number): Observable<any> {
		let headers = new Headers({ 'Content-Type': 'application/json' });
		let options = new RequestOptions({ headers: headers });
		return this._http.delete(url + id, options)
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
	
    constructor(private vhttp: Http, private constants: Constants) {
        super(vhttp, constants);
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
}
@Injectable()
export class MessageDataService extends BaseService {
	baseUrl: string = 'api/loginMsg/'

    constructor(private vhttp: Http,private constants:Constants) {
        super(vhttp,constants);
     
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

