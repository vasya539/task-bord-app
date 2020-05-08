import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { User } from '../../../models/user';
import { Logger } from '../../../services/logger.service';
import { AuthService } from 'src/app/services/auth.service';
import { UserRole } from 'src/app/models/user-roles';
import {MatSnackBar} from '@angular/material/snack-bar';


@Component({
  selector: 'app-admin-user-read',
  templateUrl: './user-read.component.html',
  styleUrls: ['./users.component.css']
})
export class UserCPComponent implements OnInit {

  user: User;
  public userId: string;
  public isAdmin: boolean;
  public isUser: boolean;
  public showExtended:boolean = false;
  public apiError: string = "";
  public successMessage = "";

  constructor(
    private authService: AuthService,
    private userService: UserService, 
    private router: Router, 
    private snackBar: MatSnackBar,
    private actRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.userId = this.actRoute.snapshot.params['id'];

    this.userService.getUserById(this.userId).subscribe((user: User) => {
      this.user = user;
    }, err => this.handleError(err));

    this.updateUserRolse();
  }
  updateUserRolse()
  {
    this.authService.getUserRoleById(this.userId).subscribe((res: string[]) => {
      this.isAdmin = res.some(role => role == "Administrator");
      this.isUser = res.some(role => role == "User");
    }, error => this.handleError(error));
  }


  deleteUser() {
    this.userService.deleteUser(this.userId).subscribe(() => {
      this.router.navigate(['/admin/users']);
    }, err => this.handleError(err));
  }

  setAdminRoles(admin: boolean) {
    let userRole = new UserRole();
    userRole.Role = "Administrator";

    let obs = admin ?
     this.authService.promoteToAdmin(this.userId):
     this.authService.demoteFromAdmin(this.userId);
    obs.subscribe(() => {
      this.successMessage = "Role succesfully changed!";
      this.snackBar.open(this.successMessage, 'Ok', { duration: 5000 });
      this.apiError = "";
      this.isAdmin = admin;
    }, error => this.handleError(error));
    window.location.reload();
  }

  setUserRoles(user:boolean)
  {
    let userRole = new UserRole();
    userRole.Role = "User";

    let obs = user ? 
    this.authService.promoteToUser(this.userId) : 
    this.authService.demoteFromUser(this.userId);
    obs.subscribe(() => {
      this.successMessage = "Role succesfully changed!";
      this.apiError = "";
      this.isUser = user;
      this.snackBar.open(this.successMessage, 'Ok', { duration: 5000 });
    }, error => this.handleError(error));
    window.location.reload();
  }

  rename()
  {
    this.router.navigateByUrl("/admin/users/"+this.user.id+"/rename");
  }

  private handleError(error: any) {
    Logger.log(error);
    this.apiError = error.error.Message;
    //this.snackBar.open(error.error.Message, 'Fail!', { duration: 5000 });
    this.successMessage = "";
  }
 
}
