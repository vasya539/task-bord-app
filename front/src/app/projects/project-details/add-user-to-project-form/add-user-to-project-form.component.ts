import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {FormControl, Validators} from '@angular/forms';

import { ProjectsUsers } from 'src/app/models/projects-users';
import { User } from 'src/app/models/user';

@Component({
  selector: 'app-add-user-to-project-form',
  templateUrl: './add-user-to-project-form.component.html',
  styleUrls: ['./add-user-to-project-form.component.css']
})
export class AddUserToProjectFormComponent implements OnInit {

  @Input() user: User;
  @Input() projectId: number;
  @Output() addedUserToProject = new EventEmitter<ProjectsUsers>();
  @Output() canceled = new EventEmitter();
  @Input() isScrumMaster: boolean;

  selectFormControl = new FormControl('', Validators.required);
  constructor() { }

  ngOnInit(): void {
  }
  onSubmit(role: number) {
    const newUserToProject: ProjectsUsers = {
      projectId : this.projectId,
      userId : this.user.id,
      userRoleId : role,
      user : this.user
    };
    this.addedUserToProject.emit(newUserToProject);
  }
  onCancel() {
    this.canceled.emit();
  }
}
