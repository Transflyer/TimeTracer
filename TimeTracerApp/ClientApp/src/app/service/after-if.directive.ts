import {Directive, AfterContentInit, Output, EventEmitter} from '@angular/core';


@Directive({ selector: '[after-if]' })
export class AfterIfDirective implements AfterContentInit {
  @Output('after-if')
  public after: EventEmitter<AfterIfDirective> = new EventEmitter();

  public ngAfterContentInit(): void {
    setTimeout(() => {
      // timeout helps prevent unexpected change errors
      this.after.next(this);
    });
  }
}
