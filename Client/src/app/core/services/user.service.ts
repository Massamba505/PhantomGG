import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@/environments/environment.development';
import { ApiResponse } from '@/app/shared/models/ApiResponse';
import { ChangePasswordRequest, UpdateProfileRequest, User } from '@/app/shared/models/User';



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

  uploadProfilePicture(file: File): Observable<ApiResponse<{ profilePictureUrl: string }>> {
    const formData = new FormData();
    formData.append('profilePicture', file);
    
    return this.http.post<ApiResponse<{ profilePictureUrl: string }>>(
      `${this.env.apiUrl}/users/profile-picture`, 
      formData
    );
  }
}
