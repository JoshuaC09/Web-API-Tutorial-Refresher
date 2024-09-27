import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RoleComponent } from './components/role/role.component';
import { UserComponent } from './components/user/user.component';
import { StudentComponent } from './components/student/student.component';
import { RoleService } from './services/role.service';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { RolePrivilegeComponent } from './components/role-privilege/role-privilege.component';

@NgModule({ declarations: [
        AppComponent,
        RoleComponent,
        UserComponent,
        StudentComponent,
        RolePrivilegeComponent
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        AppRoutingModule,
        ReactiveFormsModule], providers: [
        RoleService,
        provideHttpClient(withInterceptorsFromDi()),
    ] })
export class AppModule { }
