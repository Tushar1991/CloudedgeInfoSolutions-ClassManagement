import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTableModule } from '@angular/material/table';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { AttendanceDto, ClassDto, EnrollmentDto } from '../../core/models/models';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [
    NgIf, NgFor, DatePipe, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatSelectModule,
    MatInputModule, MatButtonModule, MatCheckboxModule, MatTableModule
  ],
  templateUrl: './attendance.component.html',
  styleUrl: './attendance.component.scss'
})
export class AttendanceComponent implements OnInit {
  records: AttendanceDto[] = [];
  classes: ClassDto[] = [];
  enrollments: EnrollmentDto[] = [];
  columns = ['date', 'className', 'studentName', 'isPresent', 'notes'];
  message = '';
  error = '';
  canMark = false;

  form = this.fb.nonNullable.group({
    classId: ['', Validators.required],
    studentId: ['', Validators.required],
    date: [new Date().toISOString().substring(0, 10), Validators.required],
    isPresent: [true],
    notes: ['']
  });

  constructor(private fb: FormBuilder, private api: ApiService, public auth: AuthService) {
    this.canMark = this.auth.hasRole('Admin', 'Teacher');
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
    this.api.getAttendance().subscribe({
      next: (r) => (this.records = r.data?.items ?? []),
      error: (e) => (this.error = e?.error?.message || 'Failed to load attendance.')
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.api.markAttendance({
      classId: value.classId,
      studentId: value.studentId,
      date: new Date(value.date).toISOString(),
      isPresent: value.isPresent,
      notes: value.notes || undefined
    }).subscribe({
      next: (r) => {
        this.message = r.message || 'Attendance marked.';
        this.load();
      },
      error: (e) => (this.error = e?.error?.message || 'Failed to mark attendance.')
    });
  }
}
