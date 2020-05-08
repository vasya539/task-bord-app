import { Item } from 'src/app/models/item';
import { SprintService } from 'src/app/services/sprint.service';

import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SprintListResolver implements Resolve<Item[]> {
  constructor(private sprintService: SprintService) {}

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<Item[]>|Promise<Item[]>|Item[] {
    return this.sprintService.getSprintItems(+route.paramMap.get('sprintId'));
  }
}
