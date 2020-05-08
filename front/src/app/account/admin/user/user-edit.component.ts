import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user/user.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { UserUpdate } from '../../../models/user-update';
import { User } from '../../../models/user';
import { Logger } from '../../../services/logger.service';
import { AuthService } from 'src/app/services/auth.service';


@Component({
  selector: 'app-admin-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./admin-user.component.css']
})
export class UserRenameComponent implements OnInit {

  public editUserForm: FormGroup;
  public userId: string;
  public apiError: string = "";
  public successMessage: string = "";
  private userUpdate: UserUpdate = new UserUpdate();


  constructor(
     private authService : AuthService,
    private userService: UserService, 
    private router: Router, 
    private actRoute: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.userId = this.actRoute.parent.snapshot.params['id'];
    this.initializeForm();

    this.userService.getUserById(this.userId).subscribe((res: User) => {
      this.userUpdate.userName = res.userName;
      this.initializeForm();
    }, error => this.handleError(error));

  }

  private initializeForm() {
    this.editUserForm = new FormGroup({
      userName: new FormControl(this.userUpdate.userName, [Validators.required, Validators.minLength(3), Validators.pattern("[A-Za-z'0-9- _]+")]),
    });
  }



  editUser() {
    this.userUpdate.userName = this.editUserForm.value.userName;
    this.authService.updateUser(this.userId, this.userUpdate).subscribe(() => {
      this.router.navigate(['/admin', 'users', this.userId]);
      }, error => this.handleError(error));
  }

  private handleError(error: any) {
    this.apiError = error.error.Message;
    this.successMessage = "";
  }

}
