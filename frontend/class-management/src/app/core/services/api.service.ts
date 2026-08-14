import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  AttendanceDto,
  AttendanceInsight,
  ClassDto,
  EnrollmentDto,
  MarkDto,
  PagedResult,
  UserDto
} from '../models/models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly base = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Users
  getUsers(page = 1, pageSize = 20, role?: string): Observable<ApiResponse<PagedResult<UserDto>>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (role) params = params.set('role', role);
    return this.http.get<ApiResponse<PagedResult<UserDto>>>(`${this.base}/users`, { params });
  }

  getUsersByRole(role: string): Observable<ApiResponse<UserDto[]>> {
    return this.http.get<ApiResponse<UserDto[]>>(`${this.base}/users/by-role/${role}`);
  }

  createUser(body: { email: string; password: string; fullName: string; role: string }): Observable<ApiResponse<UserDto>> {
    return this.http.post<ApiResponse<UserDto>>(`${this.base}/users`, body);
  }

  deleteUser(id: string): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.base}/users/${id}`);
  }

  // Classes
  getClasses(page = 1, pageSize = 50): Observable<ApiResponse<PagedResult<ClassDto>>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<ApiResponse<PagedResult<ClassDto>>>(`${this.base}/classes`, { params });
  }

  createClass(body: { name: string; subject: string; teacherId: string; description?: string }): Observable<ApiResponse<ClassDto>> {
    return this.http.post<ApiResponse<ClassDto>>(`${this.base}/classes`, body);
  }

  deleteClass(id: string): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.base}/classes/${id}`);
  }

  // Enrollments
  getEnrollments(page = 1, pageSize = 50, classId?: string): Observable<ApiResponse<PagedResult<EnrollmentDto>>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (classId) params = params.set('classId', classId);
    return this.http.get<ApiResponse<PagedResult<EnrollmentDto>>>(`${this.base}/enrollments`, { params });
  }

  createEnrollment(body: { classId: string; studentId: string }): Observable<ApiResponse<EnrollmentDto>> {
    return this.http.post<ApiResponse<EnrollmentDto>>(`${this.base}/enrollments`, body);
  }

  deleteEnrollment(id: string): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.base}/enrollments/${id}`);
  }

  // Attendance
  getAttendance(page = 1, pageSize = 50, classId?: string): Observable<ApiResponse<PagedResult<AttendanceDto>>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (classId) params = params.set('classId', classId);
    return this.http.get<ApiResponse<PagedResult<AttendanceDto>>>(`${this.base}/attendance`, { params });
  }

  markAttendance(body: {
    classId: string;
    studentId: string;
    date: string;
    isPresent: boolean;
    notes?: string;
  }): Observable<ApiResponse<AttendanceDto>> {
    return this.http.post<ApiResponse<AttendanceDto>>(`${this.base}/attendance`, body);
  }

  // Marks
  getMarks(page = 1, pageSize = 50, classId?: string): Observable<ApiResponse<PagedResult<MarkDto>>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (classId) params = params.set('classId', classId);
    return this.http.get<ApiResponse<PagedResult<MarkDto>>>(`${this.base}/marks`, { params });
  }

  createMark(body: {
    classId: string;
    studentId: string;
    examName: string;
    score: number;
    maxScore: number;
  }): Observable<ApiResponse<MarkDto>> {
    return this.http.post<ApiResponse<MarkDto>>(`${this.base}/marks`, body);
  }

  generateMarksFeedback(markId: string): Observable<ApiResponse<{ feedback: string; performanceLevel: string }>> {
    return this.http.post<ApiResponse<{ feedback: string; performanceLevel: string }>>(
      `${this.base}/ai/marks-feedback/${markId}`,
      {}
    );
  }

  // AI Insights
  getAttendanceInsights(classId?: string): Observable<ApiResponse<AttendanceInsight>> {
    let params = new HttpParams();
    if (classId) params = params.set('classId', classId);
    return this.http.get<ApiResponse<AttendanceInsight>>(`${this.base}/ai/attendance-insights`, { params });
  }
}
