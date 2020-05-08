import { ProjectsUsers } from './../models/projects-users';
import { Observable, Subject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Project } from '../models/project';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  url: string = environment.basicUrl + '/projects';
  formLoaded = new Subject<boolean>();
  projectDataModified = new Subject<Project>();
  projectToSideNav = new Subject<Project>();
  userId = -1;

  constructor(private http: HttpClient) { }

  getAllProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(this.url);
  }
  getProjectById(id: number): Observable<Project> {
    return this.http.get<Project>(this.url + '/' + id);
  }
  createProject(project: Project) {
    return this.http.post(this.url, project);
  }
  updateProject(project: Project) {
    return this.http.put(this.url, project);
  }
  addUserToProject(projectsUsers: ProjectsUsers) {
    return this.http.post(this.url + '/add-user-to-project', projectsUsers);
  }
  deleteUserFromProject(projectsUsers: ProjectsUsers) {
    return this.http.post(this.url + '/delete-user-from-project', projectsUsers);
  }
  deleteProject(id: number) {
    console.log('delete - ' + this.url + '/' + id);
    return this.http.delete(this.url + '/' + id);
  }
}
