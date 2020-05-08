import { Item } from './item';

export class Sprint {
  public id: number;
  public name: string;
  public description: string;
  public projectId: number;
  public items: Item[];
  public startDate: Date;
  public endDate: Date;
}
