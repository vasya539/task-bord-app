import {User} from './user';

export class Commentary {
  public id: number;
  public itemId: number;
  public userId: string;
  public text: string;
  public user: User;
  public date: Date;
}
