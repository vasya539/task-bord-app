import { ProjectService } from './services/project.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, Component } from '@angular/core';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { AppComponent } from './app.component';
import { ItemsComponent } from './task-board/items/items.component';
import { ItemService } from './services/item.service';
import { ColumnComponent } from './task-board/column/column.component';
import { BoardComponent } from './task-board/board/board.component';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MaterialAppsModule } from './ngmaterial.module';
import { CreateItemDialogComponent } from './task-board/create-dialog/create-dialog.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HeaderComponent } from './navigation/header/header.component';
import { PropertiesDialogComponent } from './task-board/properties-dialog/properties-dialog.component';
import { ArchivedItemsComponent } from './task-board/archived-items/archived-items.component';
import { CommentService } from './services/comment.service';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ItemDetailComponent } from './task-board/item-detail/item-detail.component';
import { StoryComponent } from './task-board/story/story.component';
import { LoginComponent } from './account/login/login.component';
import { RegisterComponent } from './account/register/register.component';
import { HomeComponent } from './home/home.component';
import { AccountComponent } from './account/account.component';
import { UserIcoComponent } from './user/user-ico/user-ico.component';
import { UserComponent } from './user/user.component';
import { UserListComponent } from './user/user-list/user-list.component';
import { AuthGuard } from './auth/auth.guard';
import { UserService } from './services/user/user.service';
import { ProfileComponent } from './profile/profile.component';
import { SprintListComponent } from './sprints/sprint-list/sprint-list.component';
import { SprintService } from './services/sprint.service';
import { SprintDialogComponent } from './sprints/sprint-dialog/sprint-dialog.component';
import { SprintsComponent } from './sprints/sprints.component';
import { LoadedGuard } from './sprints/loaded-guard.service';
import { SprintDetailsComponent } from './sprints/sprint-details/sprint-details.component';
import { ConfirmDialogComponent } from './shared/confirm-dialog/confirm-dialog.component';
import { ProjectListComponent } from './projects/project-list/project-list.component';
import { ProjectDetailsComponent } from './projects/project-details/project-details.component';
import { ProfileSettingsComponent } from './profile/profile-settings/profile-settings.component';
import { ProjectDialogComponent } from './projects/project-dialog/project-dialog.component';
import { LeaveProfileSettingsGuard } from './profile/profile-settings/can-leave/leave.guard';
import { SearchComponent } from './search/search.component';
import { SearchUsersComponent } from './projects/search-users/search-users.component';
import { HighlightPipe } from './pipes/highlight.pipe';

import { AuthService } from './services/auth.service';
import { TokenService } from './services/token.service';
import { UserInfoService } from './services/user-info.service';
import { TokenInterceptor } from './interceptor/token-interceptor.interceptor';
import { LoginDialogComponent } from './account/dialogs/login/login.component';
import { RegisterDialogComponent } from './account/dialogs/register/register.component';
import { MemberService } from './services/member.service';
import { TreeGridModule, SortService, FilterService } from '@syncfusion/ej2-angular-treegrid';
import { SprintListResolver } from './sprints/sprint-list/sprint-list-resolver.service';
import { ForbiddenComponent } from './shared/forbidden/forbidden.component';
import { PageNotFoundComponent } from './shared/page-not-found/page-not-found.component';
import { UsersOnProjectComponent } from './projects/project-details/users-on-project/users-on-project.component';
import { AddUserToProjectFormComponent } from './projects/project-details/add-user-to-project-form/add-user-to-project-form.component';
import { InternalServerErrorComponent } from './shared/internal-server-error/internal-server-error.component';
import { BadRequestComponent } from './shared/bad-request/bad-request.component';
import { ForgetComponent } from './account/forget/forget.component';
import { ResetComponent } from './account/reset/reset.component';
import { AdminGuard } from './account/admin/admin.guard';
import { AccountService } from './services/account.service';

import { CreateLinkDialogComponent } from './task-board/create-link-dialog/create-link-dialog.component';
import { AdminComponent } from './account/admin/admin.component';
import { AdminUserComponent } from './account/admin/user/admin-user.component';
import { UserCPComponent } from './account/admin/user/user-read.component';
import { UserAdminListComponent } from './account/admin/user/user-list.component';
import { UserCreateComponent } from './account/admin/user/user-create.cpmponent';
import { UserRenameComponent } from './account/admin/user/user-edit.component';
import { UserDetailsComponent } from './account/admin/user/user-details.component';
import { HomeAdminComponent } from './account/admin/home/home.component';

