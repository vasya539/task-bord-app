import { Sprint } from '../models/sprint';
import { SprintService } from '../services/sprint.service';
import { ConfirmDialogModel, ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';

import { Component, OnInit, ViewChild, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatFormField } from '@angular/material/form-field';
import { Subscription, Observable } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
    selector: 'app-sprints',
    templateUrl: './sprints.component.html',
    styleUrls: ['./sprints.component.css']
})
export class SprintsComponent implements OnInit, OnDestroy {
    public sprints: Sprint[] = [];
    public selectedSprint: Sprint;
    public selectedSprintId: number;
    public disabled: boolean = false;
    public formLoadedSub: Subscription;
    public spritDataModifiedSub: Subscription;
    public result: boolean;
    public detailsLoading: boolean = false;
    @ViewChild('formField', { static: false }) private formField: MatFormField;

    constructor(private sprintService: SprintService,
                private router: Router,
                private route: ActivatedRoute,
                private changeDetector: ChangeDetectorRef,
                public dialog: MatDialog,
                private snackBar: MatSnackBar) {}

    ngOnInit(): void {
        this.formLoadedSub = this.sprintService.formLoaded
            .subscribe(loaded => { this.disabled = loaded; this.changeDetector.detectChanges(); });
        this.route.params.subscribe(params => {
            const lastChild = this.getRouteLastChild(this.route);
            this.sprintService.projectId = +params.projectId;
            if (this.router.url.indexOf('details') > -1 ||
                this.router.url.indexOf('edit') > -1) { this.detailsLoading = true; }
            this.selectedSprintId = lastChild && lastChild.params.value.sprintId ? +lastChild.params.value.sprintId : -1;
            this.spritDataModifiedSub = this.sprintService.sprintDataModified
                .subscribe(sprint => {
                    this.LoadSprints(this.selectedSprintId);
                });
            this.LoadSprints(this.selectedSprintId);
        });
    }

    ngOnDestroy(): void {
        this.formLoadedSub.unsubscribe();
        this.spritDataModifiedSub.unsubscribe();
    }

    LoadSprints(selectedId: number) {
        const lodedSprints = this.sprintService.getSprintsByProjectId(this.sprintService.projectId).subscribe((sprints: Sprint[]) => {
            this.sprints = sprints;
            console.log(this.sprints);
            this.selectedSprint = this.sprints[0];
            this.selectedSprintId = this.sprints[0].id;
            sprints.forEach(sprint => {
                if (sprint.id === selectedId) {
                    this.selectedSprintId = sprint.id;
                    this.selectedSprint = sprint;
                }
            });
            if (!this.detailsLoading) { this.LoadSprintItems(this.selectedSprint); }
        });
    }

    LoadSprintItems(sprint: Sprint) {
        this.selectedSprintId = sprint.id;
        this.selectedSprint = sprint;
        this.router.navigate([sprint.id, 'list'], { relativeTo: this.route });
        this.sprintService.sprintChanged.emit(sprint.id);
    }

    async confirmDialog() {
        const message = `Are you sure you want to delete the sprint?`;

        const dialogData = new ConfirmDialogModel('Confirm Action', message);

        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            maxWidth: '400px',
            data: dialogData
        });

        this.result = await dialogRef.afterClosed().toPromise();
    }

    OnEditClick(sprint: Sprint) {
        this.router.navigate([this.selectedSprintId, 'edit'], { relativeTo: this.route });
    }
    OnNewSprintClick() {
        this.router.navigate(['new'], { relativeTo: this.route });
    }

    OnBoardViewClick() {
        this.router.navigate([this.selectedSprintId , 'board'], { relativeTo: this.route });
    }

    OnDetailsClick() {
        this.router.navigate([this.selectedSprintId, 'details'], { relativeTo: this.route });
    }

    async OnDeleteClick() {
        await this.confirmDialog();
        if (this.result) {
            this.sprintService.deleteSprint(this.selectedSprintId).subscribe(
                res => {
                    this.LoadSprints(0);
                    this.snackBar.open('Deleted', 'OK');
                }, error => {
                    console.log(error);
                    const message = error.status === 403 ? error.error.message : typeof error.error === 'string' ? error.error :
                        typeof error.error === 'object' && error.error.title != null ? error.error.title : 'Oops. Something went wrong';
                    this.snackBar.open(message, 'OK', { duration: 5000 });
                }
            );
        }
    }
    getRouteLastChild(val) {
        let lastChild = val.firstChild;
        while (lastChild && lastChild.firstChild) {
          lastChild = lastChild.firstChild;
        }
        return lastChild;
    }
}
