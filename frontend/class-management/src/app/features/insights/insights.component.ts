import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ApiService } from '../../core/services/api.service';
import { AttendanceInsight, ClassDto } from '../../core/models/models';

@Component({
  selector: 'app-insights',
  standalone: true,
  imports: [
    NgIf, NgFor, NgClass, DatePipe, ReactiveFormsModule, MatCardModule, MatFormFieldModule,
    MatSelectModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule
  ],
  templateUrl: './insights.component.html',
  styleUrl: './insights.component.scss'
})
export class InsightsComponent implements OnInit {
  classes: ClassDto[] = [];
  insight: AttendanceInsight | null = null;
  loading = false;
  error = '';

  form = this.fb.nonNullable.group({
    classId: ['']
  });

  constructor(private fb: FormBuilder, private api: ApiService) {}

  ngOnInit(): void {
    this.api.getClasses().subscribe((r) => (this.classes = r.data?.items ?? []));
    this.analyze();
  }

  analyze(): void {
    this.loading = true;
    this.error = '';
    const classId = this.form.value.classId || undefined;
    this.api.getAttendanceInsights(classId).subscribe({
      next: (r) => {
        this.loading = false;
        this.insight = r.data ?? null;
      },
      error: (e) => {
        this.loading = false;
        this.error = e?.error?.message || 'Failed to generate insights.';
      }
    });
  }
}
