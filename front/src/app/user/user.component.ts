import { Component, OnInit, Input } from '@angular/core';
import { User } from '../models/user'
import { UserIcoComponent } from './user-ico/user-ico.component';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {

  @Input() user: User;
  @Input() role: string;

  constructor() { }

  ngOnInit(): void {
  }

}
