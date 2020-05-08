import { ProjectService } from './../../services/project.service';
import { UserService } from './../../services/user/user.service';
import { Component, OnInit } from '@angular/core';
import { Project } from 'src/app/models/project';
import { Observable } from 'rxjs';
import { DataSource} from '@angular/cdk/collections';
import { Router, ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { Location } from '@angular/common';
import { ProjectDialogComponent } from '../project-dialog/project-dialog.component';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.css']
})

export class ProjectListComponent implements OnInit {

  projectList: Project[] = [];
  dataSource = new MatTableDataSource<Project>();
  displayedColumns: string [] = ['id','name', 'description'];

  constructor(
    private projectService: ProjectService,
    public dialog: MatDialog,
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute,
    private location: Location) { }

  ngOnInit(): void {   
    this.getProjects();
  }

  getProjects(){
    this.userService.getMyProjects().subscribe((data: Project[]) =>{
      this.dataSource.data = data;
    })
  }

  /*onSelectProject(project: Project) {
    this.projectService.getProjectById(project.id)
      .subscribe(data => this.currentProject = data);
  }*/

  OnBackClick() {
    const dialogRef = this.dialog.open(ProjectDialogComponent,
      {
       // data: { username: "", password: "" }
      });

    dialogRef.afterClosed().subscribe(result => {
      console.log(result);
    });
    this.projectService.formLoaded.next(false);
    //this.location.back();
  }

  linkClicked(id) {
    this.router.navigate(['../', 'projects', id], {relativeTo: this.route});
  }
}

