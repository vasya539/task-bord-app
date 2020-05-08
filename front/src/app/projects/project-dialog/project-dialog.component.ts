import { Component, OnInit } from '@angular/core';
import { Project } from '../../models/project';
import { ProjectService } from '../../services/project.service';
import { UserService } from '../../services/user/user.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { MatSnackBar}  from '@angular/material/snack-bar';
import { Location } from '@angular/common';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-project-dialog',
  templateUrl: './project-dialog.component.html',
  styleUrls: ['./project-dialog.component.css']
})
export class ProjectDialogComponent implements OnInit {

  public projectForm: FormGroup;
  public project: Project;
  public id: number;
  public editMode: boolean;

  constructor(private projectService: ProjectService,
              private userService: UserService,
              private router: Router,
              private route: ActivatedRoute,
              public dialogRef: MatDialogRef<ProjectDialogComponent>,
              private snackBar: MatSnackBar,
              private location: Location) { }

  ngOnInit(): void {
    this.InitForm();
    this.projectService.formLoaded.next(true);
    this.route.params.subscribe((params: Params) => {
      this.id = +params.id;
      this.editMode = params.id !== undefined;
      if (this.editMode) {
        this.ChangeForm();
      }
     });
  }

  private async InitForm() {
    this.projectForm = new FormGroup({
      'name': new FormControl('', Validators.required),
      'description': new FormControl('', Validators.required)
    });
  }

  private async ChangeForm() {
    const result = await this.projectService.getProjectById(this.id).toPromise();
    this.project = result;
    let name = this.project.name;
    let description = this.project.description;
    this.projectForm.setValue({
      name: name,
      description: description
    });
  }

  OnSaveChanges(){
    const project = new Project();
    if (this.editMode) {
      project.projectsUsers[0].userId = this.project.projectsUsers[0].userId;
      project.id = this.id;
    } else {
       project.id = 0;
    }

    project.name = this.projectForm.controls.name.value;
    project.description = this.projectForm.controls.description.value;

    if (this.editMode) {
      this.projectService.updateProject(project)
      .subscribe(() => {
         console.log('UPDATED!');
         this.projectService.formLoaded.next(false);
         this.projectService.projectDataModified.next(project);
         this.snackBar.open('Updated', 'OK');
        // this.router.navigate(['../../../projects', this.id, 'list'], {relativeTo: this.route});
      },
      (error) => {
        console.log(error);
        const message = typeof error.error === 'string' ? error.error :
         typeof error.error === 'object' && error.error.title != null ? error.error.title : 'Oops. Something went wrong';
        this.snackBar.open(message, 'OK', {duration: 5000});
      }
      );
    } else {
      this.projectService.createProject(project)
      .subscribe((projectId) => {
        console.log('CREATED!');
        this.projectService.formLoaded.next(false);
        this.projectService.projectDataModified.next(project);
        this.snackBar.open('Created', 'OK', {duration: 5000});
       // this.router.navigate(['../../projects'], {relativeTo: this.route});
      },
      (error) => {
        console.log(error);
        const message = typeof error.error === 'string' ? error.error :
         typeof error.error === 'object' && error.error.title != null ? error.error.title : 'Oops. Something went wrong';
        this.snackBar.open(message, 'OK');
      });
    }
  }

  OnClear() {
    if (this.editMode) {
      this.projectForm.setValue({
        name: this.project.name,
        description: this.project.description
      });
    } else {
      this.projectForm.reset();
    }
  }

  OnCancel() {
    this.projectService.formLoaded.next(false);
    //this.location.back();
    this.dialogRef.close();
  }
}
