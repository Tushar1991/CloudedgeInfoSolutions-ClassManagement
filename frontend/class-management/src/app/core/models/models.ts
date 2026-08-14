export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface UserInfo {
  id: string;
  email: string;
  fullName: string;
  role: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  user: UserInfo;
}

export interface UserDto {
  id: string;
  email: string;
  fullName: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

export interface ClassDto {
  id: string;
  name: string;
  subject: string;
  teacherId: string;
  teacherName: string;
  description?: string;
  createdAt: string;
}

export interface EnrollmentDto {
  id: string;
  classId: string;
  className: string;
  studentId: string;
  studentName: string;
  enrolledAt: string;
}

export interface AttendanceDto {
  id: string;
  classId: string;
  className: string;
  studentId: string;
  studentName: string;
  date: string;
  isPresent: boolean;
  notes?: string;
  createdAt: string;
}

export interface MarkDto {
  id: string;
  classId: string;
  className: string;
  studentId: string;
  studentName: string;
  examName: string;
  score: number;
  maxScore: number;
  percentage: number;
  feedback?: string;
  recordedAt: string;
}

export interface StudentAttendanceRisk {
  studentId: string;
  studentName: string;
  classId: string;
  className: string;
  attendancePercent: number;
  totalSessions: number;
  presentCount: number;
  absentCount: number;
  riskLevel: string;
  warning: string;
  suggestion: string;
}

export interface AttendanceInsight {
  summary: string;
  classAverageAttendancePercent: number;
  totalStudentsAnalyzed: number;
  atRiskCount: number;
  atRiskStudents: StudentAttendanceRisk[];
  suggestions: string[];
  generatedAt: string;
}

export type UserRole = 'Admin' | 'Teacher' | 'Student';
