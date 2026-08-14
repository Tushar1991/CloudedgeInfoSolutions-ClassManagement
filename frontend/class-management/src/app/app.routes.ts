import { Routes } from '@angular/router';
import { authGuard, roleGuard } from './core/guards/auth.guard';
import { ShellComponent } from './layout/shell.component';
import { LoginComponent } from './features/login/login.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { UsersComponent } from './features/users/users.component';
import { ClassesComponent } from './features/classes/classes.component';
import { EnrollmentsComponent } from './features/enrollments/enrollments.component';
import { AttendanceComponent } from './features/attendance/attendance.component';
import { MarksComponent } from './features/marks/marks.component';
import { InsightsComponent } from './features/insights/insights.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'users', component: UsersComponent, canActivate: [roleGuard('Admin')] },
      { path: 'classes', component: ClassesComponent },
      { path: 'enrollments', component: EnrollmentsComponent, canActivate: [roleGuard('Admin', 'Teacher')] },
      { path: 'attendance', component: AttendanceComponent },
      { path: 'marks', component: MarksComponent },
      { path: 'insights', component: InsightsComponent, canActivate: [roleGuard('Admin', 'Teacher')] }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
