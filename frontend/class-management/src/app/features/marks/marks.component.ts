import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { ClassDto, EnrollmentDto, MarkDto } from '../../core/models/models';

@Component({
  selector: 'app-marks',
  standalone: true,
  imports: [
    NgIf, NgFor, DatePipe, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatSelectModule,
    MatInputModule, MatButtonModule, MatTableModule, MatIconModule
  ],
  templateUrl: './marks.component.html',
  styleUrl: './marks.component.scss'
})
export class MarksComponent implements OnInit {
  marks: MarkDto[] = [];
  classes: ClassDto[] = [];
  enrollments: EnrollmentDto[] = [];
  columns = ['examName', 'className', 'studentName', 'score', 'percentage', 'feedback', 'actions'];
  message = '';
  error = '';
  canManage = false;

  form = this.fb.nonNullable.group({
    classId: ['', Validators.required],
    studentId: ['', Validators.required],
    examName: ['', [Validators.required, Validators.minLength(2)]],
    score: [0, [Validators.required, Validators.min(0)]],
    maxScore: [100, [Validators.required, Validators.min(1)]]
  });

  constructor(private fb: FormBuilder, private api: ApiService, public auth: AuthService) {
    this.canManage = this.auth.hasRole('Admin', 'Teacher');
  }

  ngOnInit(): void {
    this.load();
    this.api.getClasses().subscribe((r) => (this.classes = r.data?.items ?? []));
    this.form.controls.classId.valueChanges.subscribe((classId) => {
      if (!classId) {
        this.enrollments = [];
        return;
      }
      this.api.getEnrollments(1, 100, classId).subscribe((r) => (this.enrollments = r.data?.items ?? []));
    });
  }

  load(): void {
    this.api.getMarks().subscribe({
      next: (r) => (this.marks = r.data?.items ?? []),
      error: (e) => (this.error = e?.error?.message || 'Failed to load marks.')
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.api.createMark(this.form.getRawValue()).subscribe({
      next: (r) => {
        this.message = r.message || 'Mark recorded.';
        this.form.patchValue({ examName: '', score: 0, maxScore: 100 });
        this.load();
      },
      error: (e) => (this.error = e?.error?.message || 'Failed to save mark.')
    });
  }

  generateFeedback(markId: string): void {
    this.api.generateMarksFeedback(markId).subscribe({
      next: (r) => {
        this.message = r.data?.feedback || 'Feedback generated.';
        this.load();
      },
      error: (e) => (this.error = e?.error?.message || 'AI feedback failed.')
    });
  }
}
