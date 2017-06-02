import { Component, Input, Output, EventEmitter , OnDestroy } from '@angular/core';
//https://embed.plnkr.co/NzMrJixtwZv0l2ohiZgQ/
@Component({
	selector: 'accordion',
	template: `
  <ng-content></ng-content>
          `,
	host: {
		'class': 'panel-group'
	}
})
export class Accordion {
	groups: Array<AccordionGroup> = [];

	addGroup(group: AccordionGroup): void {
		this.groups.push(group);
	}

	closeOthers(openGroup: AccordionGroup): void {
		this.groups.forEach((group: AccordionGroup) => {
			if (group !== openGroup) {
				group.isOpen = false;
			}
		});
	}

	removeGroup(group: AccordionGroup): void {
		const index = this.groups.indexOf(group);
		if (index !== -1) {
			this.groups.splice(index, 1);
		}
	}
}

@Component({
	selector: 'accordion-group',

	template: ` 
                <div  class="panel panel-default" [ngClass]="{'panel-open': isOpen}">
                 <accordion-head [heading]="heading" (toggled)=toggleOpen($event)>  </accordion-head>
                  <div class="panel-collapse" [hidden]="!isOpen">
 
                    <div class="panel-body">
                        <ng-content></ng-content>
                    </div>
                  </div>
                </div>

          `,

})
export class AccordionGroup implements OnDestroy {
	private _isOpen: boolean = false;
	@Input() heading: string;
	@Output()
	headerOpened = new EventEmitter() 

	@Input()
	set isOpen(value: boolean) {
		this._isOpen = value;
		if (value) {
			this.accordion.closeOthers(this);
			this.headerOpened.emit();
		}
	}

	get isOpen() {
		return this._isOpen;
	}

	constructor(private accordion: Accordion) {
		this.accordion.addGroup(this);
	}

	ngOnDestroy() {
		this.accordion.removeGroup(this);
	}
	
	toggleOpen(event: MouseEvent): void {
		event.preventDefault();
		this.isOpen = !this.isOpen;
	}
}


@Component({
	selector: 'accordion-head',
	template: `
                <div class="panel-heading" (click)="toggleOpen($event)">
                    <h4 class="panel-title">
                      <a href tabindex="0"><span>{{heading}}</span></a>
                    </h4>
                  </div>
          `,

})
export class AccordionHead  {
	private _isOpen: boolean = false;
	@Input() heading: string;
	@Output()
	toggled: EventEmitter<MouseEvent> = new EventEmitter<MouseEvent>();
	toggleOpen(event: MouseEvent): void {
		event.preventDefault();
		//this.isOpen = !this.isOpen;
		this.toggled.emit(event)
	}
}