import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../models/user';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-user-ico',
  templateUrl: './user-ico.component.html',
  styleUrls: ['./user-ico.component.css']
})
export class UserIcoComponent implements OnInit {
  @Input()
  user: User;

  @Input()
  disableLink: boolean = false;

  @Input()
  small: boolean = true;

  constructor() { }

  ngOnInit(): void {
  }

  public firstLetters(): string {
    if(this.user.firstName == null || this.user.lastName == null)
      return '??';
    return this.user.firstName[0].toUpperCase() + this.user.lastName[0].toUpperCase();
  }

  public firstName(): string {
    return this.user.firstName;
  }

  public lastName(): string {
    return this.user.lastName;
  }

  getAvaPath(): string {
    return environment.basicUrl + '/avatars/' + this.user.id + '.jpg?' + this.user.avatarTail;
  }

  onLoadingError() {
    
  }

}
