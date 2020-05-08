import {ChangeDetectorRef, Component, OnInit, ViewChild} from '@angular/core';
import {Item} from '../../models/item';
import {ItemService} from '../../services/item.service';
import {ActivatedRoute, Router} from '@angular/router';
import {SelectionModel} from '@angular/cdk/collections';
import {MatTable, MatTableDataSource} from '@angular/material/table';
import {UserService} from '../../services/user/user.service';
import {MemberService} from '../../services/member.service';

@Component({
  selector: 'app-archived-items',
  templateUrl: './archived-items.component.html',
  styleUrls: ['./archived-items.component.css']
})
export class ArchivedItemsComponent implements OnInit {

  sprintId: number;
  elements: Item[] = [];
  selection = new SelectionModel<Item>(true, []);
  displayedColumns: string[] = ['select', 'id', 'name', 'description', 'typeId'];
  dataSource = new MatTableDataSource<Item>();
  @ViewChild(MatTable) table: MatTable<any>;

  constructor(public itemService: ItemService, private route: ActivatedRoute, private router: Router, private changeDetectorRefs: ChangeDetectorRef, public memberService: MemberService) {

    if (ItemService.isUserLoginned()) {

      this.route.params.subscribe(param => {
        console.log(param);
        if (param.projectId) {
          if (memberService.projectMembers === undefined) {
            memberService.getMembersOfProject(param.projectId);
          }
          this.itemService.sprintId = param.sprintId;

        } else {
          this.router.navigate(['home']);
        }
      });
    } else {
      this.router.navigate(['home']);
    }

  }

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems() {
    this.itemService.getArchivedItemsBySprint(0).subscribe((data) => {
      this.elements = data;
      this.dataSource.data = this.elements;
    });
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkToReturn() {
    for (const i of this.selection.selected) {
      this.itemService.archiveItem(i.id).subscribe(data => {
        this.changeDetectorRefs.detectChanges();
        this.table.renderRows();
        this.loadItems();

      });
    }
  }

  checkToDelete() {
    for (const i of this.selection.selected) {
      this.itemService.deleteItem(i.id).subscribe(data => {
        this.changeDetectorRefs.detectChanges();
        this.table.renderRows();
        this.loadItems();
      });
    }
  }

  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  checkboxLabel(row?: Item): string {
    if (!row) {
      return `${this.isAllSelected() ? 'select' : 'deselect'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.id + 1}`;
  }

}

export interface ArchivedItem {
  name: string;
  description: string;

}
