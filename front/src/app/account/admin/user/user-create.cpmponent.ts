import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user/user.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { UserRegister } from '../../../models/user-register';
import { Logger } from '../../../services/logger.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-admin-user-create',
  templateUrl: './user-create.component.html',
  styleUrls: ['./admin-user.component.css']
})
export class UserCreateComponent implements OnInit {

  public createUserForm: FormGroup;
  public apiError: string = "";
  public successMessage = "";

  constructor(
    private userService: UserService, 
    private authService: AuthService,
    private router: Router, 
    private actRoute: ActivatedRoute) {
    this.createUserForm = new FormGroup({
      'userName': new FormControl('', [Validators.required, Validators.minLength(3),Validators.pattern("[A-Za-z'0-9- _]+")]),
      'email': new FormControl('', [Validators.required, Validators.email]),
    });
  }

  ngOnInit(): void {
    
  }

  public createUser() {
    let user: UserRegister = new UserRegister("","","","");
    user.userName = this.createUserForm.value.userName;
    user.email = this.createUserForm.value.email;
    console.log(user);
    this.authService.createAdmin(user).subscribe(result => {
      this.createUserForm.reset();
      this.successMessage = "You successfull create admin!";
      this.apiError = "";
    }, error => this.handleError(error));
  }
  
  private handleError(error: any) {
    this.apiError = error.error.Message;
    Logger.error(error);
    this.successMessage = "";
  }

}
