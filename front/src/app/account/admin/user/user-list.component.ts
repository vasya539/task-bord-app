import { Component, OnInit, Input } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { UserPage } from '../../../models/user-page';
import { Logger } from '../../../services/logger.service';

@Component({
  selector: 'app-admin-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserAdminListComponent implements OnInit {
  public userPage: UserPage;
  public ArrayTotalPages=[];
  page: number = 1;
  loaded: boolean;
  pageSize: number = 15;
  adminsRolesMap = {};
  usersRolesMap= new Map<string,string>();


  constructor(
    private authService: AuthService
    ) { }

  getUserPages(page: number, pageSize: number) {
    this.authService.getUsersPage(page, pageSize).subscribe((res: UserPage) => {
      this.loaded = true;
      this.userPage = res;
      this.page = page;
      this.fillRoles();
      for ( var _i = 0; _i < res.totalItems/pageSize ; _i++)
      {
          this.ArrayTotalPages[_i] = _i + 1;
      }
    }); 
  }

  fillRoles() {
    for (let user of this.userPage.items) {
      this.authService.getUserRoleById(user.id).subscribe((resin: string[]) => {

        if (resin.length == 0)
          Logger.error(`User [ ${user.userName} ] [ ${user.id} ] has no roles.`);

        for (let role of resin) {
          if (role == 'Administrator')
            this.adminsRolesMap[user.id] = 'Administrator';
          if(role=="User")
            this.usersRolesMap[user.id] = 'User';
        }
      });
    }
  }

  ngOnInit() {
    this.getUserPages(this.page = 1, this.pageSize)
  }

}
