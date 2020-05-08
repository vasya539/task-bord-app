import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddUserToProjectFormComponent } from './add-user-to-project-form.component';

describe('AddUserToProjectFormComponent', () => {
  let component: AddUserToProjectFormComponent;
  let fixture: ComponentFixture<AddUserToProjectFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddUserToProjectFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddUserToProjectFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
