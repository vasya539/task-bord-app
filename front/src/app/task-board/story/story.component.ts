import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Item} from '../../models/item';
import {ItemService} from '../../services/item.service';
import {User} from '../../models/user';
import {UserService} from '../../services/user/user.service';
import {MatDialog} from '@angular/material/dialog';
import {PropertiesDialogComponent} from '../properties-dialog/properties-dialog.component';
import {Router} from '@angular/router';
import {Status} from '../../models/status';
import {MemberService} from '../../services/member.service';
import {ProjectMember} from '../../models/project-member';

@Component({
  selector: 'app-story',
  templateUrl: './story.component.html',
  styleUrls: ['./story.component.css']
})
export class StoryComponent implements OnInit {
  @Input() story: Item;
  assignedUser: User;
  details: boolean;
  allUsers: ProjectMember[];
  hasUser: boolean;
  statuses: Status[];

  constructor(private itemService: ItemService, private userService: UserService, public dialog: MatDialog, private router: Router, private memberService: MemberService) {
    this.statuses = this.itemService.allStatuses;

  }

  ngOnInit(): void {
    this.details = false;
    this.hasUser = false;
    if (this.story.name !== 'null') {
      this.hasUser = true;
      this.allUsers = this.memberService.projectMembers;
      if (this.story.assignedUserId === null || this.story.assignedUserId === '0') {
        this.hasUser = false;
        this.assignedUser = {id: '0', userName: 'None', email: '', firstName: '', lastName: '', info: null, avatarTail: null, password: ''};
      } else {
        this.assignedUser = this.memberService.projectMembers.find(r => r.id === this.story.assignedUserId);
      }
    }

  }


  openDialog() {
    const dialogRef = this.dialog.open(PropertiesDialogComponent, {
      panelClass: 'my-dialog-window',
      width: '850px',
      height: '650px',
      data: {item: this.story, itemService: this.itemService, router: this.router}
    });

    dialogRef.afterClosed().subscribe(() => {
    });
  }

  changeUser(event: any) {
    this.assignedUser = this.allUsers.find(r => r.userName === event.value);
    this.story.user = this.assignedUser;
    this.story.assignedUserId = this.story.user.id;
  }

  click() {
    this.details = !this.details;
  }
}
