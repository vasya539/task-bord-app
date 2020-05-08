import {Component, EventEmitter, Inject, OnDestroy, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from '@angular/material/dialog';
import {Item} from '../../models/item';
import {ItemService} from '../../services/item.service';
import {Status} from '../../models/status';
import {Commentary} from '../../models/commentary';
import {CommentService} from '../../services/comment.service';
import {Router} from '@angular/router';
import {ItemType} from '../../models/item-type';
import {ProjectMember} from '../../models/project-member';
import {MemberService} from '../../services/member.service';
import {formatDate} from '@angular/common';
import {ConfirmDialogComponent, ConfirmDialogModel} from '../../shared/confirm-dialog/confirm-dialog.component';
import {CreateLinkDialogComponent} from '../create-link-dialog/create-link-dialog.component';
import {SprintService} from '../../services/sprint.service';


@Component({
  selector: 'app-properties-dialog',
  templateUrl: './properties-dialog.component.html',
  styleUrls: ['./properties-dialog.component.css']
})
export class PropertiesDialogComponent implements OnInit, OnDestroy {
  // loading all users of current project.
  userList: ProjectMember[];
  // load parent for item
  parent: Item = null;
  // Can be parents
  tempItem: Item;
  parents: Item[];
  //  loading all statuses.
  statuses: Status[];
  selectedUser: ProjectMember;
  itemTypes: ItemType[];
  selectedType: ItemType;
  title: string;
  commentEnterText: string;
  selectedStatus: Status;
  // additional variables for operations
  editing: boolean;
  saved: boolean;
  ok: boolean;
  someError: boolean;
  creating: boolean;
  // comments for item
  comments: Commentary[] = [];
  path: string;
  // Variables for relatives block
  hideChild: boolean;
  hideParent: boolean;
  hideRelatives: boolean;
  childs: Item[] = [];
  relatedItems: Item[] = [];
  realParent: Item[] = [];
  availableToRelate: Item[];

  constructor(
    public dialogRef: MatDialogRef<PropertiesDialogComponent>,
    public dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: PropertyItemData,
    private itemService: ItemService,
    private commentService: CommentService,
    private sprintService: SprintService,
    public memberService: MemberService) {
    // Get route for links
    this.path = this.data.router.url.replace('board', 'item');
    // Get statuses and types
    this.statuses = this.itemService.allStatuses;
    this.itemTypes = this.itemService.allTypes;

    this.initializeTempItem();

    // Get all items by sprint
    this.itemService.getItemsBySprintId().subscribe(data1 => {
      // Items which can be parent
      this.parents = data1.filter(r => r.id !== this.data.item.id && r.isArchived === false); // && r.typeId === 1
      // Item parent
      this.parent = data1.find(r => r.id === this.data.item.parentId);
      if (this.parent != null) {
        this.realParent.push(this.parent);
      }
      // if UserStory have parent -> get it
      if (this.data.item.typeId === 1 && this.data.item.parentId !== null) {
        this.itemService.getItem(this.data.item.parentId).subscribe(value => {
          this.parent = value;
          if (this.parent !== null) {
            this.realParent = [];
            this.realParent.push(this.parent);
          }
        });
      }
      this.childs = data1.filter(r => r.parentId === this.data.item.id);
      this.availableToRelate = data1.filter(r => r.id !== this.data.item.id);
    });

    if (this.data.item != null) {
      // Get related items
      this.itemService.getRelatedItems(this.data.item.id).subscribe(value => {
        this.relatedItems = value;
        console.log('Related items: ', this.relatedItems);
      });
    }
    this.itemService.blurWindow();


  }

// Temp item for operations
  initializeTempItem() {
    this.tempItem = {
      id: 0,
      sprintId: Number(this.itemService.sprintId),
      statusId: 1,
      assignedUserId: null,
      description: '',
      name: '',
      typeId: 2,
      isArchived: false,
      items: null,
      parentId: 0,
      status: null,
      itemType: null,
      parent: null,
      user: null,
      storyPoint: null
    };
  }

  ngOnInit(): void {
    // Logic if we want to change properties for item
    if (this.data.item != null) {
      this.selectedType = this.data.item.itemType;
      this.selectedStatus = this.statuses.find(r => r.id === this.data.item.statusId);
      this.title = 'Properties';
      this.creating = false;
      // get users which can assign item
      this.userList = this.memberService.projectMembers.filter(r => r.role.id == 1 || r.role.id == 2 || r.role.id == 3);
      if (this.data.item.user !== null) {
        this.selectedUser = this.memberService.projectMembers.find(r => r.id === this.data.item.user.id);
       }
    } else {
      // Logic if we create new item
      this.selectedStatus = this.statuses[0];
      this.selectedType = this.itemTypes[0];
      this.data.item = this.tempItem;
      this.creating = true;
      this.title = 'Create';
      // get users which can assign item
      this.userList = this.memberService.projectMembers.filter(r => r.role.id == 1 || r.role.id == 2 || r.role.id == 3);
      if (this.data.item.user !== null) {
        this.selectedUser = this.memberService.projectMembers.find(r => r.id === this.data.item.assignedUserId);
        console.log('Selected user - ', this.selectedUser);
      }
      // Get parent items for task/bug/story
      this.itemService.getItems().subscribe(data => {
        this.parents = data.filter(r => r.id !== this.data.item.id
          && r.typeId === 1
          && r.sprintId == this.itemService.sprintId
          && r.isArchived === false);
      });
    }
    this.saved = true;
    this.ok = true;
    this.someError = false;
    this.loadComments();
    console.log('1');
  }

  ngOnDestroy(): void {
    this.clearSelections();
    this.itemService.blurWindow();
  }

  loadComments() {
    this.comments = [];
    this.itemService.getItemComments(this.data.item.id).subscribe((data) => {
        this.comments = data;
      }
    );
  }

  close() {
    this.dialogRef.close();
  }

  archive() {
    const dialogData = new ConfirmDialogModel('Confirm Action', 'Do you really want to archive this item?');
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(info => {
      if (info) {
        this.itemService.archiveItem(this.data.item.id).subscribe(data => {
          this.itemService.refreshData();
          setTimeout(() => this.close(), 1000);

        }, error => {
          this.itemService.errorOccused(error.error);
        });
      }
    });

  }

  update() {
    // enable spinner icon
    this.editing = !this.editing;
    this.saved = false;
    // set properties if user check it
    this.data.item.assignedUserId = this.selectedUser?.id ?? null;
    this.data.item.statusId = this.selectedStatus?.id ?? null;
    this.data.item.parentId = this.parent?.id ?? null;
    // if we Update
    if (!this.creating) {
      this.data.item.user = this?.selectedUser ?? null;
      this.itemService.updateItem(this.data.item).subscribe(() => {
        // enable spinner
        setTimeout(() => {
          this.saved = true;
          this.ok = false;
        }, 1000);
        setTimeout(() => {
          // stop spinner
          this.ok = true;
        }, 2500);
        // reload board
        this.itemService.refreshData();

      }, error => {

        this.itemService.errorOccused(error);
        this.someError = true;
        this.saved = true;
        setTimeout(() => {
          this.someError = false;
        }, 2500);
      });
    } else {
      // if we create new item
      this.data.item.typeId = this.selectedType?.id ?? null;
      this.data.item.sprintId = Number(this.itemService.sprintId);
      this.data.item.user = null;
      this.itemService.createItem(this.data.item).subscribe(() => {
        this.data.item.user = this?.selectedUser ?? null;
        setTimeout(() => {
          this.saved = true;
          this.ok = false;
        }, 1000);
        setTimeout(() => {
          this.ok = true;
        }, 2500);
        setTimeout(() => this.close(), 1000);
        if (this.data !== undefined) {
          this.itemService.refreshData();
        }

      }, error => {
        this.itemService.errorOccused(error);
        this.someError = true;
        this.saved = true;
        setTimeout(() => {
          this.someError = false;
        }, 2500);
      });
      this.clearSelections();
    }


  }

  storyPointsChange(event: any) {
    this.data.item.storyPoint = event.srcElement.valueAsNumber;
  }

  deleteComment(id: number) {
    this.commentService.deleteComment(id).subscribe(data => {
      this.loadComments();
    }, error => {
      this.itemService.errorOccused(error);
    });
  }

  createComment() {
    const date = formatDate(new Date(), 'yyyy-MM-dd', 'en');
    console.log(date);
    const tempComment: Commentary = {
      id: 0,
      text: this.commentEnterText,
      userId: this.itemService.loginedUser.id,
      itemId: this.data.item.id,
      date: new Date(date),
      user: null
    };
    this.commentService.createComment(tempComment).subscribe(data1 => {
        this.loadComments();
        this.commentEnterText = '';
      }, error => {
        this.itemService.errorOccused(error);
      }
    );
  }

  clearSelections() {
    this.data.itemService.nullItem.name = 'null';
    this.data.itemService.nullItem.description = '';
    this.data.itemService.nullItem.parentId = 0;
    this.data.itemService.nullItem.assignedUserId = '0';


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

  goToItem(id: number) {

    const dialogData = new ConfirmDialogModel('Confirm Action', 'Do you want to go to this item? You will lose unsaved data!');
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(info => {
      if (info) {
        this.data.router.navigate([this.path, id]);
        this.close();
      }
    });

  }

  openAddLinkDialog() {
    const dialogRef = this.dialog.open(CreateLinkDialogComponent, {
        width: '350px',
        data: {
          item: this.data.item
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

  refresh() {
    this.hideRelatives = false;
    this.hideParent = false;
    this.hideChild = false;
  }

  parentRelationValidation(secondItem: Item) {
    // if we already have a parent -> error
    if (this.data.item.parentId !== null) {
      this.itemService.errorNotFromServer(`Current item '${this.data.item.name}' already have a parent!`);
    } else {
      this.data.item.parentId = secondItem.id;
      this.itemService.updateItem(this.data.item).subscribe(value => {
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
    this.data.item.parentId === secondItem.id ? this.itemService.errorNotFromServer(`Item '${secondItem.name}' is already a parent of item '${this.data.item.name}' !`) :
      secondItem.parentId !== null ? this.itemService.errorNotFromServer(`Item '${secondItem.name}' already have a parent!`) :
        accesed = true;
    if (accesed) {
      secondItem.parentId = this.data.item.id;
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
    this.itemService.setRelationForItems(this.data.item.id, secondItem.id).subscribe(value => {
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
            this.itemService.errorOccused(error);
          });
        } else if (relationType === 'Related') {
          // Delete related item
          this.itemService.deleteRelation(this.data.item.id, itemId).subscribe(value => {
            console.log('Removed successfully!');
            this.relatedItems = this.relatedItems.filter(value1 => value1.id !== itemId);
            this.itemService.refreshData();
          }, error => {
            this.itemService.errorOccused(error);
          });
        } else if (relationType === 'Parent') {
          // Delete parent item
          this.data.item.parentId = null;
          this.itemService.updateItem(this.data.item).subscribe(value => {
            this.realParent = this.realParent.filter(value1 => value1.id !== itemId);
            this.itemService.refreshData();
          }, error => {
            this.itemService.errorOccused(error);
          });
        }
      }
    });
  }
}

export interface PropertyItemData {
  item: Item;
  statusName: string;
  itemService: ItemService;
  router: Router;
  event: EventEmitter<boolean>;
  userName: string;
}
