import {Injectable} from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {Observable} from 'rxjs';
import {User} from '../models/user';
import {Project} from '../models/project';

@Injectable({
  providedIn: 'root'
})
export class MainService {
  selectedProjectName: string;
  projectSelected: boolean;
  userLogined: boolean;
  currentUrl: string;

  userName: string;
  projectId: number;
  sprintId: number;

  allUrl: string;
  tempUrl: string;
  urlToSprint: string;
  urlToBoard: string;
  urlToProject: string;

  constructor(private http: HttpClient, private router: Router) {

  }

  getUrlToProject(): string {
    if (this.projectId !== null && this.projectId !== undefined) {
      return this.tempUrl = `user/${this.userName}/projects/${this.projectId}`;
    }
    return null;
  }

  getUrlToSprint(): string {
    if ((this.projectId !== null && this.projectId !== undefined) && (this.sprintId !== null && this.sprintId !== undefined)) {
      this.urlToSprint = this.tempUrl = `user/${this.userName}/projects/${this.projectId}/sprints/${this.sprintId}`;
      return this.urlToSprint;
    }
    return null;
  }

  getUrlToBoard(): string {
    if ((this.projectId !== null && this.projectId !== undefined) && (this.sprintId !== null && this.sprintId !== undefined)) {
      return this.tempUrl = `user/${this.userName}/projects/${this.projectId}/sprints/${this.sprintId}/board`;
    }
    return null;
  }

  getAllUrl(): string {
    return this.allUrl = `user/${this.userName}/projects/${this.projectId}/sprints/${this.sprintId}`;
  }
}
