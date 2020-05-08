import { Sprint } from '../../models/sprint';
import { SprintService } from '../../services/sprint.service';

import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { formatDate } from '@angular/common';
import {MatSnackBar} from '@angular/material/snack-bar';
import {Location} from '@angular/common';
import { HostListener } from '@angular/core';

@Component({
  selector: 'app-sprint-dialog',
  templateUrl: './sprint-dialog.component.html',
  styleUrls: ['./sprint-dialog.component.css']
})
export class SprintDialogComponent implements OnInit {
  public sprintForm: FormGroup;
  public sprint: Sprint;
  public id: number;
  public editMode: boolean;
  @ViewChild('f') myForm;
  constructor(private sprintService: SprintService,
              private router: Router,
              private route: ActivatedRoute,
              private snackBar: MatSnackBar,
              private location: Location ) { }

  ngOnInit(): void {
    this.InitForm();
    this.sprintService.formLoaded.next(true);
    this.route.params.subscribe((params: Params) => {
      this.id = +params.sprintId;
      this.editMode = params.sprintId !== undefined;
      if (this.editMode) {
        this.ChangeForm();
      }
     });
  }

  private async InitForm() {
    this.sprintForm = new FormGroup({
      'name': new FormControl('', Validators.required),
      'description': new FormControl('', Validators.required),
      'startDate': new FormControl('', Validators.required),
      'endDate': new FormControl('', Validators.required)
    });
  }

  private async ChangeForm() {
    const result = await this.sprintService.getSprint(this.id).toPromise();
    this.sprint = result;
    let name = this.sprint.name;
    let description = this.sprint.description;
    let startDate = formatDate(this.sprint.startDate, 'yyyy-MM-dd', 'en');
    let endDate = formatDate(this.sprint.endDate, 'yyyy-MM-dd', 'en');
    this.sprintForm.setValue({
      name: name,
      description: description,
      startDate: startDate,
      endDate: endDate
    });
  }

  OnSaveChanges() {
    const sprint = new Sprint();
    if (this.editMode) {
      sprint.projectId = this.sprint.projectId;
      sprint.id = this.id;
    } else {
      sprint.projectId = this.sprintService.projectId;
      sprint.id = 0;
    }
    sprint.name = this.sprintForm.controls.name.value;
    sprint.description = this.sprintForm.controls.description.value;
    sprint.items = [];
    sprint.startDate = this.sprintForm.controls.startDate.value;
    sprint.endDate = this.sprintForm.controls.endDate.value;
    if (this.editMode) {
      this.sprintService.updateSprint(sprint)
      .subscribe(() => {
         console.log('UPDATED!');
         this.sprintService.formLoaded.next(false);
         this.sprintService.sprintDataModified.next(sprint);
         this.snackBar.open('Updated', 'OK', { duration: 5000 });
         this.router.navigate(['../../../sprints', this.id, 'list'], {relativeTo: this.route});
      },
      (error) => {
        console.log(error);
        const message = error.status === 403 ? error.error.message : typeof error.error === 'string' ? error.error :
         typeof error.error === 'object' && error.error.title != null ? error.error.title : 'Oops. Something went wrong';
        this.snackBar.open(message, 'OK', { duration: 5000 });
      }
      );
    } else {
      this.sprintService.createSprint(sprint)
      .subscribe(() => {
        console.log('CREATED!');
        this.sprintService.formLoaded.next(false);
        this.sprintService.sprintDataModified.next(sprint);
        this.snackBar.open('Created', 'OK', { duration: 5000 });
        this.router.navigate(['../../sprints'], {relativeTo: this.route});
      },
      (error) => {
        console.log(error);
        const message = error.status === 403 ? error.error.message : typeof error.error === 'string' ? error.error :
         typeof error.error === 'object' && error.error.title != null ? error.error.title : 'Oops. Something went wrong';
        this.snackBar.open(message, 'OK', { duration: 5000 });
      });
    }
  }

  OnClear() {
    if (this.editMode) {
      setTimeout(() => {
        this.sprintForm.setValue({
          name: this.sprint.name,
          description: this.sprint.description,
          startDate: formatDate(this.sprint.startDate, 'yyyy-MM-dd', 'en'),
          endDate: formatDate(this.sprint.endDate, 'yyyy-MM-dd', 'en')
        });
      }, 0);
    } else {
      this.myForm.resetForm();
    }
  }

  OnCancel() {
    this.sprintService.formLoaded.next(false);
    this.location.back();
  }

  @HostListener('window:popstate', ['$event'])
  onPopState(event) {
    this.sprintService.formLoaded.next(false);
  }
}
