import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../core/services/api.service';
import { UserDto } from '../../core/models/models';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    NgIf, NgFor, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatTableModule, MatIconModule
  ],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit {
  users: UserDto[] = [];
  columns = ['fullName', 'email', 'role', 'isActive', 'actions'];
  message = '';
  error = '';
  roles = ['Admin', 'Teacher', 'Student'];

  form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: ['Student', Validators.required]
  });

  constructor(private fb: FormBuilder, private api: ApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getUsers().subscribe({
      next: (r) => (this.users = r.data?.items ?? []),
      error: (e) => (this.error = e?.error?.message || 'Failed to load users.')
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.error = '';
    this.message = '';
    this.api.createUser(this.form.getRawValue()).subscribe({
      next: (r) => {
        this.message = r.message || 'User created.';
        this.form.reset({ role: 'Student', fullName: '', email: '', password: '' });
        this.load();
      },
      error: (e) => (this.error = e?.error?.message || 'Create failed.')
    });
  }

  remove(id: string): void {
    if (!confirm('Delete this user?')) return;
    this.api.deleteUser(id).subscribe({
      next: () => this.load(),
      error: (e) => (this.error = e?.error?.message || 'Delete failed.')
    });
  }
}
