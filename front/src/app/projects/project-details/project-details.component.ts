import {UserInfoService} from './../../services/user-info.service';
import {UserService} from 'src/app/services/user/user.service';
import {Sprint} from 'src/app/models/sprint';
import {ProjectsUsers} from 'src/app/models/projects-users';
import {Component, OnInit, DoCheck} from '@angular/core';
import {ProjectService} from 'src/app/services/project.service';
import {Project} from 'src/app/models/project';
import {ActivatedRoute, Router} from '@angular/router';
import {MainService} from 'src/app/services/main.service';
import {MemberService} from '../../services/member.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogModel, ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details.component.html',
  styleUrls: ['./project-details.component.css']
})
export class ProjectDetailsComponent implements OnInit, DoCheck {
  currentProject: Project = null;
  usersToProject: ProjectsUsers [] = [];
  sprintsToProject: Sprint [] = [];
  projectId: number;
  access = false;

  constructor(
    private mainService: MainService,
    private projectService: ProjectService,
    private route: ActivatedRoute,
    private userInfoService: UserInfoService,
    private router: Router,
    private snackBar: MatSnackBar,
    public dialog: MatDialog,
    public memberService: MemberService) {

    this.sidenavSet();
    console.log('Current url = ', this.mainService.currentUrl);
    this.mainService.getUrlToProject();
    console.log('MAIN SERVICE PROJECT ID and USERNAME: ', this.mainService.userName, '', this.mainService.projectId);

    this.projectId = route.snapshot.params.projectId;
    this.memberService.getMembersOfProject(this.projectId);
  }

  ngOnInit(): void {
    this.projectService.getProjectById(this.projectId)
      .subscribe(data => {
        this.projectService.projectToSideNav.next(data);
        this.currentProject = data;
        this.usersToProject = data.projectsUsers;
        this.sprintsToProject = data.sprints;
      });
  }
  ngDoCheck() {
    this.access = this.usersToProject.some(up => up.userId === this.userInfoService.userId && up.userRoleId === 1);
  }

  goToSprint(sprintId: number) {
    this.router.navigate(['sprints', sprintId, 'list'], {relativeTo: this.route});
  }
  async deleteProject() {
    const resConfDialog = await this.confirmDialog(this.currentProject.name);
    if (resConfDialog) {
      this.router.navigate(['projects']);
      this.projectService.deleteProject(this.projectId).subscribe(() => { this.snackBar.open('Deleted', 'OK', { duration: 5000 }); },
      (error) => {
        console.log(error);
        const message = error.status === 403 ? error.error.message : typeof error.error === 'string' ? error.error :
        typeof error.error === 'object' && error.error.title != null ? error.error.title : 'Oops. Something went wrong';
        this.snackBar.open(message, 'OK', { duration: 5000 });
      });
    }
  }

  async confirmDialog(projectName: string): Promise<boolean> {
    const message = `Are you sure you want to delete project - "${projectName}" ?`;

    const dialogData = new ConfirmDialogModel('Project deleting', message);

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
        maxWidth: '400px',
        data: dialogData
    });
    const res: boolean = await dialogRef.afterClosed().toPromise();
    return res;
  }

  sidenavSet() {
    // Sidenav
    // There must be check if user authorized!
    this.route.params.subscribe(r => {
      if (r.userName) {
        this.mainService.userLogined = true;
        this.mainService.projectSelected = true;
        this.projectService.getProjectById(r.projectId).subscribe(data => {
          this.mainService.selectedProjectName = data.name;
        });
        this.mainService.currentUrl = this.router.url;
        // Set it into SprintService and get last sprint (DEFAULT).
        this.mainService.sprintId = 3;
        // Set user name
        this.mainService.userName = r.userName;
        // Set project id
        this.mainService.projectId = r.projectId;
      }
    });
  }
}
