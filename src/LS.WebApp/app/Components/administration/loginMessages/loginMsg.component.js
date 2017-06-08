"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var common_1 = require("@angular/common");
require("rxjs/add/operator/mergeMap");
var global_1 = require("../../../Shared/global");
var angular2_toaster_1 = require("angular2-toaster");
var Services_1 = require("../../../Service/Services");
var global_2 = require("../../../Shared/global");
var filters_1 = require("../../../Shared/filters");
//
var LoginMsgComponent = (function () {
    function LoginMsgComponent(_messageDataService, _global, toasterService, _constants, datePipe, router) {
        this._messageDataService = _messageDataService;
        this._global = _global;
        this._constants = _constants;
        this.datePipe = datePipe;
        this.router = router;
        this.rowData = [];
        this.loading = true;
        this.hasLoadded = false;
        this.msgGridOptions = {};
        this.toasterService = toasterService;
    }
    LoginMsgComponent.prototype.ngOnInit = function () {
        var self = this;
        var filter = new filters_1.YesNo();
        var msgFilter = new filters_1.MsgLevel();
        this.columnDefs = [
            {
                headerName: "Title", headerTooltip: "Title", field: "id", minWidth: 70, width: 185,
                cellRenderer: function (params) {
                    var eSpan = document.createElement('div');
                    eSpan.innerHTML = '<span> <a class="text-link" > ' + params.data.title.substring(0, 30) + ' </a></span >';
                    eSpan.addEventListener('click', function () {
                        self.router.navigate(['modifyLoginMsg', params.data.id]);
                    });
                    return eSpan;
                }
            },
            {
                headerName: "Message", field: "msg", headerTooltip: "Message", minWidth: 85, width: 200,
            },
            {
                headerName: "Start Date", field: "beginDate", headerTooltip: "Start Date", minWidth: 85, width: 125,
                cellRenderer: function (params) {
                    if (params.data) {
                        return '<span>' + self.datePipe.transform(params.data.beginDate, 'MM/dd/yyyy') + '</span>';
                    }
                }
            },
            {
                headerName: "End Date", field: "expirationDate", headerTooltip: "EndDate", minWidth: 85, width: 200,
                cellRenderer: function (params) {
                    if (params.data) {
                        return '<span>' + self.datePipe.transform(params.data.expirationDate, 'MM/dd/yyyy') + '</span>';
                    }
                }
            },
            {
                headerName: "Importance Level", field: "msgLevel", headerTooltip: "Importance Level", minWidth: 80, width: 100,
                cellRenderer: function (params) {
                    if (params.data) {
                        return '<span>' + msgFilter.transform(params.data.msgLevel, null) + '</span>';
                    }
                }
            },
            {
                headerName: "Active", field: "active", headerTooltip: "Active", minWidth: 65, width: 70,
                cellRenderer: function (params) {
                    if (params.data) {
                        return '<span>' + filter.transform(params.data.active, null) + '</span>';
                    }
                }
            }
        ];
        this.getAllMessages();
    };
    LoginMsgComponent.prototype.getAllMessages = function () {
        var _this = this;
        var self = this; //not sure why but must put key work this in variable to use in function call
        this.loading = true;
        this._messageDataService.getAllMessages().subscribe(function (response) {
            var response = response;
            if (!response.isSuccessful) {
                _this.loading = false;
                _this.toasterService.pop('error', 'Error Getting Login Messages.', response.error.userHelp);
            }
            _this.messages = response.data;
            _this.loading = false;
            _this.msgGridOptions = {
                columnDefs: _this.columnDefs,
                //angularCompileRows: true, not used in version2
                rowData: _this.messages,
                enableColResize: true,
                enableSorting: true,
                enableFilter: true,
                rowHeight: 22,
                suppressRowClickSelection: true,
                onGridReady: function (event) {
                    self.sizeToFit();
                    self.autoSizeAll();
                },
                getRowStyle: function (params) {
                    if (params.node.floating) {
                        return { 'font-weight': 'bold' };
                    }
                },
            };
        }, function (error) { return _this.msg = error; });
    };
    LoginMsgComponent.prototype.sizeToFit = function () {
        this.msgGridOptions.api.sizeColumnsToFit();
    };
    LoginMsgComponent.prototype.autoSizeAll = function () {
        var allColumnIds = [];
        this.columnDefs.forEach(function (columnDef) {
            allColumnIds.push(columnDef.field);
        });
        this.msgGridOptions.columnApi.autoSizeColumns(allColumnIds);
    };
    LoginMsgComponent.prototype.onBtExport = function () {
        var exportName;
        var params = {
            allColumns: true,
            fileName: exportName,
        };
        this.msgGridOptions.api.exportDataAsCsv(params);
    };
    LoginMsgComponent.prototype.modifyMessage = function (messageId) {
        alert('here');
        this.router.navigate(['modifyLoginMsg', messageId]);
    };
    return LoginMsgComponent;
}());
LoginMsgComponent = __decorate([
    core_1.Component({
        //template: `<div *ngIf="_global.loggedInUser && !loading">
        //<loginMsg-detail> </loginMsg-detail>
        //     </div>
        //<img src= "/Content/img/spiffygif_30x30.gif" style= "text-align: center" [hidden] = "!loading"/>
        //	`,	
        templateUrl: '../../../app/components/administration/loginMessages/loginMsg.html',
        styleUrls: ['../../Content/sass/siteAngular.css']
    }),
    __metadata("design:paramtypes", [Services_1.MessageDataService, global_1.Global, angular2_toaster_1.ToasterService, global_2.Constants, common_1.DatePipe, router_1.Router])
], LoginMsgComponent);
exports.LoginMsgComponent = LoginMsgComponent;
//# sourceMappingURL=loginMsg.component.js.map