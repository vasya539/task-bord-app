import { MemberService } from './../../../services/member.service';
import { Component, OnInit, Input, DoCheck } from '@angular/core';

import { ProjectsUsers } from 'src/app/models/projects-users';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user/user.service';
import { ProjectMember } from 'src/app/models/project-member';
import { UserRole } from 'src/app/models/user-role';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogModel, ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-users-on-project',
  templateUrl: './users-on-project.component.html',
  styleUrls: ['./users-on-project.component.css']
})
export class UsersOnProjectComponent implements OnInit, DoCheck {
  @Input() projectId: number;
  @Input() usersOnProject: ProjectsUsers[] = [];
  @Input() access;
  showAddUser = false;
  nameFilter = '';
  users: User[] = [];
  currentUser: User = null;
  showForm = false;
  isScrumMaster = false;
  hasNext = false;
  pageNumber = 0;
  countMarkSpace = 0;
  constructor(private userService: UserService,
              private memberService: MemberService,
              public dialog: MatDialog,
              private snackBar: MatSnackBar) { }

  ngOnInit(): void {

  }
  ngDoCheck() {
    this.isScrumMaster = this.isScrumMasterOnProject();
    this.sortByRole();
  }

  onRoleChange(newRole: number, up: ProjectsUsers) {
    const role = new UserRole();
    let isErr = false;
    role.id = newRole;
    this.memberService.changeMemberRole(up.projectId, up.userId, role)
    .subscribe(() => { this.snackBar.open('User was deleted from project', 'OK'); },
    (error) => {
      isErr = true;
      const message = this.errors(error);
      this.snackBar.open(message, 'OK');
    });
    if (!isErr) {
      const findUP = this.usersOnProject.findIndex(uOnp => uOnp === up);
      this.usersOnProject[findUP].userRoleId = newRole;
    }
  }

  sortByRole() {
    this.usersOnProject?.sort((a, b) => {
      if (a.userRoleId > b.userRoleId) {
        return 1;
      }
      if (a.userRoleId < b.userRoleId) {
        return -1;
      }
      return 0;
    });
  }

  search(filter: string) {
    this.nameFilter = filter;
    const arr = filter.split('');
    this.countMarkSpace = arr.filter(ch => ch === ' ').length;
    console.log(this.countMarkSpace);
    console.log(this.pageNumber);
    if (filter.length >= 4 + this.countMarkSpace) {
      this.loadUsers();
    } else {
      this.hasNext = false;
      this.pageNumber = 0;
      this.users = [];
    }
  }
  loadUsers() {
    this.userService.findUsers(this.nameFilter, this.pageNumber+1).subscribe(data => {
      if (data.users.length) {
        this.users = this.users.concat(this.filterUsersWhoAreOnProject(data.users));
        this.pageNumber = data.pageNumber;
        this.hasNext = data.hasNext;
      } else {
        this.users = [];
        this.pageNumber = 0;
        this.hasNext = false;
      }
    },
    (error) => {
      const message = this.errors(error);
      this.snackBar.open(message, 'OK', { duration: 5000 });
    } );
  }
  cancelLoadUsers() {
    this.hasNext = false;
    this.pageNumber = 0;
    this.users = [];
    this.showAddUser = false;
    this.nameFilter = '';
  }
  isScrumMasterOnProject(): boolean {
    return this.usersOnProject?.some(up => up.userRoleId === 2);
  }

  filterUsersWhoAreOnProject(users: User[]): User[] {
    return users.filter(user => !this.usersOnProject.some(pu => user.id === pu.userId));
  }

  showAddUserForm(user: User) {
    this.currentUser = user;
    this.showForm = true;
  }

  addUserToProject(projectUser: ProjectsUsers) {
    this.showForm = false;
    this.showAddUser = false;
    // find -  is user on project
    const findUsersOnProject: ProjectsUsers[] = this.usersOnProject.filter(up => up.userId === projectUser.userId);

    if (!findUsersOnProject.length) {
      const pm: ProjectMember = new ProjectMember();
      pm.id = projectUser.userId;
      pm.role = new UserRole();
      pm.role.id = projectUser.userRoleId;

      let isErr = false;
      this.memberService.addMemberToProject(projectUser.projectId, pm)
      .subscribe(() => { this.snackBar.open('User was added to project', 'OK', { duration: 5000 }); },
      (error) => {
        isErr = true;
        const message = this.errors(error);
        this.snackBar.open(message, 'OK', { duration: 5000 });
      });
      if (!isErr) {
        this.usersOnProject.push(projectUser);
        this.users.splice(this.users.findIndex(user => user.id === projectUser.userId), 1);
      }

    }
  }

  cancelForm() {
    this.showForm = false;
  }
  async deleteUserFromProject(projectUser: ProjectsUsers) {

    const resConfDialog = await this.confirmDialog(projectUser.user.firstName, projectUser.user.lastName);
    if (resConfDialog) {
      let isErr = false;
      this.memberService.removeMember(projectUser.projectId, projectUser.userId)
      .subscribe(() => { this.snackBar.open('User was deleted from project', 'OK', { duration: 5000 }); },
      (error) => {
        isErr = true;
        const message = this.errors(error);
        this.snackBar.open(message, 'OK', { duration: 5000 });
      });
      if (!isErr) {
        this.usersOnProject.splice(this.usersOnProject.findIndex(up => up.userId === projectUser.userId), 1);
      }
    }
  }

  errors(error: any): any {
      return error.status === 403 ? error.error.message : typeof error.error === 'string' ? error.error :
        typeof error.error === 'object' && error.error.title != null ? error.error.title : 'Oops. Something went wrong';
  }

  async confirmDialog(firstName: string, lastName: string): Promise<boolean> {
    const message = `Are you sure you want to delete this user "${firstName} ${lastName}" from project?`;

    const dialogData = new ConfirmDialogModel('deleting user from project', message);

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
        maxWidth: '400px',
        data: dialogData
    });
    const res: boolean = await dialogRef.afterClosed().toPromise();
    return res;
  }
}
