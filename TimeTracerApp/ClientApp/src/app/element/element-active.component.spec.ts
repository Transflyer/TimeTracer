import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ElementActiveComponent } from './element-active.component';

describe('ElementActiveComponent', () => {
  let component: ElementActiveComponent;
  let fixture: ComponentFixture<ElementActiveComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ElementActiveComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ElementActiveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