@NgModule({
  declarations: [
    
    SearchComponent,
    SearchUsersComponent,
    HighlightPipe,
    
    AppComponent,
    CreateItemDialogComponent,
    ItemsComponent,
    ColumnComponent,
    BoardComponent,
    HeaderComponent,
    PropertiesDialogComponent,
    ArchivedItemsComponent,
    ItemDetailComponent,
    StoryComponent,
    
    ForgetComponent,
    ResetComponent,
    LoginDialogComponent,
    RegisterDialogComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    AccountComponent,
    UserIcoComponent,
    UserComponent,
    UserListComponent,
    ProfileSettingsComponent,
    ProjectDetailsComponent,
    SprintListComponent,
    SprintDialogComponent,
    SprintsComponent,
    SprintDetailsComponent,
    ConfirmDialogComponent,
    
    ProjectListComponent,
    ProjectDetailsComponent,
    ProjectDialogComponent,
    ProfileComponent,
    
    SprintListComponent,
    SprintDialogComponent,
    SprintsComponent,
    SprintDetailsComponent,
    ConfirmDialogComponent,
    ForbiddenComponent,
    PageNotFoundComponent,
    UsersOnProjectComponent,
    AddUserToProjectFormComponent,
    InternalServerErrorComponent,
    BadRequestComponent,
    CreateLinkDialogComponent,
    
    
    AdminComponent,
    HomeAdminComponent,
    AdminUserComponent,
    UserCPComponent,
    UserCreateComponent,
    UserAdminListComponent,
    UserRenameComponent,
    UserDetailsComponent
  ],
  imports: [
    //AdminModule,
    BrowserModule,
    MaterialAppsModule,
    DragDropModule,
    FormsModule,
    MatCheckboxModule,
    ReactiveFormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    TreeGridModule,
    
    RouterModule.forRoot([
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'forbidden', component: ForbiddenComponent },
      { path: 'home', component: ProfileComponent, canActivate: [AuthGuard], data: { breadcrumbLabel: 'Home Page' } },
      { path: 'user/:userName/settings', component: ProfileSettingsComponent, canActivate: [AuthGuard], canDeactivate: [LeaveProfileSettingsGuard] },
      { path: 'user/:userName', component: ProfileComponent, canActivate: [AuthGuard] },
      { path: 'projects', component: ProjectListComponent },
      { path: 'projects/:projectId', component: ProjectDetailsComponent },
      { path: 'user/:userName/projects', component: ProjectListComponent },
      { path: 'dialog', component: ProjectDialogComponent },
      { path: 'user/:userName/projects/:projectId', component: ProjectDetailsComponent },
      { path: 'forget', component: ForgetComponent, data: { breadcrumbLabel: 'Restore Password' } },
      { path: 'reset', component: ResetComponent, data: { breadcrumbLabel: 'Change Password' } },
      
      {
        path: 'account', component: AccountComponent,
        children: [
          { path: 'login', component: LoginComponent, data: { breadcrumbLabel: 'Login' } },
          { path: 'register', component: RegisterComponent, data: { breadcrumbLabel: 'Register' } }
        ]
      },
      
      { path: 'user/:userName/projects/:projectId/sprints/:sprintId/board', component: BoardComponent },
      { path: 'user/:userName/projects/:projectId/sprints/:sprintId/archived', component: ArchivedItemsComponent },
      { path: 'user/:userName/projects/:projectId/sprints/:sprintId/item/:id', component: ItemDetailComponent },
      
      {
        path: 'user/:userName/projects/:projectId/sprints', component: SprintsComponent, canActivate: [AuthGuard], children: [
          { path: 'new', component: SprintDialogComponent, canDeactivate: [LoadedGuard] },
          { path: ':sprintId/details', component: SprintDetailsComponent, canDeactivate: [LoadedGuard] },
          { path: ':sprintId/list', component: SprintListComponent, resolve: { items: SprintListResolver }, canDeactivate: [LoadedGuard] },
          { path: ':sprintId/edit', component: SprintDialogComponent, canDeactivate: [LoadedGuard] }
          
        ]
      },
      {
        path: 'admin', component: AdminComponent, canActivate: [AdminGuard],
        //loadChildren: () => AdminModule,
        children:[
          { path: '', component: HomeAdminComponent, data: { breadcrumbIgnore: true } },
          { path: 'users', component: AdminUserComponent, data: { breadcrumbLabel: 'Users Management' }, 
          children: [
            { path: '', component: UserAdminListComponent, data: { breadcrumbIgnore: true } },
            { path: 'create', component: UserCreateComponent, data: { breadcrumbLabel: 'Create Admin' } },
            { 
              path: ':id', component: UserDetailsComponent, data: { breadcrumbLabel: 'User' }, 
              children: [
                { path: '', component: UserCPComponent, data: { breadcrumbIgnore: true } },
                { path: 'rename', component: UserRenameComponent, data: { breadcrumbLabel: 'Rename' } },
              ]
            },
          ]
      },],
              canLoad: [AdminGuard],
              data: { breadcrumbLabel: 'Admin' }
            },
            {path: 'error', component: PageNotFoundComponent},
            {path: 'bad_request', component: BadRequestComponent},
            {path: 'internal_server_error', component: InternalServerErrorComponent},
            {path: '**', component: PageNotFoundComponent}
          ])
        ],
        providers: [
          AccountService,
          AdminGuard,
          ProjectService,
          UserInfoService,
          AuthService,
          MemberService,
          TokenService,
          ItemService,
          UserService,
          SprintService,
          CommentService,
          LoadedGuard,
          LeaveProfileSettingsGuard,
          {
            provide: HTTP_INTERCEPTORS,
            useClass: TokenInterceptor,
            multi: true
          }],
          bootstrap: [AppComponent],
          entryComponents: [CreateItemDialogComponent],
          exports:
          [
            MaterialAppsModule,
            ReactiveFormsModule
          ]
        })
        export class AppModule {
        }
