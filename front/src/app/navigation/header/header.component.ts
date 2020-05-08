import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { User } from 'src/app/models/user';
import { Router, ActivatedRoute, RoutesRecognized, ActivationEnd } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import {LoginDialogComponent} from '../../account/dialogs/login/login.component';
import {RegisterDialogComponent} from '../../account/dialogs/register/register.component';
import { MatDialog } from '@angular/material/dialog';
import {MainService} from '../../services/main.service';
import { ProjectService } from 'src/app/services/project.service';
import { Project } from 'src/app/models/project';
import { SprintService } from 'src/app/services/sprint.service';
import { UserInfoService } from 'src/app/services/user-info.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  opened: boolean;
  loginedUser: User;
  displayUserMenu: boolean = false;
  displayDetails: boolean;
  userName: string;
  projectId: number;
  sprintId: number;
  project: Project;

    constructor(
        private userService: UserService,
        private router: Router,
        private route: ActivatedRoute,
        public dialog: MatDialog,
        private authService: AuthService,
        private projectService: ProjectService,
        private sprintService: SprintService,
        private userInfoService : UserInfoService) {
      UserService.loginnedUserChangesSubject.subscribe({
        next: () => {
          //console.log('new notif from subj.');
          this.redrawUserMenu();
        }
      });
    }

  public IsAdmin():Boolean {
    return this.userInfoService.isAdmin;
  }

  ngOnInit(): void {
    this.router.events.subscribe(val => {
      if (val instanceof RoutesRecognized) {
          const lastChild = this.getSnapshot(val);
          const parameters = val.state.root.firstChild.params;
          this.projectId = parameters.projectId;
          this.userName = parameters.userName;
          if (parameters.projectId) {
            this.displayDetails = true;
            this.projectService.getProjectById(+parameters.projectId).subscribe(
              project => { this.project = project; }
            );
           } else { this.displayDetails = false; this.project = null; }
          if (lastChild.params.sprintId) { this.sprintId = lastChild.params.sprintId; }
      }
    });
    this.projectService.projectToSideNav.subscribe(project => {
      this.project = project;
      this.sprintService.getSprintsByProjectId(project.id).subscribe(
        sprints => { this.sprintId = sprints[0].id;
      });
    });

    this.redrawUserMenu();
  }

  redrawUserMenu() {
    //console.log('redrawing user menu...');
    if(localStorage.getItem('token') != null) {
      this.userService.getMe().subscribe(
        user => {
          //console.log('user is comed');
          this.loginedUser = user;
          this.displayUserMenu = true;
        },
        err => {
          console.log('cannot load me to put into header');
          this.loginedUser = null;
          this.displayUserMenu = false;
        });
    } else {
      this.loginedUser = null;
      this.displayUserMenu = false;
    }
  }
  
  getSnapshot(val: RoutesRecognized) {
    let lastChild = val.state.root.firstChild;
    while (lastChild.firstChild) {
      lastChild = lastChild.firstChild;
    }
    return lastChild;
  }

  goToDetails() {
    this.router.navigate(['/user', this.userName, 'projects', this.projectId]);
  }

  goToBoard() {
    this.router.navigate(['/user', this.userName, 'projects', this.projectId, 'sprints', this.sprintId, 'board']);
  }

  goToSprint() {
    this.router.navigate(['/user', this.userName, 'projects', this.projectId, 'sprints']);
  }

  goHome() {
    this.router.navigate(['/user', localStorage.getItem('uName')]);
  }

  goToSettings() {
    this.router.navigate(['/user', localStorage.getItem('uName'), 'settings']);
  }

  goToAdminCabinet(){
    this.router.navigate(['admin']);
  }

  logout() {
    this.authService.logout();
    UserService.knock();
  }
  openLoginDialog(): void {
    const dialogRef = this.dialog.open(LoginDialogComponent,
      {
        data: { username: "", password: "" }
      });

    dialogRef.afterClosed().subscribe(result => {
      console.log(result);
    });
    UserService.knock();
  }

  openRegisterDialog(): void {
    const dialogRef = this.dialog.open(RegisterDialogComponent,
      {
        data: { username: "",email:"", password: "",confirmPasword:"" }
      });

    dialogRef.afterClosed().subscribe(result => {
      console.log(result);
    });
    this.loginedUser = null;
    this.displayUserMenu = false;
    this.authService.logout();
    UserService.knock();
  }
}
