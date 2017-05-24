import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
@Injectable()
export class CompanyDataService {
  baseUrl:string
  constructor(private _http: Http) {
	  this.baseUrl = "api/company/";
  }
  getCompany(companyId: string) {
	 
	  return this.get(this.baseUrl+"getCompany?companyId="+companyId)

	}
	//base calls
	get(url: string): Observable<any>{
	
		
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
		console.error(error);
		return Observable.throw(error.json().error || 'Server error');
	}

}
@Injectable()
export class AppUserDataService{
	baseUrl: string
	constructor(private _http: Http) {
		this.baseUrl = 'api/appUser/';
	}
	getLoggedInUser () {
		
		return this.get(this.baseUrl + 'getLoggedInUser');

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
		console.error(error);
		return Observable.throw(error.json().error || 'Server error');
	}

}
export class MessageDataService {
	baseUrl: string
	constructor(private _http: Http) {
		this.baseUrl = 'api/loginMsg/';
	}
	getActiveMessages() {


	}
	getAllMessages() {


	}
	submitMessageForAddOrEdit() {


	}
	getMsgToEdit() {

	}
	deleteMessage() {

	}
	editMessage() {

	}
	addMessage() {

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
		console.error(error);
		return Observable.throw(error.json().error || 'Server error');
	}

}
