import { SprintService } from 'src/app/services/sprint.service';
import { Sprint } from 'src/app/models/sprint';

import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import {Location} from '@angular/common';
import { HostListener } from '@angular/core';


@Component({
  selector: 'app-sprint-details',
  templateUrl: './sprint-details.component.html',
  styleUrls: ['./sprint-details.component.css']
})
export class SprintDetailsComponent implements OnInit {
  private selectedSprintId: number;
  public sprint: Sprint;
  private snackBar: MatSnackBar;
  public status: string;
  public duration: number;

  constructor(private sprintService: SprintService,
              private router: Router,
              private route: ActivatedRoute,
              private location: Location) { }

  ngOnInit(): void {
    this.route.params.subscribe(
      (params: Params) => {
        this.sprint = new Sprint();
        this.sprintService.formLoaded.next(true);
        this.selectedSprintId = params['sprintId'];
        this.sprintService.getSprint(this.selectedSprintId).subscribe(sprint => {
            this.sprint = sprint;
            this.status = this.getCurrentStatus();
            this.duration = this.getDuration();
          }, error => {
            const message = typeof error.error === 'string' ? error.error :
              typeof error.error === 'object' && error.error.title != null ? error.error.title : 'Oops. Something went wrong';
            this.snackBar.open(message, 'OK', { duration: 5000 });
          });
      });
  }
  OnBackClick() {
    this.sprintService.formLoaded.next(false);
    this.location.back();
  }
  OnChangeClick() {
    this.router.navigate(['../', 'edit'], { relativeTo: this.route });
  }
  getCurrentStatus(): string {
    const date = Date.now();
    const startDate = Date.parse(this.sprint.startDate.toString());
    const endDate = Date.parse(this.sprint.endDate.toString());
    if (date < startDate) {
      return 'planned';
    } else if (date >= startDate && date <= endDate) {
      return 'current';
    } else {
      return 'completed';
    }
  }
  getDuration(): number {
    const startDate = Date.parse(this.sprint.startDate.toString());
    const endDate = Date.parse(this.sprint.endDate.toString());
    const duration = Math.round((endDate - startDate) / (1000 * 60 * 60 * 24));
    return duration;
  }

  @HostListener('window:popstate', ['$event'])
  onPopState(event) {
    this.sprintService.formLoaded.next(false);
  }

}
