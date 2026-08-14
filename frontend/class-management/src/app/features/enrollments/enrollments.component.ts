import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../core/services/api.service';
import { ClassDto, EnrollmentDto, UserDto } from '../../core/models/models';

@Component({
  selector: 'app-enrollments',
  standalone: true,
  imports: [
    NgIf, NgFor, DatePipe, ReactiveFormsModule, MatCardModule, MatFormFieldModule,
    MatSelectModule, MatButtonModule, MatTableModule, MatIconModule
  ],
  templateUrl: './enrollments.component.html',
  styleUrl: './enrollments.component.scss'
})
export class EnrollmentsComponent implements OnInit {
  enrollments: EnrollmentDto[] = [];
  classes: ClassDto[] = [];
  students: UserDto[] = [];
  columns = ['className', 'studentName', 'enrolledAt', 'actions'];
  message = '';
  error = '';

  form = this.fb.nonNullable.group({
    classId: ['', Validators.required],
    studentId: ['', Validators.required]
  });

  constructor(private fb: FormBuilder, private api: ApiService) {}

  ngOnInit(): void {
    this.load();
    this.api.getClasses().subscribe((r) => (this.classes = r.data?.items ?? []));
    this.api.getUsersByRole('Student').subscribe({
      next: (r) => (this.students = r.data ?? []),
      error: (e) => (this.error = e?.error?.message || 'Failed to load students.')
    });
  }

  load(): void {
    this.api.getEnrollments().subscribe({
      next: (r) => (this.enrollments = r.data?.items ?? []),
      error: (e) => (this.error = e?.error?.message || 'Failed to load enrollments.')
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.api.createEnrollment(this.form.getRawValue()).subscribe({
      next: (r) => {
        this.message = r.message || 'Student enrolled.';
        this.form.reset({ classId: '', studentId: '' });
        this.load();
      },
      error: (e) => (this.error = e?.error?.message || 'Enrollment failed.')
    });
  }

  remove(id: string): void {
    if (!confirm('Remove enrollment?')) return;
    this.api.deleteEnrollment(id).subscribe({
      next: () => this.load(),
      error: (e) => (this.error = e?.error?.message || 'Delete failed.')
    });
  }
}
