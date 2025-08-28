import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@/environments/environment.development';
import { ApiResponse } from '@/app/shared/models/ApiResponse';
import { User } from '@/app/shared/models/User';

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  email: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface UserStatsResponse {
  activeTournaments: number;
  teamsManaged: number;
  completedTournaments: number;
}

export interface ActivityItem {
  id: string;
  type: 'tournament' | 'team' | 'account';
  message: string;
  date: string;
  entityId?: string;
}

export interface UserActivityResponse {
  activities: ActivityItem[];
  totalCount: number;
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly env = environment;

  updateProfile(request: UpdateProfileRequest): Observable<ApiResponse<User>> {
    return this.http.put<ApiResponse<User>>(`${this.env.apiUrl}/users/profile`, request);
  }

  changePassword(request: ChangePasswordRequest): Observable<ApiResponse<void>> {
    return this.http.put<ApiResponse<void>>(`${this.env.apiUrl}/users/change-password`, request);
  }

  getUserStats(): Observable<ApiResponse<UserStatsResponse>> {
    return this.http.get<ApiResponse<UserStatsResponse>>(`${this.env.apiUrl}/users/stats`);
  }

  getUserActivity(page: number = 1, limit: number = 10): Observable<ApiResponse<UserActivityResponse>> {
    return this.http.get<ApiResponse<UserActivityResponse>>(
      `${this.env.apiUrl}/users/activity?page=${page}&limit=${limit}`
    );
  }

  uploadProfilePicture(file: File): Observable<ApiResponse<{ profilePictureUrl: string }>> {
    const formData = new FormData();
    formData.append('profilePicture', file);
    
    return this.http.post<ApiResponse<{ profilePictureUrl: string }>>(
      `${this.env.apiUrl}/users/profile-picture`, 
      formData
    );
  }
}
