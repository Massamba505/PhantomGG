import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import { ChangePassword, ProfilePictureUpload, UpdateUserProfile, User } from '../models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiClient = inject(ApiClient);

  // User profile operations
  getProfile(): Observable<User> {
    return this.apiClient.get<User>(API_ENDPOINTS.USERS.PROFILE);
  }

  updateProfile(profileData: UpdateUserProfile): Observable<User> {
    return this.apiClient.put<User>(API_ENDPOINTS.USERS.UPDATE_PROFILE, profileData);
  }

  changePassword(passwordData: ChangePassword): Observable<void> {
    return this.apiClient.put<void>(API_ENDPOINTS.USERS.CHANGE_PASSWORD, passwordData);
  }

  uploadProfilePicture(file: File): Observable<ProfilePictureUpload> {
    return this.apiClient.uploadFile<ProfilePictureUpload>(API_ENDPOINTS.USERS.UPLOAD_PROFILE_PICTURE, file);
  }
}