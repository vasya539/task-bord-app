import { Injectable} from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
//import 'rxjs/add/operator/map';
//import 'rxjs/add/operator/catch';
import { User } from '../models/user';
import { UserRegister } from '../models/user-register';
import { UserPage } from '../models/user-page';
import { UserUpdate } from '../models/user-update';
import { UserInfoService } from './user-info.service';
import { UserNewPassword } from '../models/user-new-password';
import { AdminRegister } from '../models/admin-register';
import { UserRole } from '../models/user-roles';
import { Logger } from './logger.service';
import { Observable } from 'rxjs';


@Injectable()
export class AccountService {

  private basePath: string;
  private basePathS: string;
  private userRegister: UserRegister;

  public UserCache = Object.create(null);

  //private tokenservice: AccessTokenService;

  headers: HttpHeaders = new HttpHeaders({
    "Content-Type": "application/json",
    "Accept": "application/json"
  });
  constructor(private http: HttpClient, private userInfoService: UserInfoService) {

    this.basePath = 'http://93.175.202.228:5000'+'/api' + '/user';
    this.basePathS = this.basePath + '/';
  }


createUser(user: UserRegister): Observable<any> {
    return this.http.post(this.basePath, user,  { headers: this.headers });
  }

  createAdmin(user: AdminRegister): Observable<any> {
    return this.http.post(this.basePath + '/create-admin', user, { headers: this.headers });
  }

  updateUser(userId: string, user: UserUpdate): Observable<any> {
    return this.http.put(this.basePathS + userId, user, { headers: this.headers });
  }

  addRole(userId: string, role: UserRole): Observable<any> {
    return this.http.put(this.basePath + "/" + userId + "/add-role", role, { headers: this.headers });
  }

  removeRole(userId: string, role: UserRole): Observable<any> {
    return this.http.put(this.basePath + "/" + userId + "/remove-role", role, { headers: this.headers });
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(this.basePathS + userId);
  }


  getUsersPage(page: number, pageSize: number): Observable<UserPage> {
    const obs = this.http.get<UserPage>(this.basePathS + 'page' + '?' + 'PageNumber=' + page + '&' + 'PageSize=' + pageSize);
    obs.subscribe(result => {
      for (let user of result.items) {
        this.updateUserCache(user);
      }
    });
    return obs;
  }


  changePassword(userId: string, userPass: UserNewPassword): Observable<Object> {
    return this.http.put(this.basePathS + userId + '/change-password', userPass, { headers: this.headers });
  }




  updateUserCache(user: User): void {
    this.UserCache[user.id] = user;
    Logger.log(`User cache for ${user.email} updated.`);
  }

  getUserCache(userId: string) : User {
    return this.UserCache[userId];
  }

  getUsersCache(userIds: string[]): User[] {
    let result: User[] = [];

    for (let userID of userIds) {
      result.push(this.UserCache[userID]);
    }

    return result;
  }


}
