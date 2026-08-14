import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../core/services/auth.service';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgIf, RouterLink, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  classCount = 0;
  enrollmentCount = 0;
  markCount = 0;

  constructor(public auth: AuthService, private api: ApiService) {}

  ngOnInit(): void {
    this.api.getClasses().subscribe((r) => (this.classCount = r.data?.totalCount ?? 0));
    this.api.getEnrollments().subscribe((r) => (this.enrollmentCount = r.data?.totalCount ?? 0));
    this.api.getMarks().subscribe((r) => (this.markCount = r.data?.totalCount ?? 0));
  }
}
