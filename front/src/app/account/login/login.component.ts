import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../services/auth.service';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  
  username: string;
  password: string;
  showSpinner: boolean = false;
  formEnabled: boolean = true;

  constructor(
    private router: Router,
    private authService: AuthService,
     private userService: UserService) { }

  ngOnInit(): void {
  }

  login() : void {
    this.showSpinner = true;
    this.formEnabled = false;

    this.authService.login(this.username, this.password).subscribe(
      (succ)=>
      {
        UserService.knock();
        this.router.navigateByUrl('/home');
      },
      (err) => {
        this.showSpinner = false;
        this.formEnabled = true;
        console.log('oops', err);
        alert('oops');
      });
  }

}
