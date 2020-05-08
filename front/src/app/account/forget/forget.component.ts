import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Logger } from '../../services/logger.service';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router'

@Component({
  selector: 'app-forget',
  styleUrls: ['./forget.component.css'],
  templateUrl: './forget.component.html'
})
export class ForgetComponent implements OnInit {

  public forgetForm: FormGroup;
  public successMessage: string = "";
  public apiError: string = "";

  constructor(
    private authServie: AuthService,
    private router: Router
    ) {
    this.forgetForm = new FormGroup({
      'email': new FormControl('', [Validators.required, Validators.email])
    });
   }

  ngOnInit() {
  }

  forget() {
    this.authServie.forget(this.forgetForm.value.email)
      .subscribe(data => {
        this.successMessage = 'Reset mail have been sent';
        this.apiError = "";
      }, err => {
        this.successMessage = "";
        Logger.error(err);
        this.apiError = err.error.Message;
      });
  }
  cancel()
  {
    this.router.navigateByUrl('/home');
  }
}
