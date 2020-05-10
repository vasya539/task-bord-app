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
  noUpper:boolean = false;
  noLower:boolean = false;
  noDigit:boolean = false;
  
  
  public regUserForm: FormGroup;
  
  private apiError: string = "";
  
  constructor(
    private authService: AuthService, 
    private router: Router
    ) {
      this.regUserForm = new FormGroup({
        'userName': new FormControl('', [Validators.required, Validators.minLength(3),Validators.pattern("[A-Za-z'0-9- _]+")]),
        'password': new FormControl('', [Validators.required, Validators.minLength(8)]),
        'confirmPassword': new FormControl('', [Validators.required, Validators.minLength(8)]),
        'email': new FormControl('', [Validators.required, Validators.email]),
      }, { validators: this.comparePassword });
}

ngOnInit(): void {
}

register(): void
{
  this.noUpper = false;
  this.noLower = false;
  this.noDigit = false;
  if(!this.checkForValid(this.regUserForm))  return;
  this.authService.register(
    new UserRegister(
      this.userName,
      this.email,
      this.password,
      this.confirmPassword
      )
      ).subscribe(data => this.router.navigate(['account/login']), error => this.handleError(error));
    }
    
    private handleError(error: any) {
      this.apiError = error['status'];
      if (error['error'] != undefined) {
        this.apiError += ': ' + error['error']['Message'];
      }
    }
    
private checkForValid(group: FormGroup):boolean{
  const pass = group.value.password+"";
  const confirm = group.value.confirmPassword+"";


  if(!pass.match(/[A-Z]/))  {
    this.noUpper= true;
    return false;
  }
  else if(!pass.match(/[a-z]/)){
    this.noLower= true;
    return false;
  }
  else if(!pass.match(/[0-9]/)){
    this.noDigit= true;
    return false;
  }
  return true;
}

private comparePassword(group: FormGroup) {
  const pass = group.value.password+"";
  const confirm = group.value.confirmPassword+"";


  console.log(this);
  console.log("========================================");

  return pass === confirm ? null : { notSame: true };
}
}
