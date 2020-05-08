import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable, Subject} from 'rxjs';
import {Item} from '../models/item';
import {Logger} from './logger.service';
import {catchError, tap} from 'rxjs/operators';
import {environment} from '../../environments/environment';
import {Commentary} from '../models/commentary';
import {User} from '../models/user';
import {ActivatedRoute} from '@angular/router';
import {UserService} from './user/user.service';
import {Status} from '../models/status';
import {ItemType} from '../models/item-type';
import {UserInfoService} from './user-info.service';
import {MemberService} from './member.service';
import {ProjectMember} from '../models/project-member';

@Injectable({
  providedIn: 'root'
})
export class ItemService {
  url: string = environment.basicUrl + '/items';
  requestOptions: object = {
    headers: new HttpHeaders().append('Authorization', 'Bearer <yourtokenhere>'),
    responseType: 'text'
  };

  allUsers: ProjectMember[];
  allStatuses: Status[];
  allTypes: ItemType[];
  loginedUser: User;

  sprintId: number;
  refreshDataSubject = new Subject<boolean>();
  blurDataSubject = new Subject<boolean>();
  someErrorDataSubject = new Subject<string>();
  projectId: number;
  nullItem: Item = {
    id: 0,
    name: 'null',
    description: 'null',
    sprintId: this.sprintId,
    statusId: 1,
    assignedUserId: '0',
    status: null,
    itemType: null,
    items: null,
    parentId: 0,
    parent: null,
    typeId: 1,
    isArchived: false,
    user: null,
    storyPoint: null

  };
  loginedUserName: string;

  constructor(
    private http: HttpClient, private userService: UserService, private memberService: MemberService) {
    this.nullItem.sprintId = this.sprintId;
  }

  static isUserLoginned(): boolean {
    if (localStorage.getItem('token') != null) {
      return true;
    } else {
      return false;
    }
  }

  loadRelativesParameters() {
    this.allUsers = this.memberService.projectMembers;
    this.userService.getMe().subscribe(data => {
      this.loginedUser = data;

    });

    this.allStatuses = [
      {name: 'New', id: 1},
      {name: 'Active', id: 2},
      {name: 'CodeReview', id: 3},
      {name: 'Resolved', id: 4},
      {name: 'Closed', id: 5}];
    this.allTypes = [
      {name: 'UserStory', id: 1},
      {name: 'Task', id: 2},
      {name: 'Bug', id: 3},
      {name: 'Test', id: 4}
    ];
  }

  blurWindow() {

    this.blurDataSubject.next(true);
  }

  refreshData() {
    this.refreshDataSubject.next(true);
  }

  errorOccused(error: any) {
    // Check different types of error (Angular Bug)
    if (error.error.message !== undefined) {
      this.someErrorDataSubject.next(error.error.message);
    } else if (error.error?.includes('{"message"')) {
      let message = JSON.parse(error.error);
      this.someErrorDataSubject.next(message.message);
    } else {
      this.someErrorDataSubject.next(error.error);
    }
  }

  errorNotFromServer(error: string) {
    this.someErrorDataSubject.next(error);
  }

  getItemsBySprintId(sprintId?: number): Observable<Item[]> {
    if (sprintId) {
      return this.http.get<Item[]>(this.url + '/sprints/' + sprintId).pipe(
        tap(_ => Logger.log(`Item for sprint was loaded`)
        ));
    } else {
      if (this.sprintId !== undefined) {
        return this.http.get<Item[]>(this.url + '/sprints/' + this.sprintId).pipe(
          tap(_ => Logger.log(`Item for sprint was loaded`)
          ));
      }
    }

  }

  getChildsForItem(id: number): Observable<Item[]> {
    return this.http.get<Item[]>(this.url + `/${id}/childs`);
  }

  getArchivedItemsBySprint(sprintId: number): Observable<Item[]> {
    return this.http.get<Item[]>(this.url + `/sprints/${this.sprintId}/archived`);
  }

  getItemComments(id: number): Observable<Commentary[]> {
    return this.http.get<Commentary[]>(this.url + '/' + id + '/comments').pipe(
      tap(_ => Logger.log(`Comments for item was loaded`)
      ));
  }

  getItem(id: number): Observable<Item> {
    return this.http.get<Item>(this.url + '/' + id).pipe(tap(_ => Logger.log(`Item was loaded`))
    );
  }

  getItems(): Observable<Item[]> {
    return this.http.get<Item[]>(this.url).pipe(
      tap(_ => Logger.log('Items were loaded'))
    );
  }

  getRelatedItems(itemId: number): Observable<Item[]> {
    return this.http.get<Item[]>(this.url + `/${itemId}/relation`);
  }

  setRelationForItems(firstItemId: number, secondItemId: number) {
    return this.http.post<Item[]>(this.url + `/${firstItemId}/relation/${secondItemId}`, null, this.requestOptions);
  }

  deleteRelation(firstItemId: number, secondItemId: number) {
    return this.http.delete<Item[]>(this.url + `/${firstItemId}/relation/${secondItemId}`, this.requestOptions);
  }

  createItem(item: Item): Observable<Item> {
    console.log('Post ' + item);
    item.user = null;
    return this.http.post<Item>(this.url, item, this.requestOptions).pipe(
      tap(_ => Logger.log('Item was created'))
    );
  }

  updateItem(item: Item): Observable<Item> {
    console.log(item);
    return this.http.put<Item>(this.url, item, this.requestOptions).pipe(
      tap(_ => Logger.log(`Item was updated`))
    );
  }

  deleteItem(itemId: number): Observable<Item> {
    return this.http.delete<Item>(this.url + `/${itemId}`, this.requestOptions).pipe(
      tap(_ => Logger.log(`Item was deleted`))
    );
  }

  archiveItem(itemId: number): Observable<Item> {
    return this.http.delete<Item>(this.url + `/${itemId}/archive`, this.requestOptions).pipe(
      tap(_ => Logger.log(`Item was archived`))
    );
  }

  getChildsWithSpecificStatus(parentId: number, statusId: number): Observable<Item[]> {
    return this.http.get<Item[]>(this.url + `/${parentId}/childs/statuses/${statusId}`);
  }

  getUnparented(): Observable<Item[]> {
    if (this.sprintId !== undefined) {
      return this.http.get<Item[]>(this.url + `/${this.sprintId}/null/childs`);
    }
  }

  getUserStoriesBySprint(id?: number): Observable<Item[]> {
    if (id !== undefined) {
      return this.http.get<Item[]>(this.url + '/' + id + '/stories').pipe(
        tap(_ => Logger.log(`UserStory for sprint was loaded`)
        ));
    } else if (this.sprintId !== undefined) {
      return this.http.get<Item[]>(this.url + '/' + this.sprintId + '/stories').pipe(
        tap(_ => Logger.log(`UserStory for sprint was loaded`)
        ));
    }
  }

}
