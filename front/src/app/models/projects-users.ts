import { User } from './user';

export interface ProjectsUsers {
    projectId: number;
    userId: string;
    userRoleId: number;
    user: User;
}
