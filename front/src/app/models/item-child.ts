import {Status} from './status';
import {ItemType} from './item-type';

export class ItemChild {
  public id: number;
  public name: string;
  public statusId: number;
  public typeId: number;
  public status: Status;
  public itemType: ItemType;
}
