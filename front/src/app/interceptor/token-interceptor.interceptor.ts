import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, ObservableInput, throwError, EMPTY } from 'rxjs';


import { Logger } from '../services/logger.service';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { map, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';


@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  private refreshIsOngoing = false;
  private refreshLock = false;
  private failedRequests = 0;

  constructor(private tokenService: TokenService, private authService: AuthService, private router: Router) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    //get saved token info
    const jwt = this.tokenService.readJwtToken();
    let accessToken = null;
    let expireOn = null;
    if (jwt != null) {
      accessToken = jwt.accessToken;
      expireOn = jwt.expireOn;
    }

    const isLogout: boolean = request.url.indexOf('account/logout') !== -1;
    const isRefresh: boolean = request.url.indexOf('account/refresh') !== -1;
    const isBug: boolean = request.url.indexOf('api/members') !== -1 || request.url.indexOf('api/users') !== -1;
    
    //positive authentication intervention
    if (!isLogout && !isRefresh && accessToken != null) {
      this.refreshLock = true;
      const alreadyExpired: boolean = expireOn <= new Date();
      const timeToRefresh: boolean = Date.parse(expireOn).valueOf() - new Date().valueOf() < 2 * 60 * 60 * 1000;
      if (alreadyExpired) {
        Logger.warn(`Token interceptor: access token expired on [${request.url}]. Request may fail.`);
        if (this.refreshIsOngoing === false) {
          Logger.warn(`Token interceptor: requesting refresh from [${request.url}].`);
          this.refreshIsOngoing = true;
          this.authService.refresh().subscribe(() => {
            this.refreshIsOngoing = false;
            Logger.warn(`Token interceptor: finishing refresh from [${request.url}].`);
            if (this.failedRequests > 0) {
              this.failedRequests = 0;
              this.router.navigate(['']);
            }

          }, error => {
            Logger.error(`Token interceptor: refresh from [${request.url}] gave an error. Logging out.`);
            Logger.error(error);
            this.authService.logout();
          });
        }
        else {
          Logger.warn(`Token interceptor: refresh from [${request.url}] is ignored because it is already in progress.`);
        }
        this.refreshLock = false;
      }
      request = request.clone({
          headers: request.headers.set("Authorization",
                "Bearer " + accessToken).set("Access-Control-Allow-Origin",'*') 

      });
    }
    return next.handle(request).
    pipe(map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
            //console.log('event--->>>', event);
            //console.log('request--->>>', request);
        }
        return event;
      }), catchError((error: HttpErrorResponse, event: Observable<HttpEvent<any>>) => {
        if (!isLogout && error.status === 401) {
          if (this.refreshIsOngoing) {
            this.failedRequests++;
          } else {
            if(!isBug){
            this.authService.logout();
            Logger.error(`Token interceptor: unathorized during [${request.url}], not waiting for refresh. Logging out.`);
            }
          }
        } else
        if (request.method === 'GET' && error.status === 403) {
          this.router.navigate(['/forbidden']);
          return EMPTY;
        } else if (request.method === 'GET' && error.status === 400) {
          this.router.navigate(['/bad_request']);
          return EMPTY;
        } else
        if (error.status === 404) {
          this.router.navigate(['/error']);
          return EMPTY;
        } else if (request.method === 'GET' && error.status === 500) {
          this.router.navigate(['/internal_server_error']);
          return EMPTY;
        } else {
          return throwError(error);
        }
    }));

  }
}
