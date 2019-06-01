import { Injectable, Output, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class ElementService {
  IsChanged = false;

  @Output() change: EventEmitter<boolean> = new EventEmitter();

  elementChanged() {
    this.IsChanged = true;
    this.change.emit(this.IsChanged);
  }
  
}
