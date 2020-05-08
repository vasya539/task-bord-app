import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {ItemService} from '../../services/item.service';
import {NewItemData} from '../create-dialog/create-dialog.component';
import {ItemType} from '../../models/item-type';
import {Item} from '../../models/item';
import {Sprint} from '../../models/sprint';
import {SprintService} from '../../services/sprint.service';

@Component({
  selector: 'app-create-link-dialog',
  templateUrl: './create-link-dialog.component.html',
  styleUrls: ['./create-link-dialog.component.css']
})
export class CreateLinkDialogComponent implements OnInit {
  selectedItem: Item;
  selectedSprint: Sprint;
  allSprints: Sprint[];
  allStories: Item[];
  relatedItems: ItemGroup[] = [];
  relationTypes: string[] = ['Related', 'Child', 'Parent'];
  loading: boolean = false;

  constructor(
    public dialogRef: MatDialogRef<CreateLinkDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: NewLinkData, public itemService: ItemService, public sprintService: SprintService) {
    this.selectedItem = this.itemService.nullItem;
    // check which type of item we got
    if (this.data.item.typeId === 1) {
      // relation types for story
      this.relationTypes = ['Related', 'Child', 'Parent'];
      if (this.data.item.parentId !== null) {
        // relation types for story with parent
        this.relationTypes = ['Related', 'Child'];
      }
    } else {
      // relation type not for story
      this.relationTypes = ['Related', 'Parent'];
      if (this.data.item.parentId !== null) {
        // relation type not for story with parent
        this.relationTypes = ['Related'];
      }
    }

    // get first sprint
    this.sprintService.getSprint(this.data.item.sprintId).subscribe(value => {
      this.selectedSprint = value;
    });

    // Get all sprints
    this.sprintService.getSprintsByProjectId(this.itemService.projectId).subscribe(value => {
      this.allSprints = value;
    });
  }


  ngOnInit(): void {

  }

  changeRelation(event: any) {
    this.selectedSprint = null;
    this.selectedItem = null;
  }

  sprintUpdate(id: number) {
    // refresh related items
    this.relatedItems = [];
    this.itemService.getItemsBySprintId(id).subscribe(value => {
      this.itemService.getUserStoriesBySprint(id).subscribe(value1 => {
        this.allStories = value1;
        // iterate each story and get childs
        for (let i of this.allStories) {
          if (i.isArchived === false) {
            let tempGroup: ItemGroup = {name: i.name, items: null};
            tempGroup.items = [];
            let tempItems = value.filter(item => item.parentId === i.id && item.isArchived === false && item.id !== this.data.item.id);
            if (this.data.item.parentId !== i.id) {
              tempGroup.items.push(i);
            }
            for (let j of tempItems) {
              tempGroup.items.push(j);
            }
            this.relatedItems.push(tempGroup);
          }
        }
        // group for unparented
        let tempGroup: ItemGroup = {name: 'Unparented', items: null};
        tempGroup.items = value.filter(item => item.parentId == null && item.isArchived === false && item.typeId !== 1);
        this.relatedItems.push(tempGroup);
      });
    });
  }

  itemUpdated() {
    console.log('selected item - ', this.selectedItem);
    this.data.secondItem = this.selectedItem;
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  close() {
    this.data.secondItem = this.selectedItem;

    var object = [this.data.secondItem, this.data.relationType];
    this.dialogRef.close(object);
  }
}

export interface NewLinkData {
  item: Item;
  linkType: number;
  relatedItemId: number;
  availableToRelate: Item[];
  relationType: string;
  secondItem: Item;
}

export class ItemGroup {
  name: string;
  items: Item[];
}
