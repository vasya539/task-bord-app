import { Injectable } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { ProfileSettingsComponent } from '../profile-settings.component';
import { ConfirmDialogModel, ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Injectable({
  providedIn: 'root'
})
export class LeaveProfileSettingsGuard implements CanDeactivate<ProfileSettingsComponent> {

  //result: boolean;
  constructor(public dialog: MatDialog) {}

  async canDeactivate(
    component: ProfileSettingsComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot): Promise<boolean> {
      //console.log('values unchanged: ' + component.valuesUnChanged);

      if(component.valuesUnChanged == false) {
        let title: string = 'Changes not saved';
        let msg: string;

        if(component.valuesFieldsUnChanged == false && component.avatarFile != null) {
          msg = 'Chanched settings and selected image aren\'t saved.';
        } else if(component.valuesFieldsUnChanged == false && component.avatarFile == null) {
          msg = 'Chanched settings aren\'t saved.';
        } else if(component.valuesFieldsUnChanged == true && component.avatarFile != null) {
          msg = 'Selected image isn\'t saved.';
        }

        msg += ' Are you sure you want to leave this page?';

        let res: boolean = await this.callYesNoDialog(title, msg);
        return res;
      }
      else
        return true;
  }

  // async confirmDialog() {
  //   const message = `Changes are not saved. Are you sure you want to leave this page?`;

  //   const dialogData = new ConfirmDialogModel('Confirm Action', message);

  //   const dialogRef = this.dialog.open(ConfirmDialogComponent, {
  //     maxWidth: '400px',
  //     data: dialogData
  //   });

  //   this.result = await dialogRef.afterClosed().toPromise();
  // }

  async callYesNoDialog(title: string, msg: string): Promise<boolean> {
    const dialogData = new ConfirmDialogModel(title, msg);

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    return dialogRef.afterClosed().toPromise();
  }
  
}
