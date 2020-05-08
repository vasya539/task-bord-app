import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule} from '@angular/material/card';
import { MatIconModule} from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';


import { AdminRoutingModule } from './admin-routing.module';
import { HomeAdminComponent } from './home/home.component';
import { AdminUserComponent } from './user/admin-user.component';
import { AdminComponent } from './admin.component';
import { UserCPComponent } from './user/user-read.component';
import { UserCreateComponent } from './user/user-create.cpmponent';
import { UserAdminListComponent } from './user/user-list.component';
import { UserRenameComponent } from './user/user-edit.component';
import { UserDetailsComponent } from './user/user-details.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

/*
@NgModule({
  imports: [
    MatProgressSpinnerModule,
    MatTableModule,
    MatIconModule,
    MatCardModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AdminRoutingModule,
  ],
  declarations: [
    AdminComponent,
    HomeAdminComponent,
    AdminUserComponent,
    UserCPComponent,
    UserCreateComponent,
    UserAdminListComponent,
    UserRenameComponent,
    UserDetailsComponent
  ],
})
export class AdminModule { }
*/