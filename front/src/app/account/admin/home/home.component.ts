import { Component, OnInit } from '@angular/core';
import { UserInfoService } from '../../../services/user-info.service';
import { MatDialog } from '@angular/material/dialog';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-admin-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeAdminComponent implements OnInit {


  constructor(
    private userInfo: UserInfoService,
    private router:Router
    ) { }

  ngOnInit() {

  }
  
  manageUsers()
  {
    this.router.navigate(['/admin/users']);
  }
  createAdmin()
  {
    this.router.navigate(['/admin/users/create']);
  }
  
}
