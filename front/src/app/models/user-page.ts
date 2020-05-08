import { Page } from "./page";
import { User } from "./user";

export class UserPage {
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  items: User[];
}
