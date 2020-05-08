import {Status} from './status';
import {ItemType} from './item-type';
import {User} from './user';

export class Item {
  public id: number;
  public sprintId: number;
  public assignedUserId: string;
  public parentId: number;
  public name: string;
  public description: string;
  public statusId: number;
  public typeId: number;
  public isArchived: boolean;
  public items?: Item[];
  public status: Status;
  public itemType: ItemType;
  public parent: Item;
  public user: User;
  public storyPoint: number;

}
