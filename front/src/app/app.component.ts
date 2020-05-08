import { Component } from '@angular/core';
import {ItemService} from './services/item.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'IF-106.Net front-end';
  constructor(private itemService: ItemService) {
    this.itemService.loadRelativesParameters();
  }
}
