import { Component, OnInit, ViewChild, Input, ViewEncapsulation} from "@angular/core"
import { BrandingService } from './Service/branding.service';
import { Observable } from 'rxjs/Rx';
import { Global } from './Shared/global';
import { HeaderComponent } from "./common/header"
import { BrandingComponent } from './common/branding'
@Component({
	
	selector: "my-app",	
	template: 
`	 
			<div *ngIf="this.branding">
		      <app-header [brandingmodel]="branding"> 
        </app-header></div>
            <div class='container'>
                <router-outlet></router-outlet>
            </div>
			`,
	styles: [` 
th {
	background-color: #d9970e !important;
}

.table > thead > tr > td.info, .table > tbody > tr > td.info, .table > tfoot > tr > td.info, .table > thead > tr > th.info, .table > tbody > tr > th.info, .table > tfoot > tr > th.info, .table > thead > tr.info > td, .table > tbody > tr.info > td, .table > tfoot > tr.info > td, .table > thead > tr.info > th, .table > tbody > tr.info > th, .table > tfoot > tr.info > th {
	background-color: #CCCCCC;
	font-weight: bold
}

.table-hover > tbody > tr > td.info:hover, .table-hover > tbody > tr > th.info:hover, .table-hover > tbody > tr.info:hover > td, .table-hover > tbody > tr:hover > .info, .table-hover > tbody > tr.info:hover > th {
	background-color: #BBBBBB
}

.header-logo {
	display: block;
	height: 3em;
	background-image: url('../../Content/img/safelink-wireless.png');
	background-repeat: no-repeat;
	background-size: contain;
	margin: .5em 0
}
.header-nav{
	background-color:#fff !important
}
a {
	color: #d9970e
}

.text-link {
	color: #d9970e !important;
}

.admin-link, .admin-link:hover {
	color: #d9970e !important;
}

.header-welcome > span > a {
	color: #d9970e !important;
}

.bg-primary {
	background-color: #888888;
	border: 1px solid #666666;
}

.bg-info {
	background-color: #FAFAFA;
	border: 1px solid #DDDDDD;
}

.padded {
	padding: 1%;
}

.bottomMargin {
	margin-bottom: 3%
}

.panel-heading {
	background-color: #7b7b7b !important;
}

.panel-heading-teamuser {
	padding: 10px 0px;
	margin: 0px 0px;
	border-radius: 5px;
	background-color: #d9970e;
}

.active {
	background-color: #7b7b7b !important;
}

.navbar-default .navbar-nav > .open > a, .navbar-default .navbar-nav > .open > a:hover, .navbar-default .navbar-nav > .open > a:focus {
	background-color: #d9970e;
}

.header-menu {
	height: auto;
	background-color: #d9970e !important;
}

/*Global overrides to bootstrap menu*/
.navbar-default {
	background: none;
	box-shadow: none;
}

.navbar {
	border: 0px;
	margin-bottom: 0px
}

.navbar-collapse {
	//padding: 0px;
}

.navbar-default .navbar-nav > li > a {
	color: #F3F3F3
}

.navbar-brand, .navbar-nav > li > a {
	text-shadow: 0 1px 0 rgba(0,0,0,.25)
}

.navbar-default .navbar-nav > li > a:hover, .navbar-default .navbar-nav > li > a:focus {
	color: #FFFFFF
}

.navbar-default .navbar-nav > .open > a, .navbar-default .navbar-nav > .active > a {
	background-image: none;
}

	.navbar-default .navbar-nav > .open > a, .navbar-default .navbar-nav > .open > a:hover, .navbar-default .navbar-nav > .open > a:focus {
		color: #FFFFFF
	}

.dropdown-menu {
	border-top-width: 0px
}

	.dropdown-menu > li > a:hover, .dropdown-menu > li > a:focus {
		background-image: none;
	}

.navbar-default .navbar-toggle {
	background-color: #EEEEEE
}

	.navbar-default .navbar-toggle:hover, .navbar-default .navbar-toggle:focus {
		background-color: #FAFAFA
	}

.dropdown-header {
	font-weight: bold;
	font-size: 13px;
	color: #111111;
	padding-left: 16px !important;
}

.navbar-collapse {
	border-top-width: 0px;
	box-shadow: none
}

.navbar-default .navbar-nav > .open > a, .navbar-default .navbar-nav > .active > a {
	box-shadow: none
}

@media (max-width: 767px) {
	.navbar-default .navbar-nav .open .dropdown-menu > li > a {
		color: #EEEEEE
	}

		.navbar-default .navbar-nav .open .dropdown-menu > li > a:hover, .navbar-default .navbar-nav .open .dropdown-menu > li > a:focus {
			color: #FFFFFF
		}

	.dropdown-header {
		padding-left: 16px !important;
		color: #FFFFFF;
	}

	.collapse.navbar-collapse.in {
		box-shadow: 0 6px 12px rgba(0,0,0,.175);
	}

	.dropdown-menu .divider {
		background-color: #888888
	}

	.padded {
		padding: 1.5%;
	}

	.bottomMargin {
		margin-bottom: 5%
	}
}
/*END BOOTSTRAP OVERRIDE*/

.order-field-title {
	color: #000 !important;
}

.order-button-small,
.order-button-small:hover,
.order-button-small:focus {
	background-color: #d9970e !important;
}

.order-button,
.order-button:hover,
.order-button:focus {
	background-color: #d9970e !important;
}

.admin-button,
.admin-button:hover,
.admin-button:focus {
	background-color: #d9970e !important;
}


.alpha-btn {
	margin: 2px;
	font-weight: bold;
	color: #fff;
	background-color: #7b7b7b;
}

	.alpha-btn:hover {
		color: #eee;
	}

.searchBox {
	border: 1px solid #1f2022;
	background-color: #F1F1F1;
	padding: 6px;
	margin: 5px;
}

.grayBox {
	border: 1px solid #1f2022;
	background-color: #F1F1F1;
	padding: 6px;
	margin: 5px;
}

.searchBox .left {
	float: left;
}

.searchBox .order-button {
	margin-top: 5px
}

.clear {
	clear: left
}

.ag-header-container {
	background-color: #d9970e;
	color: #fff;
	text-align: left;
}

.ag-secondary .ag-header-container {
	background-color: #7b7b7b !important;
}

.ag-header-cell-resize {
	border-right: 1px solid #ddd !important;
}

.ag-bootstrap .ag-root {
	border: 1px solid #ddd !important;
}

.ag-cell {
	padding: 4px !important;
}

	.ag-cell:focus {
		outline: auto 5px #ffef11 !important
	}

.ag-cell-no-focus {
	border-bottom: 1px solid #ddd !important;
	border-right: 1px solid #ddd !important;
}

.ag-header {
	background-color: #d9970e !important;
}

.ag-secondary .ag-header {
	background-color: #7b7b7b !important;
}

.ag-row-selected {
	background-color: #AAA !important;
}

.ag-cell-focus {
	border: 1px solid #ffef11 !important
}

.ag-font-style {
	-webkit-user-select: auto !important
}

.ag-header-cell ag-header-cell-sortable ag-header-cell-sorted-none {
	color: #fff;
	text-align: left;
}

.ag-header-icon .ag-sort-ascending-icon .ag-hidden {
	color: #fff
}

.ag-header-icon .ag-sort-descending-icon {
	color: #fff
}

.ag-cell-value {
	padding: 5px
}

.ag-bootstrap .ag-header-cell-label {
	text-align: left
}

.ag-row:hover {
	background-color: #fcffb6 !important
}



`],
	providers: [BrandingService],
	encapsulation: ViewEncapsulation.None ,
	})

export class AppComponent implements OnInit {

	branding: {};
	msg: string;
	constructor(private _brandingService: BrandingService) {}
	ngOnInit(): void {
		this.GetBranding();
	
	}
	GetBranding(){

	 this._brandingService.get("api/company/getCompany?companyId=65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c")
			.subscribe(branding => {
				this.branding = branding.Data;
				return this.branding
			}, error => this.msg = <any>error);

		
	}
}
	
