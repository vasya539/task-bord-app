import {Component, OnInit} from '@angular/core';
import {Item} from '../../models/item';
import {ItemService} from '../../services/item.service';
import {ActivatedRoute, Router} from '@angular/router';
import {Commentary} from '../../models/commentary';
import {User} from '../../models/user';
import {Status} from '../../models/status';
import {NestedTreeControl} from '@angular/cdk/tree';
import {MatTreeNestedDataSource} from '@angular/material/tree';
import {UserService} from '../../services/user/user.service';
import {ItemType} from '../../models/item-type';
import {CommentService} from '../../services/comment.service';
import {MemberService} from '../../services/member.service';
import {ProjectMember} from '../../models/project-member';
import {ConfirmDialogComponent, ConfirmDialogModel} from '../../shared/confirm-dialog/confirm-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {formatDate} from '@angular/common';
import {CreateLinkDialogComponent} from '../create-link-dialog/create-link-dialog.component';

@Component({
  selector: 'app-item-detail',
  templateUrl: './item-detail.component.html',
  styleUrls: ['./item-detail.component.css']
})
export class ItemDetailComponent implements OnInit {
  isReal: boolean;

  // second tab
  currentItem: Item;
  itemId: number;
  commentEnterText: string;
  itemComments: Commentary[] = [];
  saved: boolean;
  ok: boolean;
  selectedUser: ProjectMember;
  selectedType: ItemType;
  userList: ProjectMember[];
  // There must be loading all statuses.
  statuses: Status[];
  selectedStatus: Status;
  parentItems: Item[];
  parent: Item;
  activeUser: User;
  activeUserId: string;
  // Related part variables
  hideChild: boolean;
  hideParent: boolean;
  hideRelatives: boolean;
  childs: Item[];
  relatedItems: Item[];
  realParent: Item[] = [];
  path: string;
  ready = false;

  constructor(private itemService: ItemService,
              private route: ActivatedRoute,
              private router: Router,
              public dialog: MatDialog,
              private userService: UserService,
              private commentService: CommentService, public memberService: MemberService) {
    if (ItemService.isUserLoginned()) {
      this.route.params.subscribe(params => {
        if (params.projectId) {
          this.itemService.projectId = params.projectId;
          if (params.sprintId) {
            this.itemService.sprintId = params.sprintId;
          }
        }
      });
      this.path = this.router.url.replace('board', 'item');
      this.route.params.subscribe(param => {
        console.log(param);
        if (param.id) {
          this.itemId = param.id;
        }
        if (param.projectId) {
          if (this.memberService.projectMembers === undefined) {
            this.memberService.getMembersOfProject(param.projectId);
          }
        }
      });
      this.statuses = this.itemService.allStatuses;
    } else {
      this.router.navigate(['home']);
    }


  }

  ngOnInit(): void {
    setTimeout(() => this.loadAll(), 1000);

  }

  loadAll() {
    this.itemService.getItem(this.itemId).subscribe(data => {
        this.isReal = true;
        this.currentItem = data;
        this.selectedType = this.currentItem.itemType;
        this.selectedStatus = this.statuses.find(r => r.id === this.currentItem.statusId);
        console.log('loaded items for sprint' + this.currentItem.sprintId);
        this.itemService.getItemsBySprintId(this.currentItem.sprintId).subscribe((data1: Item[]) => {
          this.parentItems = data1.filter(r => r.id !== this.currentItem.id && r.typeId === 1 && r.sprintId == this.currentItem.sprintId && r.isArchived === false);
          this.parent = data1.find(r => r.id === this.currentItem.parentId);
          if (this.parent != null) {
            this.realParent.push(this.parent);
          }
          this.childs = data1.filter(r => r.parentId === this.currentItem.id);
        });
        this.itemService.getRelatedItems(this.currentItem.id).subscribe(value => {
          this.relatedItems = value;
          console.log('Related items: ', this.relatedItems);
        });
        this.userList = this.memberService.projectMembers?.filter(r => r.role.id == 1 || r.role.id == 2 || r.role.id == 3);
        this.activeUser = this.itemService.loginedUser;
        if (this.currentItem.assignedUserId !== null) {
          console.log('active user - ' + this.activeUser.userName);
          this.selectedUser = this.memberService.projectMembers.find(r => r.id === this.currentItem.user.id);
        }

      },
      error => this.router.navigate(['']));
    this.loadComments();
  }

  loadComments() {
    this.itemService.getItemComments(this.itemId).subscribe(data => {
      this.itemComments = data;
    });
  }

  update() {
    this.currentItem.assignedUserId = this.selectedUser?.id ?? null;
    this.currentItem.statusId = this.selectedStatus?.id ?? null;
    this.currentItem.parentId = this.parent?.id ?? null;
    this.itemService.updateItem(this.currentItem).subscribe(() => {
      alert('Updated!');
    }, error => {
      alert('Error. Try again!');
    });
  }

  archive() {
    const dialogData = new ConfirmDialogModel('Confirm Action', 'Do you really want to archive this item?');
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(info => {
      if (info) {
        this.itemService.archiveItem(this.currentItem.id).subscribe(data => {
          this.itemService.refreshData();
        }, error => {
          this.itemService.errorOccused(error.error);
        });
      }
    });

  }

