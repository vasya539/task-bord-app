import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {FormBuilder, FormGroup} from '@angular/forms';
import {ItemType} from '../../models/item-type';
import {ItemService} from '../../services/item.service';

@Component({
  selector: 'app-items-dialog',
  templateUrl: './create-dialog.component.html'
  ,
  styleUrls: ['./create-dialog.component.css']
})
export class CreateItemDialogComponent implements OnInit {
  allTypes: ItemType[];

  constructor(
    public dialogRef: MatDialogRef<CreateItemDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: NewItemData, public itemService: ItemService) {
    this.data.itemType = this.itemService.allTypes.find(r => r.name == 'Task');
    this.allTypes = this.itemService.allTypes.filter(r => r.name != 'UserStory');
  }

  ngOnInit(): void {

  }


  onNoClick(): void {
    this.dialogRef.close();
  }

}

export interface NewItemData {
  status: number;
  name: string;
  description: string;
  itemType: ItemType;
}

enum Statuses {
  New = 1,
  Active,
  CodeReview = 1002,
  Resolved,
  Closed
}
