import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { ClassDto, UserDto } from '../../core/models/models';

@Component({
  selector: 'app-classes',
  standalone: true,
  imports: [
    NgIf, NgFor, DatePipe, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatTableModule, MatIconModule
  ],
  templateUrl: './classes.component.html',
  styleUrl: './classes.component.scss'
})
export class ClassesComponent implements OnInit {
  classes: ClassDto[] = [];
  teachers: UserDto[] = [];
  columns = ['name', 'subject', 'teacherName', 'createdAt', 'actions'];
  message = '';
  error = '';
  canManage = false;

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    subject: ['', [Validators.required, Validators.minLength(2)]],
    teacherId: ['', Validators.required],
    description: ['']
  });

  constructor(private fb: FormBuilder, private api: ApiService, public auth: AuthService) {
    this.canManage = this.auth.hasRole('Admin', 'Teacher');
  }

  ngOnInit(): void {
    this.load();
    if (this.canManage) {
      this.api.getUsersByRole('Teacher').subscribe({
        next: (r) => {
          this.teachers = r.data ?? [];
          if (this.auth.hasRole('Teacher') && this.auth.user()) {
            this.form.patchValue({ teacherId: this.auth.user()!.id });
          }
        },
        error: () => {
          // Admin-only endpoint fallback: teachers can assign themselves
          if (this.auth.hasRole('Teacher') && this.auth.user()) {
            this.teachers = [{
              id: this.auth.user()!.id,
              email: this.auth.user()!.email,
              fullName: this.auth.user()!.fullName,
              role: 'Teacher',
              isActive: true,
              createdAt: ''
            }];
            this.form.patchValue({ teacherId: this.auth.user()!.id });
          }
        }
      });
    }
  }

  load(): void {
    this.api.getClasses().subscribe({
      next: (r) => (this.classes = r.data?.items ?? []),
      error: (e) => (this.error = e?.error?.message || 'Failed to load classes.')
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.api.createClass(this.form.getRawValue()).subscribe({
      next: (r) => {
        this.message = r.message || 'Class created.';
        this.form.patchValue({ name: '', subject: '', description: '' });
        this.load();
      },
      error: (e) => (this.error = e?.error?.message || 'Create failed.')
    });
  }

  remove(id: string): void {
    if (!this.auth.hasRole('Admin')) return;
    if (!confirm('Delete this class?')) return;
    this.api.deleteClass(id).subscribe({
      next: () => this.load(),
      error: (e) => (this.error = e?.error?.message || 'Delete failed.')
    });
  }
}
