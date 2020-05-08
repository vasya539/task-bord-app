import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersOnProjectComponent } from './users-on-project.component';

describe('UsersOnProjectComponent', () => {
  let component: UsersOnProjectComponent;
  let fixture: ComponentFixture<UsersOnProjectComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsersOnProjectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsersOnProjectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
