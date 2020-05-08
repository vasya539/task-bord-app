import {Injectable} from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {UserService} from './user/user.service';
import {Observable} from 'rxjs';
import {Item} from '../models/item';
import {tap} from 'rxjs/operators';
import {Logger} from './logger.service';
import {UserRole} from '../models/user-role';
import {ProjectMember} from '../models/project-member';
import {ItemService} from './item.service';

@Injectable({
  providedIn: 'root'
})

export class MemberService {
  url: string = environment.basicUrl;
  allLoaded = false;
  requestOptions: object = {
    headers: new HttpHeaders().append('Authorization', 'Bearer <yourtokenhere>'),
    responseType: 'text'
  };
  userRoles: UserRole[];
  projectMembers: ProjectMember[];
  currentMember: ProjectMember;

  constructor(private http: HttpClient, private userService: UserService) {
    this.getAllRoles();
  }

  getAllRoles() {
    return this.http.get<UserRole[]>(this.url + '/members').subscribe(data => {
      this.userRoles = data;
    });

  }

  getMembersOfProject(projectId: number) {
    return this.http.get<ProjectMember[]>(this.url + `/${projectId}/members`).subscribe(data => {
      this.projectMembers = data;
      this.userService.getMe().subscribe(user => {
        this.currentMember = this.projectMembers.find(r => r.id === user.id);
        this.allLoaded = true;
      });
    });
  }

  addMemberToProject(projectId: number, member: ProjectMember) {
    return this.http.post<ProjectMember>(this.url + `/${projectId}/members`, member, this.requestOptions);
  }

  changeMemberRole(projectId: number, userId: string, userRole: UserRole) {
    return this.http.put<ProjectMember>(this.url + `/${projectId}/members/${userId}`, userRole, this.requestOptions);
  }

  removeMember(projectId: number, userId: string) {
    return this.http.delete<ProjectMember>(this.url + `/${projectId}/members/${userId}`, this.requestOptions);
  }

  isMemberOfTeam(): boolean {
    return (this.currentMember.role.id == 1 || this.currentMember.role.id == 2 || this.currentMember.role.id == 3);
  }

  isNotMemberOfTeam(): boolean {
    return (this.currentMember.role.id == 0 || this.currentMember.role.id == 5);

  }

  isOwnerOrMaster(): boolean {
    return (this.currentMember.role.id == 1 || this.currentMember.role.id == 2);
  }

}
