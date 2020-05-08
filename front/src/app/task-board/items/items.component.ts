import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Item} from '../../models/item';
import {ItemService} from '../../services/item.service';
import {CdkDrag, CdkDragDrop, moveItemInArray, transferArrayItem} from '@angular/cdk/drag-drop';
import {MatDialog} from '@angular/material/dialog';
import {CreateItemDialogComponent} from '../create-dialog/create-dialog.component';
import {environment} from '../../../environments/environment';
import {PropertiesDialogComponent} from '../properties-dialog/properties-dialog.component';
import {User} from '../../models/user';
import {ActivatedRoute, Route, Router} from '@angular/router';
import {ConfirmDialogComponent, ConfirmDialogModel} from '../../shared/confirm-dialog/confirm-dialog.component';
import {MemberService} from '../../services/member.service';

@Component({
  selector: 'app-items',
  templateUrl: './items.component.html',
  styleUrls: ['./items.component.css']
})
export class ItemsComponent implements OnInit {
  @Input() itemsStatus: number;
  @Input() statusTitle: string;
  @Input() parentId: number;
  items: Item[] = [];
  comments: Comment[] = [];
  tempItem: Item = null;
  result: any;

  constructor(public itemService: ItemService, public dialog: MatDialog, private router: Router, public memberService: MemberService
  ) {

  }

  ngOnInit(): void {
    this.loadAllItemsBySprint();
    this.initializeTempItem();

  }


  initializeTempItem() {
    this.tempItem = {
      id: 0,
      sprintId: Number(this.itemService.sprintId),
      statusId: this.itemsStatus,
      assignedUserId: null,
      description: '',
      name: '',
      typeId: environment.defaultItemType,
      isArchived: false,
      items: null,
      parentId: this.parentId,
      status: null,
      itemType: null,
      parent: null,
      user: null,
      storyPoint: null

    };
  }

   loadAllItemsBySprint() {
    this.items = [];
    if (this.parentId !== null) {
      // get child for current column
      this.itemService.getChildsWithSpecificStatus(this.parentId, this.itemsStatus).subscribe(res => {
        this.items = res;
      });
    } else {
      // get child if its Unparented
      this.itemService.getUnparented().subscribe((res: Item[]) => {
        this.items = res.filter(
          m => m.statusId === this.itemsStatus
        );
      });
    }
  }

  drop(event: CdkDragDrop<Item[]>) {
    const dialogData = new ConfirmDialogModel('Confirm Action', 'Do you want to assign this item?');
    const item: Item = event.previousContainer.data[event.previousIndex];
    const itemNew: Item = event.container.data[event.currentIndex];
    // Check if current column is 'NEW' and move upper than 'Active'
    if (event.previousContainer.element.nativeElement.parentElement.outerHTML.includes('ng-reflect-status-title="New"') && (!event.container.element.nativeElement.parentElement.outerHTML.includes('ng-reflect-status-title="Active"'))) {
      this.itemService.errorNotFromServer('Item from "New" must be moved to "Active" firstly!');
    } else if (item.statusId == 1 && item.user == null) {
      // check if user want ot assign item
      const dialogRef = this.dialog.open(ConfirmDialogComponent, {
        maxWidth: '400px',
        data: dialogData
      });

      dialogRef.afterClosed().subscribe(info => {
        if (info) {
          item.user = this.itemService.loginedUser;
          item.assignedUserId = this.itemService.loginedUser.id;
          this.changeStatusAndUpdate(item, event);
        }
      });
    } else {
      this.changeStatusAndUpdate(item, event);
    }
  }

  changeStatusAndUpdate(item: Item, event: any) {
    item.statusId = this.itemsStatus;
    this.itemService.updateItem(item).subscribe(data => {
      // check if it move in same column
      if (event.previousContainer === event.container) {
        moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      } else {
        transferArrayItem(event.previousContainer.data,
          event.container.data,
          event.previousIndex,
          event.currentIndex);
      }
      this.itemService.getItem(item.id).subscribe(gotItem => {
        // update item after dropping
        let index = this.items.findIndex(r => r.id == item.id);
        this.items[index] = gotItem;
      }, error => {
        return;
      });

    }, error => {
      this.itemService.errorOccused(error);
    });

  }

  openPropertiesDialog(tempItem: Item): void {
    const dialogRef = this.dialog.open(PropertiesDialogComponent, {
      panelClass: 'my-dialog-window',
      width: '850px',
      height: '650px',
      data: {
        item: tempItem,
        itemService: this.itemService,
        router: this.router,
      }
    });

    dialogRef.afterClosed().subscribe(() => {
      this.loadAllItemsBySprint();
    });
  }

  clickToUser(name: string) {
    this.router.navigate([`/user/${name}`]);
  }

  openAddItemDialog(): void {
    const dialogRef = this.dialog.open(CreateItemDialogComponent, {
        width: '350px',
        data: {
          name: '', description: '', status: this.itemsStatus
        }
      }
    );
    dialogRef.afterClosed().subscribe(result => {
      // get parameters from dialog
      if (result !== undefined) {
        this.tempItem.name = result[0];
        this.tempItem.description = result[1];
        this.tempItem.typeId = result[2];
        this.itemService.createItem(this.tempItem).subscribe(data => {
          this.loadAllItemsBySprint();
        });
      }
    });
  }

  clicked(event: any) {
  }


}

