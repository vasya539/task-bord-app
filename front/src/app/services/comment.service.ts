import {Injectable} from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Item} from '../models/item';
import {tap} from 'rxjs/operators';
import {Logger} from './logger.service';
import {Commentary} from '../models/commentary';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  url: string = environment.basicUrl + '/comments';
  requestOptions: object = {
    headers: new HttpHeaders().append('Authorization', 'Bearer <yourtokenhere>'),
    responseType: 'text'
  };

  constructor(
    private http: HttpClient
  ) {

  }

  getComments(): Observable<Commentary[]> {
    return this.http.get<Commentary[]>(this.url).pipe(
      tap(_ => Logger.log('comments were loaded'))
    );
  }

  createComment(comment: Commentary): Observable<Commentary> {
    console.log('Post ' + comment);
    return this.http.post<Commentary>(this.url, comment, this.requestOptions);
  }

  updateComment(comment: Commentary): Observable<Commentary> {
    console.log(comment);
    return this.http.put<Commentary>(this.url, comment, this.requestOptions);
  }

  deleteComment(commentId: number): Observable<Commentary> {
    return this.http.delete<Commentary>(this.url + `/${commentId}`, this.requestOptions);
  }
}
