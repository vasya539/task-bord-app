import {User} from './user';
import {UserRole} from './user-role';

export class ProjectMember extends User {
  role: UserRole;
}
