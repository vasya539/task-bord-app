import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-internal-server-error',
  templateUrl: './internal-server-error.component.html',
  styleUrls: ['./internal-server-error.component.css']
})
export class InternalServerErrorComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void {
  }
  homePageClicked() {
    this.router.navigate(['/home']);
  }
}
