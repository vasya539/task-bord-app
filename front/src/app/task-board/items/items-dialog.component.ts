import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {FormBuilder, FormGroup} from '@angular/forms';

@Component({
  selector: 'app-items-dialog',
  templateUrl: './items-dialog.component.html'
  ,
  styleUrls: ['./items-dialog.component.css']
})
export class ItemsDialogComponent implements OnInit {


  constructor(
    public dialogRef: MatDialogRef<ItemsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: NewItemData) {
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
}

enum Statuses {
  New = 1,
  Active,
  CodeReview = 1002,
  Resolved,
  Closed
}
