import { TestBed } from '@angular/core/testing';

import { LeaveProfileSettingsGuard } from './leave.guard';

describe('LeaveProfileSettingsGuard', () => {
  let guard: LeaveProfileSettingsGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(LeaveProfileSettingsGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
