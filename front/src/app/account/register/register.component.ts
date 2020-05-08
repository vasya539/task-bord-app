import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { UserRegister } from '../../models/user-register';
import { Router } from '@angular/router';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
  showSpinner: boolean = false;

  private apiError: string = "";

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
  }

  register(): void
  {
    this.authService.register(
    new UserRegister(
      this.userName,
      this.email,
      this.password,
      this.confirmPassword
    )
  ).subscribe(data => this.router.navigate(['/login']), error => this.handleError(error));
  this.router.navigateByUrl('/home');
}

private handleError(error: any) {
  this.apiError = error['status'];
  if (error['error'] != undefined) {
    this.apiError += ': ' + error['error']['Message'];
  }
}
}
