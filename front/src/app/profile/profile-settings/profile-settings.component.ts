import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { User } from 'src/app/models/user';
import { FormControl, Validators, FormBuilder, FormGroup } from '@angular/forms';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { stringify } from 'querystring';
import { ConfirmDialogModel, ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-profile-settings',
  templateUrl: './profile-settings.component.html',
  styleUrls: ['./profile-settings.component.css']
})
export class ProfileSettingsComponent implements OnInit {
  user: User;
  formGroup: FormGroup;
  showSpinner: boolean = false;
  showAvaSpinner: boolean = false;
  showExtended: boolean = false;
  valuesUnChanged: boolean = true;
  valuesFieldsUnChanged: boolean = true;
  requiredFieldErrorMessage: string = "This field is required and can't be greater than 20 characters";

  avatarFile: File = null;
  canClickUpdateAva: boolean = true;
  canClickDeleteAva: boolean = true;

  moreLessButtText: string = 'more...';

  private ctx: CanvasRenderingContext2D;

  constructor(
      private userService: UserService,
      private formBuilder: FormBuilder,
      private router: Router,
      private route: ActivatedRoute,
      private snackBar: MatSnackBar,
      private dialog: MatDialog,
      ) {
    this.makeForm();
  }

  ngOnInit(): void {
    this.route.params.subscribe(
        (params: Params) => {
          let userName: string = params['userName'];
          if(localStorage.getItem('uName') != userName)
            this.router.navigate(['/user', localStorage.getItem('uName'), 'settings']);
        },
        err => {
          ;
        });
  }

  extractErrorMessage(err: any): string {
    let e;
    if(typeof(err) === 'string') {
      e = JSON.parse(err);
    } else {
      e = err;
    }

    if(e.message != null) {
      return e.message;
    } else if(e.title != null) {
      return e.title;
    }
  }

  handleError(err: any) {
    console.log('err: ', err);
    let message: string = 'error occured';
    if(err.status != 0)
      message = this.extractErrorMessage(err.error);

    console.log(message);
    this.snackBar.open(message, 'OK');
  }

  getDetailedLoginnedUser(): Observable<User> {
    return this.userService.getDetailedUserById(localStorage.getItem('loginedUserId'));
  }

  makeForm() {
    this.getDetailedLoginnedUser().subscribe(
      user => {
        this.setUser(user);

        this.formGroup = this.formBuilder.group({
          'firstName': [this.user.firstName, [Validators.required, Validators.maxLength(20)]],
          'lastName': [this.user.lastName, [Validators.required, Validators.maxLength(20)]],
          'email': [this.user.email, [Validators.required, Validators.email]],
          'info': [this.user.info, [Validators.maxLength(1000)]]
        });
        this.subscribeFieldsChanges();
      },
      err => {
        this.handleError(err);
      }
    )
  }

  setUser(user: User) {
    this.user = user;
        if(this.user.info == null)
          this.user.info = '';
  }

  subscribeFieldsChanges() {
    this.formGroup.controls['firstName'].valueChanges.subscribe(
      () => {
        this.checkFieldsChanges();
    });
    this.formGroup.controls['lastName'].valueChanges.subscribe(
      () => {
        this.checkFieldsChanges();
    });
    this.formGroup.controls['email'].valueChanges.subscribe(
      () => {
        this.checkFieldsChanges();
    });
    this.formGroup.controls['info'].valueChanges.subscribe(
      () => {
        this.checkFieldsChanges();
    });
  }

  checkFieldsChanges() {
    if(
      this.user.firstName === this.formGroup.controls['firstName'].value &&
      this.user.lastName === this.formGroup.controls['lastName'].value &&
      this.user.email === this.formGroup.controls['email'].value &&
      this.user.info === this.formGroup.controls['info'].value
    )
      this.valuesFieldsUnChanged = true;
    else
      this.valuesFieldsUnChanged = false;

    if(this.valuesFieldsUnChanged && this.avatarFile === null)
      this.valuesUnChanged = true;
    else
      this.valuesUnChanged = false;
  }

  getErrorEmail() {
    return this.formGroup.get('email').hasError('required') ? this.requiredFieldErrorMessage :
      this.formGroup.get('email').hasError('email') ? 'Not a valid emailaddress': '';
  }

  getErrorInfo() {
    return this.formGroup.get('info').errors ? 'Info cannot contain more than 1000 characters.' : '';
  }

  async onConfirmClick() {
    let answer: boolean = await this.callYesNoDialog(
          'Confirm new settings',
          'Are you sure you want to apply new settings?');

    if(answer == true)
      this.confirm();
  }

  confirm() {
    this.showSpinner = true;

    this.setSettings();

    this.userService.changeMySettings(this.user).subscribe(
      success => {
        //console.log('success.');
        this.showSpinner = false;
        UserService.knock();
        this.valuesFieldsUnChanged = true;
        this.checkFieldsChanges();

        if(this.valuesUnChanged == true)
          this.router.navigateByUrl('/home');
      },
      err => {
        this.handleError(err);
      }
    )
  }

