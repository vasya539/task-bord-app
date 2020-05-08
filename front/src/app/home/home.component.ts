import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user/user.service';
import { AuthService } from '../services/auth.service';
import { User } from '../models/user';
import { Project } from '../models/project';
import { MainService } from '../services/main.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    user: User;
    myProjects: Project[];

    constructor(
        private userService: UserService,
        private authService: AuthService,
        private router: Router,
        public route: ActivatedRoute) { }

    ngOnInit(): void {
        this.loadMe();
        this.loadMyProjects();
        //this.router.navigate(['/user', localStorage.getItem('uName')]);
    }

    loadMe() {
        this.userService.getUserById(localStorage.getItem('loginedUserId')).subscribe(
            user => {
                this.user = user;
                // this.mainService.userName = user.userName;
                // this.mainService.userLogined = true;
                // this.mainService.projectSelected = false;
                // this.mainService.selectedProjectName = 'Unselected';
                // this.mainService.currentUrl = this.router.url;
                // console.log('MAIN SERVICE USERNAME : ', this.mainService.userName);
            },
            err => {
                console.log('error when loading user in home.');
            }
        );
    }

    loadMyProjects() {
        this.userService.getMyProjects().subscribe(
            projs => {
                this.myProjects = projs;
            },
            err => {
                console.log('error occured when my projects loading.');
            }
        );
    }

    goToProjects() {
        this.router.navigate(['/user', this.user.userName, 'projects']);
    }

    itIsMyProject(p: Project): boolean {
        return p.owner.id == localStorage.getItem('loginedUserId');
    }

    logout() {
        this.authService.logout();
    }
}
