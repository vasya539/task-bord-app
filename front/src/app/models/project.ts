import { Sprint } from './sprint';
import { ProjectsUsers } from './projects-users';

import { User } from './user';

export class Project {
  public id: number;
  public name: string;
  public description: string;
  public owner: User;
  public sprints: Sprint [];
  public projectsUsers: ProjectsUsers [];
}