  cancel() {
    this.router.navigateByUrl('/home');
  }

  setSettings() {
    this.user.firstName = this.formGroup.controls['firstName'].value;
    this.user.lastName = this.formGroup.controls['lastName'].value;
    this.user.email = this.formGroup.controls['email'].value;
    this.user.info = this.formGroup.controls['info'].value;
  }


  onFileChanged(event) {
    if(event.target.files.length == 1)
      this.avatarFile = event.target.files[0];
    else
      this.avatarFile = null;

    this.checkFieldsChanges();

    if(this.avatarFile == null) {
      this.clearCanvas();
      return;
    }

    var reader = new FileReader();
    reader.onload = function(event){
        let img = new Image();
        let cnv = document.getElementById('ps-canvas') as HTMLCanvasElement;
        let cx = cnv.getContext('2d');

        img.onload = function(){
            cnv.width = 100;
            cnv.height = 100;
            let sx, sy, side: number;

            if(img.width != img.height)
            {
              if(img.width < img.height)
              { sx = 0; side = img.width; sy = img.height / 2 - side / 2; }
              else
              { sy = 0; side = img.height; sx = img.width / 2 - side / 2; }
            }
            else
            {
              sx = 0; sy = 0; side = img.width;
            }
            cx.drawImage(img, sx, sy, side, side, 0, 0, 100, 100);
            let currImg = document.getElementById('curr-ava');
            if(currImg != null)
              currImg.hidden = true;
        }
        img.src = event.target.result as string;
    }
    reader.readAsDataURL(this.avatarFile);
  }

  updateAvatar() {
    if(this.avatarFile != null) {
      this.canClickUpdateAva = false;
      this.showAvaSpinner = true;
      this.userService.changeAvatar(this.avatarFile).subscribe(
        succ => {
          UserService.knock();
          this.avatarFile = null;
          this.checkFieldsChanges();
          this.canClickUpdateAva = true;
          this.showAvaSpinner = false;

          this.getDetailedLoginnedUser().subscribe(
            succ => {
              this.setUser(succ);
            },
            err => {
              //console.log('err whe loadin user for redraw avatar in settings: ', err);
              this.handleError(err);
            }
          );
          console.log('successful updated avatar');
        },
        err => {
          this.canClickUpdateAva = true;
          this.handleError(err);
        }
      );
    }
  }

  deleteAvatar() {
    console.log('deleting avatar...');
    this.canClickDeleteAva = false;
    this.showAvaSpinner = true;
    this.userService.clearAvatar().subscribe(
      succ => {
        UserService.knock();
        this.canClickDeleteAva = true;
        this.showAvaSpinner = false;
        
        this.getDetailedLoginnedUser().subscribe(
          succ => {
            this.setUser(succ);
            this.avatarFile = null;
            this.checkFieldsChanges();
            this.clearCanvas();
          },
          err => {
            //console.log('err whe loadin user for redraw avatar in settings: ', err);
            this.handleError(err);
          }
        );
        console.log('successful deleted avatar.');
      },
      err => {
        this.handleError(err);
        this.canClickDeleteAva = true;
      }
    )
  }

  clearCanvas() {
    let cnv = document.getElementById('ps-canvas') as HTMLCanvasElement;
    let cx = cnv.getContext('2d');
    cx.clearRect(0, 0, 100, 100);

    if(this.user.avatarTail != null) {
      let currImg = document.getElementById('curr-ava');
      if(currImg != null)
        currImg.hidden = false;
    }
  }

  async onUpdateAvatarClick() {
    let answer: boolean = await this.callYesNoDialog(
          'Confirm avatar changing',
          'Are you sure you want to upload this image to use as avatar?');

    if(answer == true)
      this.updateAvatar();
  }

  async onDeleteAvatarClick() {
    let answer: boolean = await this.callYesNoDialog(
          'Confirm avatar removing',
          'Are you sure you want to delete your current avatar?');

    if(answer == true)
      this.deleteAvatar();
  }


  async callYesNoDialog(title: string, msg: string): Promise<boolean> {
    const dialogData = new ConfirmDialogModel(title, msg);

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    return dialogRef.afterClosed().toPromise();
  }

  onMoreLessClick() {
    if(this.showExtended == false) {
      this.showExtended = true;
      this.moreLessButtText = 'less...';
    } else {
      this.showExtended = false;
      this.moreLessButtText = 'more...';
    }
  }

  async onDeleteUser() {
    let answer: boolean = await this.callYesNoDialog(
        'Confirm profile deleting',
        'If you will delete your profile you will not log in again. '+
        'Are you sure you want to delete your profile.');
    if(answer == true)
      this.deleteProfile();
  }

  deleteProfile() {
    this.userService.deleteMe().subscribe(
      succ => {
        localStorage.clear();
        UserService.knock();
        this.router.navigateByUrl('/account/login');
      },
      err => {
        this.handleError(err);
      }
    );
  }

  getAvaPath(): string {
    return environment.basicUrl + '/avatars/' + this.user.id + '.jpg?' + this.user.avatarTail;
  }

}
