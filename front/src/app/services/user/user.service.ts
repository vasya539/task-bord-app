import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders, HttpResponse} from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import {environment} from '../../../environments/environment';
import {User} from '../../models/user';
import { Router } from '@angular/router';
import { getLocaleMonthNames } from '@angular/common';
import { Project } from 'src/app/models/project';
import { EventEmitter } from 'protractor';
import { FoundUsersPage } from 'src/app/models/foundUsersPage';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  private headers = new HttpHeaders({
    'Content-Type': 'application/json',
     'Accept': 'application/json',
    'Access-Control-Allow-Origin': '*'
  });


  private request_url: string = environment.basicUrl + '/users/';

  public static readonly loginnedUserChangesSubject = new Subject();
  //public static UserEventEmitter: EventEmitter = new EventEmitter();

  constructor(private http: HttpClient, private router: Router) {

  }

  static knock() {
    this.loginnedUserChangesSubject.next();
  }

  getUserByUserName(userName: string): Observable<User> {
    return this.http.get<User>(this.request_url + 'by-username/' + userName + '?detailed=false');
  }

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(this.request_url + id, {headers: this.headers});
  }

  getDetailedUserByUserName(userName: string): Observable<User> {
    return this.http.get<User>(this.request_url + 'by-username/' + userName);
  }

  getDetailedUserById(id: string): Observable<User> {
    return this.http.get<User>(this.request_url + 'detailed/' + id);
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(environment.basicUrl + '/users-all', {headers: this.headers});
  }

  getMe(): Observable<User> {
    return this.http.get<User>(environment.basicUrl + '/users', {headers: this.headers});
  }

  getMyProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(this.request_url + localStorage.getItem('loginedUserId') + '/projects', {headers: this.headers});
  }

  getProjectsOfUser(id: string): Observable<Project[]> {
    return this.http.get<Project[]>(this.request_url + id.toString() + '/projects', {headers: this.headers});
  }

  changeMySettings(newUser: User): Observable<Object> {
    return this.http.put(this.request_url, newUser);
  }

  findUsers(template: string, page: number = 1): Observable<FoundUsersPage> {
    return this.http.get<FoundUsersPage>(this.request_url + 'search?template=' + template + '&page=' + page);
  }

  changeAvatar(file: File): Observable<any> {
    let formData: FormData = new FormData()
    formData.append('avatar', file, file.name);

    return this.http.post(environment.basicUrl + '/avatars', formData);
  }

  clearAvatar(): Observable<any> {
    return this.http.delete(environment.basicUrl + '/avatars');
  }

  deleteMe(): Observable<any> {
    return this.http.delete(this.request_url);
  }

  deleteUser(userId:string):Observable<any>{
    return this.http.delete(this.request_url + userId);
  }
}
