import { Component, inject, signal, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ToastService } from '@/app/shared/services/toast.service';
import { Router, RouterLink } from '@angular/router';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { Modal } from "@/app/shared/components/ui/modal/modal";
import { UserService, UpdateProfileRequest, ChangePasswordRequest, UserStatsResponse, ActivityItem } from '@/app/core/services/user.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, DashboardLayout, RouterLink, ReactiveFormsModule, LucideAngularModule, Modal],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css']
})
export class Profile implements OnInit {
  private authState = inject(AuthStateService);
  private userService = inject(UserService);
  private toast = inject(ToastService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  readonly icons = LucideIcons;

  user = this.authState.user;
  isEditProfileModalOpen = signal(false);
  isChangePasswordModalOpen = signal(false);
  profilePictureUrl = signal<string>('');

  isUpdatingProfile = signal(false);
  isChangingPassword = signal(false);
  isLoadingStats = signal(false);
  isLoadingActivity = signal(false);

  stats = signal<UserStatsResponse>({
    activeTournaments: 0,
    teamsManaged: 0,
    completedTournaments: 0
  });

  recentActivity = signal<ActivityItem[]>([]);

  profileForm: FormGroup = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]]
  });

  passwordForm: FormGroup = this.fb.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', Validators.required]
  }, {
    validators: this.passwordMatchValidator
  });

  constructor() {
    effect(() => {
      const currentUser = this.user();
      if (currentUser) {
        this.profileForm.patchValue({
          firstName: currentUser.firstName,
          lastName: currentUser.lastName,
          email: currentUser.email
        });
        this.profilePictureUrl.set(currentUser.profilePictureUrl || '');
      }
    });
  }

  ngOnInit() {
    this.loadUserStats();
    this.loadUserActivity();
  }

  private async loadUserStats() {
    try {
      this.isLoadingStats.set(true);
      const response = await this.userService.getUserStats().toPromise();
      if (response?.success && response.data) {
        this.stats.set(response.data);
      }
    } catch (error) {
      console.error('Failed to load user stats:', error);
    } finally {
      this.isLoadingStats.set(false);
    }
  }

  private async loadUserActivity() {
    try {
      this.isLoadingActivity.set(true);
      const response = await this.userService.getUserActivity(1, 5).toPromise();
      if (response?.success && response.data) {
        this.recentActivity.set(response.data.activities);
      }
    } catch (error) {
      console.error('Failed to load user activity:', error);
    } finally {
      this.isLoadingActivity.set(false);
    }
  }

  onProfilePictureSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.uploadProfilePicture(file);
    }
  }

  private async uploadProfilePicture(file: File) {
    try {
      const response = await this.userService.uploadProfilePicture(file).toPromise();
      if (response?.success && response.data) {
        this.profilePictureUrl.set(response.data.profilePictureUrl);
        const currentUser = this.user();
        if (currentUser) {
          this.authState.updateUser({
            ...currentUser,
            profilePictureUrl: response.data.profilePictureUrl
          });
        }
        this.toast.success('Profile picture updated successfully');
      }
    } catch (error) {
      console.error('Failed to upload profile picture:', error);
      this.toast.error('Failed to upload profile picture');
    }
  }

  confirmLogout() {
    if (confirm('Are you sure you want to logout?')) {
      this.logout();
    }
  }

  logout() {
    this.authState.logout().subscribe({
      next: () => {
        this.router.navigate(['/']);
        this.toast.success('Logged out successfully');
      },
      error: (error) => {
        console.error('Logout error:', error);
        this.router.navigate(['/']);
        this.toast.success('Logged out successfully');
      }
    });
  }

  async updateProfile() {
    if (this.profileForm.invalid) {
      this.toast.error('Please fill in all required fields correctly');
      return;
    }
    
    try {
      this.isUpdatingProfile.set(true);
      const formValue = this.profileForm.value;
      const request: UpdateProfileRequest = {
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        email: formValue.email
      };

      const response = await this.userService.updateProfile(request).toPromise();
      
      if (response?.success && response.data) {
        this.authState.updateUser(response.data);
        this.isEditProfileModalOpen.set(false);
        this.toast.success('Profile updated successfully');
      }
    } catch (error: any) {
      console.error('Failed to update profile:', error);
      this.toast.error(error?.error?.message || 'Failed to update profile');
    } finally {
      this.isUpdatingProfile.set(false);
    }
  }

  async updatePassword() {
    if (this.passwordForm.invalid) {
      this.toast.error('Please fill in all fields correctly');
      return;
    }
    
    if (this.passwordForm.hasError('mismatch')) {
      this.toast.error('New passwords do not match');
      return;
    }
    
    try {
      this.isChangingPassword.set(true);
      const formValue = this.passwordForm.value;
      const request: ChangePasswordRequest = {
        currentPassword: formValue.currentPassword,
        newPassword: formValue.newPassword
      };

      const response = await this.userService.changePassword(request).toPromise();
      
      if (response?.success) {
        this.isChangePasswordModalOpen.set(false);
        this.passwordForm.reset();
        this.toast.success('Password updated successfully');
      }
    } catch (error: any) {
      console.error('Failed to update password:', error);
      this.toast.error(error?.error?.message || 'Failed to update password');
    } finally {
      this.isChangingPassword.set(false);
    }
  }

  passwordMatchValidator(g: FormGroup) {
    const newPassword = g.get('newPassword')?.value;
    const confirmPassword = g.get('confirmPassword')?.value;
    
    return newPassword === confirmPassword ? null : { mismatch: true };
  }
}
