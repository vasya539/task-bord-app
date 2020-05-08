import {Component, Input, OnInit} from '@angular/core';
import {ItemService} from '../../services/item.service';
import {MatDialog} from '@angular/material/dialog';
import {PropertiesDialogComponent} from '../properties-dialog/properties-dialog.component';
import {ActivatedRoute, Router} from '@angular/router';
import {Sprint} from '../../models/sprint';
import {Item} from '../../models/item';
import {SprintService} from '../../services/sprint.service';
import {ChangeDetectorRef} from '@angular/core';
import {User} from '../../models/user';
import {UserService} from '../../services/user/user.service';
import {MainService} from '../../services/main.service';
import {UserInfoService} from '../../services/user-info.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {MemberService} from '../../services/member.service';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.css']
})
export class BoardComponent implements OnInit {
  @Input() projectId: number;
  allSprints: Sprint[];
  selectedSprint: Sprint;
  sprintId: number;
  userStories: Item[];
  nullStory: Item = this.itemService.nullItem;
  changed = 0;
  loaded = false;
  blured = false;

  constructor(private itemService: ItemService,
              public dialog: MatDialog,
              private route: ActivatedRoute,
              private router: Router,
              private sprintService: SprintService,
              private cd: ChangeDetectorRef,
              private userInfoService: UserInfoService,
              private userService: UserService,
              public mainService: MainService,
              private snackBar: MatSnackBar,
              public memberService: MemberService) {
    // check if user loginned
    if (ItemService.isUserLoginned()) {
      this.route.params.subscribe(param => {
        if (param.projectId) {
          // check if memberservice is not initialized
          if (this.memberService.projectMembers === undefined) {
            this.memberService.getMembersOfProject(param.projectId);
          }
          // get parameters from route
          this.itemService.sprintId = param.sprintId;
          this.itemService.projectId = param.projectId;

          this.loadStories(param.sprintId);
          this.itemService.loadRelativesParameters();
        }
      });
    } else {
      // redirect to home
      this.router.navigate(['home']);
    }
  }

  loadStories(id: number) {
    this.sprintId = id;
    // get stories for selected sprint
    this.itemService.getUserStoriesBySprint().subscribe(data => {
        this.userStories = data.filter(r => r.isArchived === false);
        this.loaded = true;
      }
    );
  }

  ngOnInit(): void {
    this.sprintService.getSprintsByProjectId(this.itemService.projectId).subscribe(data => {
      // get all sprints
      this.allSprints = data;
      this.nullStory = this.itemService.nullItem;
      // select current sprint
      this.selectedSprint = data.find(r => r.id === this.sprintId);
    });
    // subscribe on different events
    this.itemService.refreshDataSubject.subscribe(r => {
      this.refresh();
    });
    this.itemService.blurDataSubject.subscribe(r => {
      this.blured = !this.blured;
    });
    this.itemService.someErrorDataSubject.subscribe(errorText => {
      this.someError(errorText);
    });
  }

  changeSprint(event: any) {
    // reload board
    this.changed++;
    this.itemService.nullItem.sprintId = this.selectedSprint.id;
  }

  openCreateDialog() {
    const dialogRef = this.dialog.open(PropertiesDialogComponent, {
      panelClass: 'my-dialog-window',
      width: '850px',
      height: '650px',
      data: {item: null, itemService: this.itemService, router: this.router}
    });

  }

  someError(errorText: string) {
    this.snackBar.open(errorText, 'OK', {
      duration: 5000
    });
  }

  refresh() {
    this.loadStories(this.sprintId);
    this.changed++;
  }
}
