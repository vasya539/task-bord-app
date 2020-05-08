import {SprintService} from '../../services/sprint.service';
import {Item} from '../../models/item';

import {Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router, Params, Data} from '@angular/router';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-sprint-list',
  templateUrl: './sprint-list.component.html',
  styleUrls: ['./sprint-list.component.css'],
})
export class SprintListComponent implements OnInit {
  public data: Item[];
  public isLoading: boolean;

  constructor(private sprintService: SprintService,
              private router: Router,
              private route: ActivatedRoute,
              private snackBar: MatSnackBar) {}

  ngOnInit(): void {
    this.initializeListComponent();
  }

  initializeListComponent() {
    this.isLoading = true;
    this.route.data.subscribe(
      (data: Data) => {
        this.data = data.items;
        this.isLoading = false;
      },
      (error) => {
        this.isLoading = false;
        const message = typeof error.error === 'string' ? error.error :
          typeof error.error === 'object' && error.error.title != null ? error.error.title : 'Oops. Something went wrong';
        this.snackBar.open(message, 'OK', { duration: 5000 });
      });
  }

  rowClicked(id) {
    this.router.navigate(['../', 'item', id], {relativeTo: this.route});
  }

  fullNameFormatter(field: string, data: Object, column: Object): string {
    const fullName = ((data as Item).user != null) ?
    (data as Item).user.firstName + ' ' + (data as Item).user.lastName : ' ';
    return fullName;
  }

}