  storyPointsChange(event: any) {
    this.currentItem.storyPoint = event.srcElement.valueAsNumber;
  }

  createComment() {
    const date = formatDate(new Date(), 'yyyy-MM-dd', 'en');
    console.log(date);
    const tempComment: Commentary = {
      id: 0,
      text: this.commentEnterText,
      userId: this.itemService.loginedUser.id,
      itemId: this.currentItem.id,
      date: new Date(date),
      user: null
    };
    this.commentService.createComment(tempComment).subscribe(data => {
        this.loadComments();
        this.commentEnterText = '';
      }, error => {
        this.itemService.errorOccused(error);
      }
    );
  }

  deleteComment(id: number) {
    this.commentService.deleteComment(id).subscribe(data => {
      this.loadComments();
    });
  }

  goTo(id: number) {
    this.router.navigate([`/item/${id}`]);
  }

  getDate(date): string {
    let d = new Date(date),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

    if (month.length < 2) {
      month = '0' + month;
    }
    if (day.length < 2) {
      day = '0' + day;
    }

    return [year, month, day].join('-');
  }

  toogleChild() {
    this.hideChild = !this.hideChild;
  }

  toogleRelatives() {
    this.hideRelatives = !this.hideRelatives;

  }

  toogleParent() {
    this.hideParent = !this.hideParent;
  }


  openAddLinkDialog() {
    const dialogRef = this.dialog.open(CreateLinkDialogComponent, {
        width: '350px',
        data: {
          item: this.currentItem
        }
      }
    );
    dialogRef.afterClosed().subscribe(result => {
      console.log('Vasya!!!');
      const secondItem = result[0];
      const linkType = result[1];
      this.refresh();
      if (linkType === 'Child') {
        this.childValidation(secondItem);
      } else if (linkType === 'Related') {
        this.relatedValidation(secondItem);
        console.log('Updated! Related');

      } else if (linkType === 'Parent') {
        this.parentRelationValidation(secondItem);
      }
    });
  }

  parentRelationValidation(secondItem: Item) {
    // if we already have a parent -> error
    if (this.currentItem.parentId !== null) {
      this.itemService.errorNotFromServer(`Current item '${this.currentItem.name}' already have a parent!`);
    } else {
      this.currentItem.parentId = secondItem.id;
      this.itemService.updateItem(this.currentItem).subscribe(value => {
        this.realParent.push(secondItem);
        console.log('Updated! Parent!');
        this.hideParent = false;
        this.itemService.refreshData();
      }, error => {
        this.itemService.errorOccused(error);
      });
    }
  }

  childValidation(secondItem: Item) {
    let accesed = false;
    this.currentItem.parentId === secondItem.id ? this.itemService.errorNotFromServer(`Item '${secondItem.name}' is already a parent of item '${this.currentItem.name}' !`) :
      secondItem.parentId !== null ? this.itemService.errorNotFromServer(`Item '${secondItem.name}' already have a parent!`) :
        accesed = true;
    if (accesed) {
      secondItem.parentId = this.currentItem.id;
      this.itemService.updateItem(secondItem).subscribe(value => {
        console.log('Updated! Child!');
        this.childs.push(secondItem);
        this.hideChild = false;
        this.itemService.refreshData();
      }, error => {
        this.itemService.errorOccused(error);
      });
    }
  }

  relatedValidation(secondItem: Item) {
    this.itemService.setRelationForItems(this.currentItem.id, secondItem.id).subscribe(value => {
      console.log('Updated! Related');
      this.relatedItems.push(secondItem);
      this.hideRelatives = true;

    }, error => {
      this.itemService.errorOccused(error);
    });
  }

  deleteRelation(itemId: number, relationType: string) {
    const dialogData = new ConfirmDialogModel('Confirm Action', 'Do you really want to remove this relation?');
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(info => {
      if (info) {
        if (relationType === 'Child') {
          // Delete child item
          let childItem = this.childs.find(value => value.id === itemId);
          childItem.parentId = null;
          this.itemService.updateItem(childItem).subscribe(value => {
            this.childs = this.childs.filter(value1 => value1.id !== childItem.id);
            this.itemService.refreshData();
          }, error => {
            this.itemService.errorOccused(error.error);
          });
        } else if (relationType === 'Related') {
          // Delete related item
          this.itemService.deleteRelation(this.currentItem.id, itemId).subscribe(value => {
            console.log('Removed successfully!');
            this.relatedItems = this.relatedItems.filter(value1 => value1.id !== itemId);
            this.itemService.refreshData();
          }, error => {
            this.itemService.errorOccused(error.error);
          });
        } else if (relationType === 'Parent') {
          // Delete parent item
          this.currentItem.parentId = null;
          this.itemService.updateItem(this.currentItem).subscribe(value => {
            this.realParent = this.realParent.filter(value1 => value1.id !== itemId);
            this.itemService.refreshData();
          }, error => {
            this.itemService.errorOccused(error.error);
          });
        }
      }
    });
  }

  refresh() {
    this.hideRelatives = false;
    this.hideParent = false;
    this.hideChild = false;
  }
}
