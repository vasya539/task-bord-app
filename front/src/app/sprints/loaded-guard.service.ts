import { SprintsComponent } from './sprints.component';

import { UrlTree, Router, CanDeactivate } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot} from '@angular/router';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

export interface CanDeactivateComponent {
    canActivate: () => Observable<boolean> | boolean;
  }

@Injectable({ providedIn: 'root'})
export class LoadedGuard implements CanDeactivate<CanDeactivateComponent> {
    constructor(private router: Router) {}
    canDeactivate(component: CanDeactivateComponent,
                  currentRoute: ActivatedRouteSnapshot,
                  currentState: RouterStateSnapshot,
                  nextState: RouterStateSnapshot):
          Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
              if (!nextState.url.endsWith('sprints')) { return true; } else { return false; }
    }
}
