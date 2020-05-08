import { Injectable, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Logger } from './logger.service';
import { JwtToken } from '../models/jwt-token';
import { TokenService } from './token.service';
import { UserInfoService } from './user-info.service';
import { UserRegister } from '../models/user-register';
import { Observable } from 'rxjs';
import { map, finalize } from 'rxjs/operators';
import { Router } from '@angular/router';

import { TokenDto } from '../models/token';
import { tap } from "rxjs/operators";
import * as moment from "moment";
import { JwtHelperService } from '@auth0/angular-jwt';
import { UserRestorePassword } from '../models/user-Restore-password';
import { AdminRegister } from '../models/admin-register';
import { UserUpdate } from '../models/user-update';
import { UserPage } from '../models/user-page';
import { User } from '../models/user';

@Injectable()
export class AuthService {

  public UserCache = Object.create(null);

  private jwtService: JwtHelperService = new JwtHelperService();
  private baseApiUrl: string = /*document.location.protocol*/'http://93.175.202.228:5000'+'/api'
  private baseUrlRegister: string;
  private baseUrlLogin: string;
  private baseUrlLogout: string;
  private baseUrlRefresh: string;
  private baseUrlForget: string;
  private baseUrlCreateAdmin: string;
  private baseUrlPromoteAdmin: string;
  private baseUrlDemoteAdmin: string;
  private baseUrlPromoteUser: string;
  private baseUrlDemoteUser: string;
  private baseUrlPages: string;
  private headers = new HttpHeaders({
    'Content-Type': 'application/json',
     'Accept': 'application/json'
  });
  @Output() AuthChanged: EventEmitter<any> = new EventEmitter();


  constructor(
    private http: HttpClient,
    private tokenService: TokenService,
    private userInfoService: UserInfoService,
    private router:Router    ) {
    this.baseUrlRegister = this.baseApiUrl + '/accounts/register';
    this.baseUrlLogin = this.baseApiUrl + '/accounts/login';
    this.baseUrlLogout = this.baseApiUrl + '/accounts/logout';
    this.baseUrlRefresh = this.baseApiUrl + '/accounts/refresh';
    this.baseUrlForget = this.baseApiUrl + '/accounts/forget';
    this.baseUrlCreateAdmin = this.baseApiUrl + '/accounts/create-admin';
    this.baseUrlPromoteAdmin = this.baseApiUrl + '/accounts/roles/promote-to-admin/';
    this.baseUrlDemoteAdmin = this.baseApiUrl + '/accounts/roles/demote-from-admin/';
    this.baseUrlPromoteUser = this.baseApiUrl + '/accounts/roles/promote-to-user/';
    this.baseUrlDemoteUser = this.baseApiUrl + '/accounts/roles/demote-from-user/';
    this.baseUrlPages = this.baseApiUrl + '/accounts/page?';
  }

  public get isUser(): boolean {
    return true;
  }



  public register(registerFormModel: UserRegister): Observable<any> {
    return this.http.post(
      this.baseUrlRegister,
      JSON.stringify({
        name: registerFormModel.userName,
        email: registerFormModel.email,
        password: registerFormModel.password,
        confirmPassword: registerFormModel.confirmPassword
      }),
      { headers: this.headers }
    );
  }

  public login(login: string, password: string): Observable<TokenDto>{
    return this.http.post<TokenDto>(
      this.baseUrlLogin,
      JSON.stringify({ Password: password, Name: login }),
      { headers: this.headers }
    ).
    pipe(tap(res => {
      console.log("Token: ");
      console.log(res);
      this.setSession(res.accessToken);
      this.writeTokenFromResponse(res);
      this.AuthChanged.emit('Logged in');
    }));
  }

  private setSession(authResult) : void {
    let json = this.jwtService.decodeToken(authResult);

    const expiresAt = moment().add(json['exp'], 'second');

    console.log("exp: " + expiresAt.toLocaleString());

    console.log(authResult);

    localStorage.setItem('token', authResult);
    localStorage.setItem('id_token', json['jti']);
    localStorage.setItem("expires_at", JSON.stringify(expiresAt.valueOf()));
    localStorage.setItem("uName", json['sub']);
    localStorage.setItem('loginedUserId', json['uid']);
  }

  public refresh() {
    const jwtToken: JwtToken = this.tokenService.readJwtToken();
    Logger.log("Auth Service: sending refresh request.");
    return this.http.post(
      this.baseUrlRefresh,
      JSON.stringify({
        accessToken: jwtToken.accessToken,
        refreshToken: jwtToken.refreshToken,
        expireOn: jwtToken.expireOn
      }),
      { headers: this.headers }
    ).pipe(map(data => {
      this.writeTokenFromResponse(data);
    }));
  }

  writeTokenFromResponse(response : any) {
    const token: JwtToken = new JwtToken(
      response['accessToken'],
      response['refreshToken'],
      response['expireOn']
    );
    this.tokenService.writeToken(token);
  }

  public logout() {
    const jwtToken: JwtToken = this.tokenService.readJwtToken();
    if(jwtToken!=null)
    this.http.post(
      this.baseUrlLogout,
      JSON.stringify({
        accessToken: jwtToken.accessToken,
        refreshToken: jwtToken.refreshToken,
        expireOn: jwtToken.expireOn
      }),
      { headers: this.headers }
    ).subscribe(data => { }, err => console.log(err));
    this.tokenService.deleteToken();
    this.AuthChanged.emit('Logged out');

    this.router.navigateByUrl('/account/login');
    localStorage.clear();
  }


  public forget(email: string): Observable<any> {
    return this.http.post(
      this.baseUrlForget,
      JSON.stringify({ email: email }),
      { headers: this.headers }
    );
  }

  restorePassword(userId: string, userPass: UserRestorePassword): Observable<Object> {
    return this.http.put(this.baseApiUrl+"/accounts/" + userId + '/restore-password', userPass, { headers: this.headers });
  }

  createUser(user: UserRegister): Observable<any> {
    return this.http.post(this.baseApiUrl, user,  { headers: this.headers });
  }

  createAdmin(user: AdminRegister): Observable<any> {
    return this.http.post(this.baseUrlCreateAdmin, user, { headers: this.headers });
  }

  updateUser(userId: string, user: UserUpdate): Observable<any> {
    return this.http.put(this.baseApiUrl+'/accounts' + userId, user, { headers: this.headers });
  }

  getUserRoleById(userId: string): Observable<string[]> {
    return this.http.get<string[]>(this.baseApiUrl+'/accounts/' + userId + '/roles');
  }

  promoteToAdmin(userId: string):Observable<any>  {
    return this.http.post(this.baseUrlPromoteAdmin+userId, {headres: this.headers });
  }

  demoteFromAdmin(userId: string):Observable<any>  {
    return this.http.post(this.baseUrlDemoteAdmin+userId, {headres: this.headers });
  }

  promoteToUser(userId: string):Observable<any>  {
    return this.http.post(this.baseUrlPromoteUser+userId, {headres: this.headers });
  }

  demoteFromUser(userId: string):Observable<any>  {
    return this.http.post(this.baseUrlDemoteUser+userId, {headres: this.headers });
  }

  getUsersPage(page: number, pageSize: number): Observable<UserPage> {
    const obs = this.http.get<UserPage>(this.baseUrlPages  + 'pageNumber=' + page + '&' + 'pageSize=' + pageSize);
    obs.subscribe(result => {
      for (let user of result.items) {
        this.updateUserCache(user);
      }
    });
    return obs;
  }

  updateUserCache(user: User): void {
    this.UserCache[user.id] = user;
    Logger.log(`User cache for ${user.email} updated.`);
  }
}
