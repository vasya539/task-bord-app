import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { AdminGuard } from './admin.guard';
import { HomeAdminComponent } from './home/home.component';
import { AdminComponent } from './admin.component';
import { AdminUserComponent } from './user/admin-user.component';
import { UserCPComponent } from './user/user-read.component';
import { UserCreateComponent } from './user/user-create.cpmponent';
import { UserAdminListComponent } from './user/user-list.component';
import { UserRenameComponent } from './user/user-edit.component';
import { UserDetailsComponent } from './user/user-details.component';


const routesAdmin: Routes = [
  {
    path: '', component: AdminComponent, canActivate: [AdminGuard], children: [
      { path: '', component: HomeAdminComponent, data: { breadcrumbIgnore: true } },
      {
        path: 'users', component: AdminUserComponent, data: { breadcrumbLabel: 'Users Management' }, children: [
          { path: '', component: UserAdminListComponent, data: { breadcrumbIgnore: true } },
          { path: 'create', component: UserCreateComponent, data: { breadcrumbLabel: 'Create Admin' } },
          {
            path: ':id', component: UserDetailsComponent, data: { breadcrumbLabel: 'User' }, children: [
              { path: '', component: UserCPComponent, data: { breadcrumbIgnore: true } },
              { path: 'rename', component: UserRenameComponent, data: { breadcrumbLabel: 'Rename' } },
            ]
          },
        ]
      },
    ]
  }
];

@NgModule({
  imports: [
    /*CommonModule,
    RouterModule.forChild(routesAdmin)*/
  ],
  //exports: [RouterModule],
  declarations: []
})
export class AdminRoutingModule { }
