import {AfterViewInit, Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {ItemsComponent} from '../items/items.component';

@Component({
  selector: 'app-column',
  templateUrl: './column.component.html',
  styleUrls: ['./column.component.css']
})
export class ColumnComponent implements OnInit, AfterViewInit {
  @ViewChild(ItemsComponent) viewChild: ItemsComponent;
  @Input() StatusId: number;
  @Input() columnTitle: string;
  @Input() parentId: number;

  constructor() {
  }

  ngOnInit(): void {

  }

  @Output() giveInfo() {
  }

  ngAfterViewInit(): void {
  }

  openDialog() {
    this.viewChild.openAddItemDialog();
  }

}
