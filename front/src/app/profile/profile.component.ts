import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { UserService } from '../services/user/user.service';
import { User } from '../models/user';
import { Project } from '../models/project';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  //userName: string;
  //@Input()
  user: User;
  //@Input()
  //showUserInfo: boolean = true;

  userProjects: Project[];

  constructor(private route: ActivatedRoute, private router: Router, private userService: UserService) { }

  onUrlBarChanged(p: Params): void {
    if(this.router.url.startsWith('/home'))
      this.router.navigate(['/user', localStorage.getItem('uName')]);
      
    if(this.router.url.startsWith('/user/')) {
      let userName = p['userName'];

      this.userService.getDetailedUserByUserName(userName).subscribe(
        user => {
          this.onDataLoaded(user);
          this.loadUserProjects();
        },
        err => this.onError(err)
      );
    }
  }

  loadUserProjects(): void {
    this.userService.getProjectsOfUser(this.user.id).subscribe(
      projs => {
        this.userProjects = projs;
      },
      err => {
        console.log('err when loading projects.');
      }
    );
  }

  onDataLoaded(user: User): void {
    this.user = user;
  }

  onError(err: any): void {
    if(err.status == 404)
      alert('user-profile not found');
    else
      console.log('error while user-profile getting. err: ', err);
  }

  ngOnInit(): void {
    this.route.params.subscribe(p => {
      this.onUrlBarChanged(p);
    });
  }

  itIsMe(): boolean {
    return this.user.userName == localStorage.getItem('uName');
  }

  goToProjects() {
    this.router.navigate(['/user', this.user.userName, 'projects']);
  }

  itIsProjectOfProfileUser(p: Project): boolean {
    return p.owner.id == this.user.id;
  }

  itIsMyProject(p: Project): boolean {
    return p.owner.id == localStorage.getItem('loginedUserId');
  }

}
