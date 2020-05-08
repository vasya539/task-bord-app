import { Component, OnInit, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  @Output() oneChangeNameFilter = new EventEmitter<string>();

  nameFilter: string;
  minLength = 4;
  constructor() { }

  ngOnInit() {
  }

  changeFilter(filter) {

    if (this.countMarkSpace(filter) > 1 ) {
      filter = filter.trim();
      while (this.countMarkSpace(filter) > 1) {
        filter = filter.replace(' ', '');
      }
    }
    filter = filter.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, '');
    this.nameFilter = filter;
    this.oneChangeNameFilter.emit(filter);
  }
  countMarkSpace(str: string): number {
    const arr = str.split('');
    const count = arr.filter(ch => ch === ' ').length;
    this.minLength = 4 + count;
    return count;

  }
  clearFilter() {
    this.nameFilter = '';
    this.oneChangeNameFilter.emit(this.nameFilter);
  }

}
