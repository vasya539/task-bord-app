import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-bad-request',
  templateUrl: './bad-request.component.html',
  styleUrls: ['./bad-request.component.css']
})
export class BadRequestComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void {
  }
  homePageClicked() {
    this.router.navigate(['/home']);
  }
}
