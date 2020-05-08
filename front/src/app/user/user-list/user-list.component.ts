import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {

  public users: User[] = [];
  /*
   [
    {
      id: 'aqss-aqdf',
      login: 'john',
      email: null,
      firstName: 'John',
      lastName: 'Connor'
    },
    {
      id: 'aqrt-r43e',
      login: 'alice',
      email: null,
      firstName: 'Alice',
      lastName: 'Brawn'
    },
    {
      id: 'zx90-dfk4',
      login: 'bob',
      email: null,
      firstName: 'Bob',
      lastName: 'Smith'
    }
  ];*/

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.userService.getUsers().subscribe(
      (value) => {
        //console.log('response on success: ' + value);
        for(let u of value)
        {
          //console.log(u);
          this.users.push(u as User);
        }
      }
    );
  }

}
