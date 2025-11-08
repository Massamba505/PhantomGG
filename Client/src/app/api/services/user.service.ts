import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import { 
  UserDto, 
  UpdateUserProfileRequest, 
  ChangePasswordRequest, 
  ProfilePictureUploadDto 
} from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiClient = inject(ApiClient);

  getProfile(): Observable<UserDto> {
    return this.apiClient.get<UserDto>(API_ENDPOINTS.USERS.PROFILE);
  }

  updateProfile(profileData: UpdateUserProfileRequest): Observable<UserDto> {
    return this.apiClient.patch<UserDto>(API_ENDPOINTS.USERS.UPDATE_PROFILE, profileData);
  }

  changePassword(passwordData: ChangePasswordRequest): Observable<void> {
    return this.apiClient.patch<void>(API_ENDPOINTS.USERS.CHANGE_PASSWORD, passwordData);
  }

  uploadProfilePicture(file: File): Observable<ProfilePictureUploadDto> {
    const formData = new FormData();
    formData.append('profilePicture', file);
    return this.apiClient.postFormData<ProfilePictureUploadDto>(API_ENDPOINTS.USERS.UPLOAD_PROFILE_PICTURE, formData);
  }
}