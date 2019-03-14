import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IntervalEditComponent } from './interval-edit.component';

describe('IntervalEditComponent', () => {
  let component: IntervalEditComponent;
  let fixture: ComponentFixture<IntervalEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IntervalEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IntervalEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
