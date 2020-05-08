import { Component, OnInit, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AuthService } from '../../../services/auth.service';

export interface LoginDialogData {
  username: string;
  password: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginDialogComponent implements OnInit {

  hide = true;
  
  username: string;
  password: string;
  showSpinner: boolean = false;
  formEnabled: boolean = true;

  constructor(
    private authService: AuthService,
    public dialogRef: MatDialogRef<LoginDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: LoginDialogData) { }

  ngOnInit(): void {
  }
  onNoClick(): void {
    this.dialogRef.close();
  }

  onLogin(): void {
    console.log(this.data);

    this.authService.login(this.data.username, this.data.password)
      .subscribe(res => { console.log(res.accessToken); });

    this.dialogRef.close();
  }
}
