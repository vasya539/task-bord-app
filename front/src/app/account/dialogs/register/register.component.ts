import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { UserRegister } from '../../../models/user-register';
import { Router } from '@angular/router';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


export interface RegisterDialogData {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterDialogComponent implements OnInit {

  hide = true;

  
  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
  showSpinner: boolean = true;

  private apiError: string = "";

  constructor(
    private authService: AuthService, 
    private router: Router,
    public dialogRef: MatDialogRef<RegisterDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RegisterDialogData
    ) { }

  ngOnInit(): void {
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  onRegister(): void
  {
    this.authService.register(
    new UserRegister(
      this.userName,
      this.email,
      this.password,
      this.confirmPassword
    )
  ).subscribe(data => this.router.navigate(['/login']), error => this.handleError(error));
  this.dialogRef.close();
}

private handleError(error: any) {
  this.apiError = error['status'];
  if (error['error'] != undefined) {
    this.apiError += ': ' + error['error']['Message'];
  }
}
}
