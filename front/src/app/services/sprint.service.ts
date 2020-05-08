import {Injectable, EventEmitter} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, Subject} from 'rxjs';
import {Sprint} from '../models/sprint';
import {Logger} from './logger.service';
import {tap} from 'rxjs/operators';
import {environment} from '../../environments/environment';
import { Item } from '../models/item';

@Injectable({
  providedIn: 'root'
})
export class SprintService {
  url: string = environment.basicUrl + '/sprints';
  sprintLoaded = new EventEmitter<number>();
  sprintChanged = new EventEmitter<number>();
  formLoaded = new Subject<boolean>();
  sprintDataModified = new Subject<Sprint>();
  projectId = -1;

  constructor(private http: HttpClient) {}

  getSprint(id: number): Observable<Sprint> {
    return this.http.get<Sprint>(this.url + '/' + id).pipe(
      tap(_ => Logger.log(`${id} Sprint was loaded`))
    );
  }

  getSprintItems(id: number): Observable<Item[]> {
    return this.http.get<Item[]>(this.url + '/' + id + '/items').pipe(
      tap(_ => Logger.log(`${id} Sprint items were loaded`))
    );
  }

  getSprints(): Observable<Sprint[]> {
    return this.http.get<Sprint[]>(this.url).pipe(
      tap(_ => Logger.log('sprints were loaded'))
    );
  }

  getSprintsByProjectId(id: number): Observable<Sprint[]> {
    return this.http.get<Sprint[]>(environment.basicUrl + '/projects/' + id + '/sprints').pipe(
      tap(_ => Logger.log('Sprints were loaded for Project with id=' + id))
    );
  }

  createSprint(sprint: Sprint): Observable<Sprint> {
    console.log('Post ' + sprint);
    return this.http.post<Sprint>(this.url, sprint).pipe(
      tap(_ => Logger.log('sprint was created'))
    );
  }

  updateSprint(sprint: Sprint): Observable<Sprint> {
    console.log(sprint);
    return this.http.put<Sprint>(this.url, sprint).pipe(
      tap(_ => Logger.log(`${sprint.id} sprint was updated`))
    );
  }

  deleteSprint(sprintId: number): Observable<Sprint> {
    return this.http.delete<Sprint>(this.url + `/${sprintId}`).pipe(
      tap(_ => Logger.log(`${sprintId} item was deleted`))
    );
  }


}
