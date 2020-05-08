import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { UserService } from '../../services/user/user.service';
import { UserRestorePassword } from '../../models/user-Restore-password';


@Component({
  selector: 'app-reset',
  styleUrls: ['./reset.component.css'],
  templateUrl: './reset.component.html'
})
export class ResetComponent implements OnInit, OnDestroy {

  public querySubscription: Subscription;
  public resetForm: FormGroup;
  public restoreToken: string;
  public userId: string;
  public isParamsExist = true;
  public formProductModel: UserRestorePassword = new UserRestorePassword();

  constructor(private authService: AuthService, private route: ActivatedRoute,
    private router: Router, private userService: UserService) {
    this.resetForm = new FormGroup({
      'password': new FormControl('', Validators.required),
      'confirmPassword': new FormControl('', Validators.required),
    }, { validators: this.comparePassword });
    this.querySubscription = route.queryParams.subscribe(
      (queryParam: any) => {
        this.userId = queryParam['userId'];
        this.restoreToken = queryParam['code'];
      }
    );
  }

  ngOnInit() {
    if (this.userId !== undefined && this.restoreToken !== undefined) {
      this.isParamsExist = true;
    } else {
      this.isParamsExist = false;
    }  
  }

  ngOnDestroy() {
    this.querySubscription.unsubscribe();
  }

  reset() {
    this.formProductModel.NewPassword = this.resetForm.value.password;
    this.formProductModel.ConfirmNewPassword = this.resetForm.value.confirmPassword;
    this.formProductModel.RestoreToken = this.restoreToken;
    this.authService.restorePassword(this.userId, this.formProductModel).subscribe(() => {
      this.router.navigate(['/account/login']);
    });
    
  }

  private comparePassword(group: FormGroup) {
    const pass = group.value.password;
    const confirm = group.value.confirmPassword;
    return pass === confirm ? null : { notSame: true };
  }

}
