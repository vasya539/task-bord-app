import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { newArray } from '@angular/compiler/src/util';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private router: Router) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if( req.url.startsWith(environment.basicUrl) && localStorage.getItem('token') != null) {

            const token = 'Bearer ' + localStorage.getItem('token');
            const authReq = req.clone({
                headers: req.headers.set('Authorization', token)
            });

            return next.handle(authReq).pipe(
                tap(
                    succ => {},
                    err => {
                        if(err.status == 401)
                        {
                            localStorage.removeItem('token');
                            this.router.navigateByUrl('/account/login');
                        }
                    }
                )
            );

        }
        else
            return next.handle(req);
    }
}
